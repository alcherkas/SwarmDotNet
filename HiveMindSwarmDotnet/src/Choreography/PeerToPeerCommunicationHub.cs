using System.Collections.Concurrent;
using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;

namespace HiveMindSwarmDotnet.Console.Choreography;

public class PeerToPeerCommunicationHub : ICommunicationHub
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<PeerToPeerCommunicationHub> _logger;
    private readonly ConcurrentDictionary<string, List<AgentMessage>> _messageQueues = new();
    private readonly ConcurrentDictionary<string, List<CommunicationLink>> _communicationLinks = new();
    private readonly ConcurrentDictionary<string, AgentPeerInfo> _peerRegistry = new();
    private readonly ConcurrentDictionary<string, List<string>> _peerGroups = new();
    private readonly Random _random = new();

    public PeerToPeerCommunicationHub(IEventBus eventBus, ILogger<PeerToPeerCommunicationHub> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task SendMessageAsync(AgentMessage message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(message.ToAgentId))
        {
            throw new ArgumentException("Target agent ID cannot be empty");
        }

        _logger.LogDebug("P2P message from {FromAgent} to {ToAgent}: {MessageType}", 
            message.FromAgentId, message.ToAgentId, message.Type);

        var targetQueue = _messageQueues.GetOrAdd(message.ToAgentId, _ => new List<AgentMessage>());
        
        lock (targetQueue)
        {
            targetQueue.Add(message);
        }

        await UpdatePeerConnectionAsync(message.FromAgentId, message.ToAgentId, cancellationToken);
        await PublishCommunicationEventAsync(message, cancellationToken);
    }

    public async Task BroadcastAsync(string fromAgentId, AgentMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("P2P broadcast from {FromAgent}: {MessageType}", fromAgentId, message.Type);

        var targetPeers = await GetBroadcastTargetsAsync(fromAgentId, cancellationToken);
        
        var broadcastTasks = targetPeers.Select(async peerId =>
        {
            var peerMessage = new AgentMessage
            {
                FromAgentId = message.FromAgentId,
                ToAgentId = peerId,
                Content = message.Content,
                Type = message.Type,
                CreatedAt = message.CreatedAt,
                Metadata = new Dictionary<string, object>(message.Metadata)
            };

            await SendMessageAsync(peerMessage, cancellationToken);
        });

        await Task.WhenAll(broadcastTasks);
    }

    public async Task<List<AgentMessage>> GetMessagesAsync(string agentId, CancellationToken cancellationToken = default)
    {
        if (!_messageQueues.TryGetValue(agentId, out var messages))
        {
            return new List<AgentMessage>();
        }

        List<AgentMessage> result;
        lock (messages)
        {
            result = new List<AgentMessage>(messages);
            messages.Clear();
        }

        await UpdatePeerActivityAsync(agentId, cancellationToken);
        
        _logger.LogDebug("Retrieved {MessageCount} P2P messages for agent {AgentId}", result.Count, agentId);
        return result;
    }

    public async Task<List<CommunicationLink>> GetCommunicationLinksAsync(string agentId, CancellationToken cancellationToken = default)
    {
        if (!_communicationLinks.TryGetValue(agentId, out var links))
        {
            return new List<CommunicationLink>();
        }

        return await Task.FromResult(new List<CommunicationLink>(links));
    }

    public async Task OptimizeCommunicationAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Optimizing P2P communication topology");

        await PruneInactiveConnections(cancellationToken);
        await FormPeerGroupsAsync(cancellationToken);
        await OptimizePeerConnectionsAsync(cancellationToken);
        await PublishTopologyUpdateAsync(cancellationToken);
    }

    public bool ShouldSendMessage(string fromAgentId, string toAgentId)
    {
        if (!_communicationLinks.TryGetValue(fromAgentId, out var links))
            return true;

        var link = links.FirstOrDefault(l => l.ToAgentId == toAgentId);
        if (link == null)
            return true;

        var shouldSend = _random.NextDouble() < link.Probability;
        
        if (shouldSend)
        {
            link.MessageCount++;
            link.LastUsed = DateTime.UtcNow;
        }

        return shouldSend;
    }

    public async Task RegisterAgentAsync(string agentId, CancellationToken cancellationToken = default)
    {
        _messageQueues.TryAdd(agentId, new List<AgentMessage>());
        _communicationLinks.TryAdd(agentId, new List<CommunicationLink>());
        
        var peerInfo = new AgentPeerInfo
        {
            AgentId = agentId,
            RegisteredAt = DateTime.UtcNow,
            LastActivity = DateTime.UtcNow,
            ConnectionCount = 0,
            IsActive = true
        };

        _peerRegistry.TryAdd(agentId, peerInfo);
        
        await EstablishInitialConnectionsAsync(agentId, cancellationToken);
        
        _logger.LogInformation("Registered P2P agent {AgentId}", agentId);
    }

    public async Task UnregisterAgentAsync(string agentId, CancellationToken cancellationToken = default)
    {
        _messageQueues.TryRemove(agentId, out _);
        _communicationLinks.TryRemove(agentId, out _);
        _peerRegistry.TryRemove(agentId, out _);
        
        await RemoveFromPeerGroupsAsync(agentId, cancellationToken);
        await NotifyPeerDisconnectionAsync(agentId, cancellationToken);
        
        _logger.LogInformation("Unregistered P2P agent {AgentId}", agentId);
    }

    public async Task<List<string>> GetConnectedPeersAsync(string agentId, CancellationToken cancellationToken = default)
    {
        if (!_communicationLinks.TryGetValue(agentId, out var links))
        {
            return new List<string>();
        }

        var connectedPeers = links
            .Where(l => l.LastUsed > DateTime.UtcNow.AddMinutes(-5))
            .Select(l => l.ToAgentId)
            .ToList();

        return await Task.FromResult(connectedPeers);
    }

    public async Task InitiatePeerDiscoveryAsync(string agentId, CancellationToken cancellationToken = default)
    {
        var discoveryEvent = new PeerDiscoveryEvent
        {
            SourceAgentId = agentId,
            DiscoveryType = "peer_search",
            Payload = new Dictionary<string, object>
            {
                ["requesting_agent"] = agentId,
                ["discovery_time"] = DateTime.UtcNow
            }
        };

        await _eventBus.PublishAsync(discoveryEvent, cancellationToken);
        
        _logger.LogInformation("Initiated peer discovery for agent {AgentId}", agentId);
    }

    private async Task<List<string>> GetBroadcastTargetsAsync(string fromAgentId, CancellationToken cancellationToken)
    {
        var targets = new List<string>();
        
        if (_peerGroups.TryGetValue(fromAgentId, out var groupMembers))
        {
            targets.AddRange(groupMembers.Where(m => m != fromAgentId));
        }

        if (_communicationLinks.TryGetValue(fromAgentId, out var links))
        {
            var activeLinks = links
                .Where(l => l.LastUsed > DateTime.UtcNow.AddMinutes(-2))
                .Select(l => l.ToAgentId)
                .ToList();
            
            targets.AddRange(activeLinks);
        }

        return await Task.FromResult(targets.Distinct().ToList());
    }

    private async Task UpdatePeerConnectionAsync(string fromAgentId, string toAgentId, CancellationToken cancellationToken)
    {
        var fromLinks = _communicationLinks.GetOrAdd(fromAgentId, _ => new List<CommunicationLink>());
        
        lock (fromLinks)
        {
            var link = fromLinks.FirstOrDefault(l => l.ToAgentId == toAgentId);
            if (link == null)
            {
                link = new CommunicationLink
                {
                    FromAgentId = fromAgentId,
                    ToAgentId = toAgentId,
                    Weight = 1.0,
                    Probability = 1.0,
                    MessageCount = 0
                };
                fromLinks.Add(link);
            }

            link.MessageCount++;
            link.LastUsed = DateTime.UtcNow;
            link.Weight = Math.Min(10.0, link.Weight + 0.1);
        }

        await Task.CompletedTask;
    }

    private async Task UpdatePeerActivityAsync(string agentId, CancellationToken cancellationToken)
    {
        if (_peerRegistry.TryGetValue(agentId, out var peerInfo))
        {
            peerInfo.LastActivity = DateTime.UtcNow;
            peerInfo.IsActive = true;
        }

        await Task.CompletedTask;
    }

    private async Task EstablishInitialConnectionsAsync(string agentId, CancellationToken cancellationToken)
    {
        var activePeers = _peerRegistry.Values
            .Where(p => p.IsActive && p.AgentId != agentId)
            .OrderBy(p => p.ConnectionCount)
            .Take(3)
            .ToList();

        var links = _communicationLinks.GetOrAdd(agentId, _ => new List<CommunicationLink>());
        
        foreach (var peer in activePeers)
        {
            var link = new CommunicationLink
            {
                FromAgentId = agentId,
                ToAgentId = peer.AgentId,
                Weight = 0.5,
                Probability = 0.8,
                LastUsed = DateTime.UtcNow
            };

            lock (links)
            {
                links.Add(link);
            }

            peer.ConnectionCount++;
            
            _logger.LogDebug("Established initial connection from {FromAgent} to {ToAgent}", agentId, peer.AgentId);
        }

        await Task.CompletedTask;
    }

    private async Task FormPeerGroupsAsync(CancellationToken cancellationToken)
    {
        var activePeers = _peerRegistry.Values
            .Where(p => p.IsActive && p.LastActivity > DateTime.UtcNow.AddMinutes(-10))
            .ToList();

        var groupSize = Math.Min(5, Math.Max(3, activePeers.Count / 3));
        var groups = new List<List<string>>();
        
        foreach (var peer in activePeers)
        {
            var existingGroup = groups.FirstOrDefault(g => g.Count < groupSize);
            if (existingGroup != null)
            {
                existingGroup.Add(peer.AgentId);
            }
            else
            {
                groups.Add(new List<string> { peer.AgentId });
            }
        }

        foreach (var group in groups)
        {
            foreach (var memberId in group)
            {
                _peerGroups.AddOrUpdate(memberId, group, (_, _) => group);
            }
        }

        _logger.LogDebug("Formed {GroupCount} peer groups with average size {AvgSize}", 
            groups.Count, groups.Count > 0 ? groups.Average(g => g.Count) : 0);

        await Task.CompletedTask;
    }

    private async Task OptimizePeerConnectionsAsync(CancellationToken cancellationToken)
    {
        foreach (var agentLinks in _communicationLinks.Values)
        {
            lock (agentLinks)
            {
                var totalWeight = agentLinks.Sum(l => l.Weight);
                if (totalWeight > 0)
                {
                    foreach (var link in agentLinks)
                    {
                        link.Probability = link.Weight / totalWeight;
                        
                        var timeSinceLastUse = DateTime.UtcNow - link.LastUsed;
                        if (timeSinceLastUse.TotalMinutes > 5)
                        {
                            link.Probability *= 0.8;
                        }
                    }
                }
            }
        }

        await Task.CompletedTask;
    }

    private async Task PruneInactiveConnections(CancellationToken cancellationToken)
    {
        var cutoffTime = DateTime.UtcNow.AddMinutes(-10);
        var prunedCount = 0;

        foreach (var agentLinks in _communicationLinks.Values)
        {
            lock (agentLinks)
            {
                var toRemove = agentLinks.Where(l => l.LastUsed < cutoffTime && l.MessageCount < 3).ToList();
                foreach (var link in toRemove)
                {
                    agentLinks.Remove(link);
                    prunedCount++;
                }
            }
        }

        if (prunedCount > 0)
        {
            _logger.LogDebug("Pruned {PrunedCount} inactive P2P connections", prunedCount);
        }

        await Task.CompletedTask;
    }

    private async Task RemoveFromPeerGroupsAsync(string agentId, CancellationToken cancellationToken)
    {
        if (_peerGroups.TryGetValue(agentId, out var group))
        {
            group.Remove(agentId);
            _peerGroups.TryRemove(agentId, out _);
        }

        await Task.CompletedTask;
    }

    private async Task NotifyPeerDisconnectionAsync(string agentId, CancellationToken cancellationToken)
    {
        var disconnectionEvent = new PeerDisconnectionEvent
        {
            SourceAgentId = agentId,
            DisconnectedAgentId = agentId,
            Payload = new Dictionary<string, object>
            {
                ["disconnection_time"] = DateTime.UtcNow
            }
        };

        await _eventBus.PublishAsync(disconnectionEvent, cancellationToken);
    }

    private async Task PublishCommunicationEventAsync(AgentMessage message, CancellationToken cancellationToken)
    {
        var communicationEvent = new PeerCommunicationEvent
        {
            SourceAgentId = message.FromAgentId,
            FromAgentId = message.FromAgentId,
            ToAgentId = message.ToAgentId,
            MessageType = message.Type,
            Payload = new Dictionary<string, object>
            {
                ["message_content"] = message.Content,
                ["message_metadata"] = message.Metadata
            }
        };

        await _eventBus.PublishAsync(communicationEvent, cancellationToken);
    }

    private async Task PublishTopologyUpdateAsync(CancellationToken cancellationToken)
    {
        var topologyEvent = new TopologyUpdateEvent
        {
            SourceAgentId = "communication_hub",
            ActivePeers = _peerRegistry.Values.Where(p => p.IsActive).Select(p => p.AgentId).ToList(),
            ConnectionCount = _communicationLinks.Values.Sum(links => links.Count),
            Payload = new Dictionary<string, object>
            {
                ["update_time"] = DateTime.UtcNow,
                ["peer_groups"] = _peerGroups.Count
            }
        };

        await _eventBus.PublishAsync(topologyEvent, cancellationToken);
    }

    private class AgentPeerInfo
    {
        public string AgentId { get; set; } = string.Empty;
        public DateTime RegisteredAt { get; set; }
        public DateTime LastActivity { get; set; }
        public int ConnectionCount { get; set; }
        public bool IsActive { get; set; }
    }
}

public class PeerDiscoveryEvent : SwarmEvent
{
    public override string EventType => "PeerDiscovery";
    public string DiscoveryType { get; set; } = string.Empty;
}

public class PeerDisconnectionEvent : SwarmEvent
{
    public override string EventType => "PeerDisconnection";
    public string DisconnectedAgentId { get; set; } = string.Empty;
}

public class PeerCommunicationEvent : SwarmEvent
{
    public override string EventType => "PeerCommunication";
    public string FromAgentId { get; set; } = string.Empty;
    public string ToAgentId { get; set; } = string.Empty;
    public MessageType MessageType { get; set; }
}

public class TopologyUpdateEvent : SwarmEvent
{
    public override string EventType => "TopologyUpdate";
    public List<string> ActivePeers { get; set; } = new();
    public int ConnectionCount { get; set; }
}