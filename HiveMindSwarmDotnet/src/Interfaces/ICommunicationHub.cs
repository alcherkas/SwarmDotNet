using HiveMindSwarmDotnet.Console.Models;

namespace HiveMindSwarmDotnet.Console.Interfaces;

public interface ICommunicationHub
{
    Task SendMessageAsync(AgentMessage message, CancellationToken cancellationToken = default);
    Task BroadcastAsync(string fromAgentId, AgentMessage message, CancellationToken cancellationToken = default);
    Task<List<AgentMessage>> GetMessagesAsync(string agentId, CancellationToken cancellationToken = default);
    Task<List<CommunicationLink>> GetCommunicationLinksAsync(string agentId, CancellationToken cancellationToken = default);
    Task OptimizeCommunicationAsync(CancellationToken cancellationToken = default);
    Task RegisterAgentAsync(string agentId, CancellationToken cancellationToken = default);
    Task UnregisterAgentAsync(string agentId, CancellationToken cancellationToken = default);
}