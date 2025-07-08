using HiveMindSwarmDotnet.Console.Models;

namespace HiveMindSwarmDotnet.Console.Interfaces;

public interface IOptimizationEngine
{
    Task<List<CommunicationLink>> OptimizeCommunicationLinksAsync(
        List<CommunicationLink> links, 
        SwarmMetrics metrics,
        CancellationToken cancellationToken = default);
    
    Task<Dictionary<AgentRole, double>> CalculateAgentPerformanceAsync(
        List<AgentResponse> responses,
        CancellationToken cancellationToken = default);
    
    Task AdaptAgentBehaviorAsync(
        string agentId, 
        Dictionary<string, object> performanceData,
        CancellationToken cancellationToken = default);
}