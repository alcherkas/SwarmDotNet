using ByzantineSwarmDotnet.Models;

namespace ByzantineSwarmDotnet.Interfaces;

public interface IHierarchicalAgentPool
{
    Task<List<AgentInfo>> GetActiveAgentsAsync();
    Task<List<AgentInfo>> GetAgentsByRoleAsync(AgentRole role);
    Task<List<AgentInfo>> GetAgentsByHierarchyLevelAsync(AgentHierarchyLevel level);
    Task<AgentInfo?> GetAgentAsync(string agentId);
    Task<string> RegisterAgentAsync(AgentInfo agent);
    Task<bool> DeregisterAgentAsync(string agentId);
    Task ScaleUpAsync(int additionalAgents);
    Task ScaleDownAsync(int agentsToRemove);
    Task<bool> UpdateAgentReputationAsync(string agentId, double newReputation);
    Task<bool> UpdateAgentStatusAsync(string agentId, bool isActive);
}