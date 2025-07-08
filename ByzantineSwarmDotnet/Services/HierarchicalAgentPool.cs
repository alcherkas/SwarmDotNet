using Microsoft.Extensions.Logging;
using ByzantineSwarmDotnet.Models;
using ByzantineSwarmDotnet.Interfaces;

namespace ByzantineSwarmDotnet.Services;

public class HierarchicalAgentPool : IHierarchicalAgentPool
{
    private readonly ILogger<HierarchicalAgentPool> _logger;
    private readonly Dictionary<string, AgentInfo> _agents;
    private readonly Dictionary<AgentRole, List<string>> _agentsByRole;
    private readonly Dictionary<AgentHierarchyLevel, List<string>> _agentsByLevel;
    private readonly object _lockObject = new();
    private int _nextAgentId = 1;

    public HierarchicalAgentPool(ILogger<HierarchicalAgentPool> logger)
    {
        _logger = logger;
        _agents = new Dictionary<string, AgentInfo>();
        _agentsByRole = new Dictionary<AgentRole, List<string>>();
        _agentsByLevel = new Dictionary<AgentHierarchyLevel, List<string>>();

        // Initialize role and level dictionaries
        foreach (AgentRole role in Enum.GetValues<AgentRole>())
        {
            _agentsByRole[role] = new List<string>();
        }

        foreach (AgentHierarchyLevel level in Enum.GetValues<AgentHierarchyLevel>())
        {
            _agentsByLevel[level] = new List<string>();
        }
    }

    public async Task<List<AgentInfo>> GetActiveAgentsAsync()
    {
        lock (_lockObject)
        {
            return _agents.Values.Where(a => a.IsActive).ToList();
        }
    }

    public async Task<List<AgentInfo>> GetAgentsByRoleAsync(AgentRole role)
    {
        lock (_lockObject)
        {
            if (_agentsByRole.TryGetValue(role, out var agentIds))
            {
                return agentIds.Select(id => _agents[id]).Where(a => a.IsActive).ToList();
            }
            return new List<AgentInfo>();
        }
    }

    public async Task<List<AgentInfo>> GetAgentsByHierarchyLevelAsync(AgentHierarchyLevel level)
    {
        lock (_lockObject)
        {
            if (_agentsByLevel.TryGetValue(level, out var agentIds))
            {
                return agentIds.Select(id => _agents[id]).Where(a => a.IsActive).ToList();
            }
            return new List<AgentInfo>();
        }
    }

    public async Task<AgentInfo?> GetAgentAsync(string agentId)
    {
        lock (_lockObject)
        {
            return _agents.TryGetValue(agentId, out var agent) ? agent : null;
        }
    }

    public async Task<string> RegisterAgentAsync(AgentInfo agent)
    {
        try
        {
            lock (_lockObject)
            {
                if (_agents.ContainsKey(agent.Id))
                {
                    throw new InvalidOperationException($"Agent {agent.Id} is already registered");
                }

                _agents[agent.Id] = agent;
                _agentsByRole[agent.Role].Add(agent.Id);
                _agentsByLevel[agent.HierarchyLevel].Add(agent.Id);
            }

            _logger.LogInformation("Registered agent {AgentId} with role {Role} at level {Level}",
                agent.Id, agent.Role, agent.HierarchyLevel);

            return agent.Id;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering agent {AgentId}", agent.Id);
            throw;
        }
    }

