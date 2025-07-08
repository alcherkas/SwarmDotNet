using HiveMindSwarmDotnet.Console.Models;

namespace HiveMindSwarmDotnet.Console.Interfaces;

public interface IAgentPool
{
    Task<ISwarmAgent> GetAgentAsync(AgentRole role, CancellationToken cancellationToken = default);
    Task ReturnAgentAsync(ISwarmAgent agent, CancellationToken cancellationToken = default);
    Task<IEnumerable<ISwarmAgent>> GetAvailableAgentsAsync(CancellationToken cancellationToken = default);
    Task<SwarmMetrics> GetPoolMetricsAsync();
    Task InitializePoolAsync(CancellationToken cancellationToken = default);
    Task DisposeAsync();
}