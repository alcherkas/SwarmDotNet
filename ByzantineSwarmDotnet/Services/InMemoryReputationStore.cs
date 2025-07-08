using Microsoft.Extensions.Logging;
using ByzantineSwarmDotnet.Models;
using ByzantineSwarmDotnet.Interfaces;

namespace ByzantineSwarmDotnet.Services;

public class InMemoryReputationStore : IDistributedReputationStore
{
    private readonly ILogger<InMemoryReputationStore> _logger;
    private readonly Dictionary<string, double> _reputations;
    private readonly Dictionary<string, List<ReputationUpdate>> _history;
    private readonly object _lockObject = new();

    public InMemoryReputationStore(ILogger<InMemoryReputationStore> logger)
    {
        _logger = logger;
        _reputations = new Dictionary<string, double>();
        _history = new Dictionary<string, List<ReputationUpdate>>();
    }

    public async Task<double> GetReputationAsync(string agentId)
    {
        lock (_lockObject)
        {
            if (_reputations.TryGetValue(agentId, out var reputation))
            {
                return reputation;
            }
            
            // Initialize with default reputation
            _reputations[agentId] = 0.5;
            return 0.5;
        }
    }

    public async Task UpdateReputationAsync(ReputationUpdate update)
    {
        lock (_lockObject)
        {
            // Get current reputation
            var currentReputation = _reputations.GetValueOrDefault(update.AgentId, 0.5);
            
            // Calculate new reputation
            var newReputation = Math.Max(0, Math.Min(1, currentReputation + update.ScoreChange));
            _reputations[update.AgentId] = newReputation;
            
            // Store in history
            if (!_history.ContainsKey(update.AgentId))
            {
                _history[update.AgentId] = new List<ReputationUpdate>();
            }
            
            _history[update.AgentId].Add(update);
            
            // Keep only last 1000 updates per agent
            if (_history[update.AgentId].Count > 1000)
            {
                _history[update.AgentId].RemoveAt(0);
            }
        }
        
        _logger.LogDebug("Updated reputation for agent {AgentId}: {NewReputation:F3}", 
            update.AgentId, _reputations[update.AgentId]);
    }

    public async Task<Dictionary<string, double>> GetAllReputationsAsync()
    {
        lock (_lockObject)
        {
            return new Dictionary<string, double>(_reputations);
        }
    }

    public async Task<List<ReputationUpdate>> GetReputationHistoryAsync(string agentId)
    {
        lock (_lockObject)
        {
            if (_history.TryGetValue(agentId, out var history))
            {
                return new List<ReputationUpdate>(history);
            }
            return new List<ReputationUpdate>();
        }
    }
}