    public async Task<bool> DeregisterAgentAsync(string agentId)
    {
        try
        {
            lock (_lockObject)
            {
                if (!_agents.TryGetValue(agentId, out var agent))
                {
                    return false;
                }

                _agents.Remove(agentId);
                _agentsByRole[agent.Role].Remove(agentId);
                _agentsByLevel[agent.HierarchyLevel].Remove(agentId);
            }

            _logger.LogInformation("Deregistered agent {AgentId}", agentId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deregistering agent {AgentId}", agentId);
            return false;
        }
    }

    public async Task ScaleUpAsync(int additionalAgents)
    {
        try
        {
            _logger.LogInformation("Scaling up by {Count} agents", additionalAgents);

            for (int i = 0; i < additionalAgents; i++)
            {
                var agentId = $"agent-{_nextAgentId++}";
                
                // Determine role and hierarchy level based on current distribution
                var (role, level) = DetermineOptimalRoleAndLevel();

                var agent = new AgentInfo
                {
                    Id = agentId,
                    Role = role,
                    HierarchyLevel = level,
                    ReputationScore = 0.5, // Start with neutral reputation
                    Capabilities = GetCapabilitiesForRole(role),
                    LastHeartbeat = DateTime.UtcNow,
                    IsActive = true,
                    PublicKey = GeneratePublicKey(),
                    CurrentLoad = 0,
                    AverageResponseTime = TimeSpan.FromSeconds(1)
                };

                await RegisterAgentAsync(agent);
            }

            _logger.LogInformation("Successfully scaled up {Count} agents", additionalAgents);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scaling up agents");
            throw;
        }
    }

    public async Task ScaleDownAsync(int agentsToRemove)
    {
        try
        {
            _logger.LogInformation("Scaling down by {Count} agents", agentsToRemove);

            var activeAgents = await GetActiveAgentsAsync();
            
            // Select agents to remove (prefer agents with lower reputation and higher load)
            var agentsToDeregister = activeAgents
                .OrderBy(a => a.ReputationScore)
                .ThenByDescending(a => a.CurrentLoad)
                .Take(agentsToRemove)
                .ToList();

            foreach (var agent in agentsToDeregister)
            {
                await DeregisterAgentAsync(agent.Id);
            }

            _logger.LogInformation("Successfully scaled down {Count} agents", agentsToDeregister.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error scaling down agents");
            throw;
        }
    }

    public async Task<bool> UpdateAgentReputationAsync(string agentId, double newReputation)
    {
        try
        {
            lock (_lockObject)
            {
                if (!_agents.TryGetValue(agentId, out var agent))
                {
                    return false;
                }

                var updatedAgent = agent with { ReputationScore = Math.Max(0, Math.Min(1, newReputation)) };
                _agents[agentId] = updatedAgent;
            }

            _logger.LogDebug("Updated reputation for agent {AgentId} to {Reputation:F3}", agentId, newReputation);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating reputation for agent {AgentId}", agentId);
            return false;
        }
    }

    public async Task<bool> UpdateAgentStatusAsync(string agentId, bool isActive)
    {
        try
        {
            lock (_lockObject)
            {
                if (!_agents.TryGetValue(agentId, out var agent))
                {
                    return false;
                }

                var updatedAgent = agent with 
                { 
                    IsActive = isActive,
                    LastHeartbeat = isActive ? DateTime.UtcNow : agent.LastHeartbeat
                };
                _agents[agentId] = updatedAgent;
            }

            _logger.LogInformation("Updated status for agent {AgentId} to {Status}", agentId, isActive ? "Active" : "Inactive");
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating status for agent {AgentId}", agentId);
            return false;
        }
    }

    private (AgentRole role, AgentHierarchyLevel level) DetermineOptimalRoleAndLevel()
    {
        lock (_lockObject)
        {
            var totalAgents = _agents.Count;
            
            // Calculate current distribution
            var supervisorCount = _agentsByLevel[AgentHierarchyLevel.Supervisor].Count;
            var workerCount = _agentsByLevel[AgentHierarchyLevel.Worker].Count;
            var executiveCount = _agentsByLevel[AgentHierarchyLevel.Executive].Count;

            // Determine optimal level based on ratios
            var supervisorRatio = totalAgents > 0 ? (double)supervisorCount / totalAgents : 0;
            var executiveRatio = totalAgents > 0 ? (double)executiveCount / totalAgents : 0;

            AgentHierarchyLevel level;
            if (executiveRatio < 0.1 && totalAgents > 5) // Need at least 10% executives
            {
                level = AgentHierarchyLevel.Executive;
            }
            else if (supervisorRatio < 0.25) // Need about 25% supervisors
            {
                level = AgentHierarchyLevel.Supervisor;
            }
            else
            {
                level = AgentHierarchyLevel.Worker;
            }

            // Determine role based on current needs
            var role = DetermineOptimalRole(level);

            return (role, level);
        }
    }

    private AgentRole DetermineOptimalRole(AgentHierarchyLevel level)
    {
        lock (_lockObject)
        {
            // Count current roles
            var roleCounts = new Dictionary<AgentRole, int>();
            foreach (var roleKvp in _agentsByRole)
            {
                roleCounts[roleKvp.Key] = roleKvp.Value.Count;
            }

            // Determine optimal role based on level and current distribution
            return level switch
            {
                AgentHierarchyLevel.Executive => roleCounts[AgentRole.Supervisor] < roleCounts[AgentRole.ReputationManager] 
                    ? AgentRole.Supervisor : AgentRole.ReputationManager,
                
                AgentHierarchyLevel.Supervisor => 
                    roleCounts[AgentRole.Supervisor] <= roleCounts[AgentRole.Validator] 
                        ? AgentRole.Supervisor : AgentRole.Validator,
                
                AgentHierarchyLevel.Worker => 
                    roleCounts.OrderBy(kvp => kvp.Value)
                        .Where(kvp => kvp.Key is AgentRole.Worker or AgentRole.Validator or AgentRole.Security or AgentRole.Monitoring)
                        .FirstOrDefault().Key,
                
                _ => AgentRole.Worker
            };
        }
    }

    private List<string> GetCapabilitiesForRole(AgentRole role)
    {
        return role switch
        {
            AgentRole.Supervisor => new List<string> { "TaskCoordination", "AgentManagement", "DecisionMaking", "ConflictResolution" },
            AgentRole.Validator => new List<string> { "DataValidation", "ConsensusParticipation", "QualityAssurance", "ByzantineDetection" },
            AgentRole.ReputationManager => new List<string> { "ReputationTracking", "TrustManagement", "BehaviorAnalysis", "RiskAssessment" },
            AgentRole.LoadBalancer => new List<string> { "LoadDistribution", "ResourceOptimization", "PerformanceMonitoring" },
            AgentRole.Security => new List<string> { "SecurityValidation", "ThreatDetection", "AccessControl", "AuditLogging" },
            AgentRole.Monitoring => new List<string> { "SystemMonitoring", "MetricsCollection", "AlertGeneration", "HealthChecking" },
            AgentRole.Recovery => new List<string> { "FaultRecovery", "SystemHealing", "BackupManagement", "DisasterRecovery" },
            AgentRole.Worker => new List<string> { "TaskExecution", "DataProcessing", "ProblemSolving", "ResultGeneration" },
            _ => new List<string> { "GeneralProcessing" }
        };
    }

    private string GeneratePublicKey()
    {
        // Simplified public key generation for demo
        // In a real implementation, this would use proper cryptographic key generation
        var random = new Random();
        var keyBytes = new byte[32];
        random.NextBytes(keyBytes);
        return Convert.ToBase64String(keyBytes);
    }
}
