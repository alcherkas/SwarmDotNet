using HiveMindSwarmDotnet.Console.Models;

namespace HiveMindSwarmDotnet.Console.Choreography;

public interface IChoreographyCoordinator
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task<SwarmResult> ExecuteTaskAsync(SwarmTask task, CancellationToken cancellationToken = default);
    Task<SwarmMetrics> GetMetricsAsync();
    Task DisposeAsync();
}

public interface IChoreographyConfiguration
{
    int MaxConcurrentTasks { get; }
    TimeSpan TaskTimeout { get; }
    int MaxRetries { get; }
    double MinConfidenceThreshold { get; }
    bool EnableAutoCollaboration { get; }
    bool EnableSelfOrganization { get; }
}