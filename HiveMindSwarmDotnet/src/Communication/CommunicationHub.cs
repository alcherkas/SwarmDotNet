using System.Collections.Concurrent;
using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;

namespace HiveMindSwarmDotnet.Console.Communication;

public class CommunicationHub : ICommunicationHub
{
    private readonly ILogger<CommunicationHub> _logger;
    private readonly ConcurrentDictionary<string, List<AgentMessage>> _messageQueues = new();
    private readonly ConcurrentDictionary<string, List<CommunicationLink>> _communicationLinks = new();
    private readonly ConcurrentDictionary<string, DateTime> _lastActivity = new();
    private readonly Random _random = new();

    public CommunicationHub(ILogger<CommunicationHub> logger)
    {
        _logger = logger;
    }

    public async Task SendMessageAsync(AgentMessage message, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(message.ToAgentId))
        {
            throw new ArgumentException("Target agent ID cannot be empty");
        }

        _logger.LogDebug("Sending message from {FromAgent} to {ToAgent}: {MessageType}", 
            message.FromAgentId, message.ToAgentId, message.Type);

        var targetQueue = _messageQueues.GetOrAdd(message.ToAgentId, _ => new List<AgentMessage>());
        
        lock (targetQueue)
        {
            targetQueue.Add(message);
        }

