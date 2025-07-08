using ByzantineSwarmDotnet.Models;

namespace ByzantineSwarmDotnet.Interfaces;

public interface IByzantineTaskDistributor
{
    Task<List<AgentTask>> DistributeTaskAsync(SwarmTask swarmTask);
    Task<List<AgentInfo>> SelectCandidateAgentsAsync(SwarmTask task, Dictionary<string, object> taskComplexity);
    Task<List<AgentTask>> CreateRedundantTasksAsync(SwarmTask swarmTask, List<AgentInfo> selectedAgents);
    Task<bool> ValidateTaskDistributionAsync(List<AgentTask> distributedTasks);
}