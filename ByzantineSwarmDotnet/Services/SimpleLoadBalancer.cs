using Microsoft.Extensions.Logging;
using ByzantineSwarmDotnet.Interfaces;

namespace ByzantineSwarmDotnet.Services;

public class SimpleLoadBalancer : ILoadBalancer
{
    private readonly ILogger<SimpleLoadBalancer> _logger;
    private readonly IHierarchicalAgentPool _agentPool;
    private readonly Dictionary<string, int> _agentLoads;
    private readonly object _lockObject = new();

    public SimpleLoadBalancer(ILogger<SimpleLoadBalancer> logger, IHierarchicalAgentPool agentPool)
    {
        _logger = logger;
        _agentPool = agentPool;
        _agentLoads = new Dictionary<string, int>();
    }

    public async Task<string> SelectOptimalAgentAsync(List<string> candidateAgents, Models.AgentTask task)
    {
        try
        {
            if (!candidateAgents.Any())
            {
                throw new InvalidOperationException("No candidate agents provided");
            }

            // Get current loads
            var agentLoads = new Dictionary<string, int>();
            lock (_lockObject)
            {
                foreach (var agentId in candidateAgents)
                {
                    agentLoads[agentId] = _agentLoads.GetValueOrDefault(agentId, 0);
                }
            }

            // Select agent with lowest load
            var selectedAgent = agentLoads.OrderBy(kvp => kvp.Value).First().Key;

            // Update load
            lock (_lockObject)
            {
                _agentLoads[selectedAgent] = _agentLoads.GetValueOrDefault(selectedAgent, 0) + 1;
            }

            _logger.LogDebug("Selected agent {AgentId} for task {TaskId} (current load: {Load})", 
                selectedAgent, task.Id, _agentLoads[selectedAgent]);

            return selectedAgent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting optimal agent");
            return candidateAgents.First(); // Fallback to first agent
        }
    }

    public async Task<Dictionary<string, int>> GetCurrentLoadDistributionAsync()
    {
        lock (_lockObject)
        {
            return new Dictionary<string, int>(_agentLoads);
        }
    }

    public async Task RebalanceLoadAsync()
    {
        try
        {
            var loadDistribution = await GetCurrentLoadDistributionAsync();
            var activeAgents = await _agentPool.GetActiveAgentsAsync();

            if (!loadDistribution.Any() || !activeAgents.Any())
                return;

            var maxLoad = loadDistribution.Values.Max();
            var minLoad = loadDistribution.Values.Min();
            var threshold = 5; // Rebalance if difference is more than 5 tasks

            if (maxLoad - minLoad <= threshold)
                return;

            _logger.LogInformation("Rebalancing load: max={MaxLoad}, min={MinLoad}", maxLoad, minLoad);

            // Simple rebalancing: reduce load on overloaded agents
            lock (_lockObject)
            {
                var overloadedAgents = loadDistribution.Where(kvp => kvp.Value > maxLoad - 2).ToList();
                var underloadedAgents = loadDistribution.Where(kvp => kvp.Value < minLoad + 2).ToList();

                foreach (var overloaded in overloadedAgents)
                {
                    if (underloadedAgents.Any())
                    {
                        var targetAgent = underloadedAgents.First();
                        var loadToTransfer = Math.Min(2, overloaded.Value - targetAgent.Value);
                        
                        _agentLoads[overloaded.Key] -= loadToTransfer;
                        _agentLoads[targetAgent.Key] += loadToTransfer;
                        
                        _logger.LogDebug("Transferred {Load} load from {From} to {To}", 
                            loadToTransfer, overloaded.Key, targetAgent.Key);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rebalancing load");
        }
    }

    public async Task<bool> IsAgentOverloadedAsync(string agentId)
    {
        lock (_lockObject)
        {
            var load = _agentLoads.GetValueOrDefault(agentId, 0);
            return load > 10; // Consider agent overloaded if more than 10 concurrent tasks
        }
    }

    public void DecrementLoad(string agentId)
    {
        lock (_lockObject)
        {
            if (_agentLoads.ContainsKey(agentId) && _agentLoads[agentId] > 0)
            {
                _agentLoads[agentId]--;
            }
        }
    }
}
