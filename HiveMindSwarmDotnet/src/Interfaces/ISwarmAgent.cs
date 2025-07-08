using HiveMindSwarmDotnet.Console.Models;

namespace HiveMindSwarmDotnet.Console.Interfaces;

public interface ISwarmAgent
{
    string Id { get; }
    AgentRole Role { get; }
    bool IsAvailable { get; }
    Task<AgentResponse> ProcessTaskAsync(AgentTask task, CancellationToken cancellationToken = default);
    Task<bool> ValidateResponseAsync(AgentResponse response, CancellationToken cancellationToken = default);
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task DisposeAsync();
}