        await UpdateCommunicationLinkAsync(message.FromAgentId, message.ToAgentId, cancellationToken);
        await Task.CompletedTask;
    }

    public async Task BroadcastAsync(string fromAgentId, AgentMessage message, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Broadcasting message from {FromAgent}: {MessageType}", fromAgentId, message.Type);

        var activeAgents = _messageQueues.Keys.Where(k => k != fromAgentId).ToList();
        
        var broadcastTasks = activeAgents.Select(async agentId =>
        {
            var broadcastMessage = new AgentMessage
            {
                FromAgentId = message.FromAgentId,
                ToAgentId = agentId,
                Content = message.Content,
                Type = message.Type,
                CreatedAt = message.CreatedAt,
                Metadata = new Dictionary<string, object>(message.Metadata)
            };

            await SendMessageAsync(broadcastMessage, cancellationToken);
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

        _lastActivity[agentId] = DateTime.UtcNow;
        
        _logger.LogDebug("Retrieved {MessageCount} messages for agent {AgentId}", result.Count, agentId);
        return await Task.FromResult(result);
    }

    public async Task<List<CommunicationLink>> GetCommunicationLinksAsync(string agentId, CancellationToken cancellationToken = default)
    {
        if (!_communicationLinks.TryGetValue(agentId, out var links))
        {
            return new List<CommunicationLink>();
        }

        var result = new List<CommunicationLink>(links);
        return await Task.FromResult(result);
    }

    public async Task OptimizeCommunicationAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Optimizing communication topology");

        await PruneInactiveLinksAsync(cancellationToken);
        await AdjustLinkProbabilitiesAsync(cancellationToken);
        await CreateDynamicLinksAsync(cancellationToken);
    }

    private async Task UpdateCommunicationLinkAsync(string fromAgentId, string toAgentId, CancellationToken cancellationToken)
    {
        var fromLinks = _communicationLinks.GetOrAdd(fromAgentId, _ => new List<CommunicationLink>());
        var toLinks = _communicationLinks.GetOrAdd(toAgentId, _ => new List<CommunicationLink>());

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
                    Probability = 1.0
                };
                fromLinks.Add(link);
            }

            link.MessageCount++;
            link.LastUsed = DateTime.UtcNow;
            link.Weight = Math.Min(10.0, link.Weight + 0.1);
        }

        lock (toLinks)
        {
            var reverseLink = toLinks.FirstOrDefault(l => l.ToAgentId == fromAgentId);
            if (reverseLink == null)
            {
                reverseLink = new CommunicationLink
                {
                    FromAgentId = toAgentId,
                    ToAgentId = fromAgentId,
                    Weight = 0.5,
                    Probability = 0.8
                };
                toLinks.Add(reverseLink);
            }

            reverseLink.LastUsed = DateTime.UtcNow;
        }

        await Task.CompletedTask;
    }

    private async Task PruneInactiveLinksAsync(CancellationToken cancellationToken)
    {
        var cutoffTime = DateTime.UtcNow.AddMinutes(-5);
        var prunedCount = 0;

        foreach (var agentLinks in _communicationLinks.Values)
        {
            lock (agentLinks)
            {
                var toRemove = agentLinks.Where(l => l.LastUsed < cutoffTime && l.MessageCount < 2).ToList();
                foreach (var link in toRemove)
                {
                    agentLinks.Remove(link);
                    prunedCount++;
                }
            }
        }

        if (prunedCount > 0)
        {
            _logger.LogDebug("Pruned {PrunedCount} inactive communication links", prunedCount);
        }

        await Task.CompletedTask;
    }

    private async Task AdjustLinkProbabilitiesAsync(CancellationToken cancellationToken)
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
                        if (timeSinceLastUse.TotalMinutes > 2)
                        {
                            link.Probability *= 0.9;
                        }
                    }
                }
            }
        }

        await Task.CompletedTask;
    }

    private async Task CreateDynamicLinksAsync(CancellationToken cancellationToken)
    {
        var allAgents = _messageQueues.Keys.ToList();
        if (allAgents.Count < 2)
            return;

        var newLinksCreated = 0;
        var maxNewLinks = Math.Min(5, allAgents.Count);

        for (int i = 0; i < maxNewLinks; i++)
        {
            var fromAgent = allAgents[_random.Next(allAgents.Count)];
            var toAgent = allAgents[_random.Next(allAgents.Count)];

            if (fromAgent == toAgent)
                continue;

            var fromLinks = _communicationLinks.GetOrAdd(fromAgent, _ => new List<CommunicationLink>());
            
            lock (fromLinks)
            {
                if (!fromLinks.Any(l => l.ToAgentId == toAgent))
                {
                    var dynamicLink = new CommunicationLink
                    {
                        FromAgentId = fromAgent,
                        ToAgentId = toAgent,
                        Weight = 0.3,
                        Probability = 0.3,
                        LastUsed = DateTime.UtcNow
                    };

                    fromLinks.Add(dynamicLink);
                    newLinksCreated++;
                }
            }
        }

        if (newLinksCreated > 0)
        {
            _logger.LogDebug("Created {NewLinksCount} dynamic communication links", newLinksCreated);
        }

        await Task.CompletedTask;
    }

    public bool ShouldSendMessage(string fromAgentId, string toAgentId)
    {
        if (!_communicationLinks.TryGetValue(fromAgentId, out var links))
            return true;

        var link = links.FirstOrDefault(l => l.ToAgentId == toAgentId);
        if (link == null)
            return true;

        return _random.NextDouble() < link.Probability;
    }

    public async Task RegisterAgentAsync(string agentId, CancellationToken cancellationToken = default)
    {
        _messageQueues.TryAdd(agentId, new List<AgentMessage>());
        _communicationLinks.TryAdd(agentId, new List<CommunicationLink>());
        _lastActivity[agentId] = DateTime.UtcNow;
        
        _logger.LogInformation("Registered agent {AgentId} for communication", agentId);
        await Task.CompletedTask;
    }

    public async Task UnregisterAgentAsync(string agentId, CancellationToken cancellationToken = default)
    {
        _messageQueues.TryRemove(agentId, out _);
        _communicationLinks.TryRemove(agentId, out _);
        _lastActivity.TryRemove(agentId, out _);
        
        foreach (var agentLinks in _communicationLinks.Values)
        {
            lock (agentLinks)
            {
                agentLinks.RemoveAll(l => l.ToAgentId == agentId);
            }
        }

        _logger.LogInformation("Unregistered agent {AgentId} from communication", agentId);
        await Task.CompletedTask;
    }
}