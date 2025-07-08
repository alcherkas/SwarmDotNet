using HiveMindSwarmDotnet.Console.Models;

namespace HiveMindSwarmDotnet.Console.Interfaces;

public interface ITaskDistributor
{
    Task<List<AgentTask>> DistributeTaskAsync(SwarmTask swarmTask, CancellationToken cancellationToken = default);
    Task<AgentTask> GetNextTaskAsync(string agentId, CancellationToken cancellationToken = default);
    Task UpdateTaskStatusAsync(string taskId, SwarmTaskStatus status, CancellationToken cancellationToken = default);
}