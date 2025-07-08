using HiveMindSwarmDotnet.Console.Models;

namespace HiveMindSwarmDotnet.Console.Interfaces;

public interface ISwarmOrchestrator
{
    Task<SwarmResult> ExecuteTaskAsync(SwarmTask task, CancellationToken cancellationToken = default);
    Task<SwarmMetrics> GetMetricsAsync();
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task DisposeAsync();
}