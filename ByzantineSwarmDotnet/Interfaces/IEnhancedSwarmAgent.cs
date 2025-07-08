using ByzantineSwarmDotnet.Models;

namespace ByzantineSwarmDotnet.Interfaces;

public interface IEnhancedSwarmAgent
{
    string Id { get; }
    AgentRole Role { get; }
    AgentHierarchyLevel HierarchyLevel { get; }
    double ReputationScore { get; }
    bool IsActive { get; }
    
    // Core agent capabilities
    Task<AgentResponse> ProcessTaskAsync(AgentTask task);
    Task<bool> ValidateResponseAsync(AgentResponse response);
    
    // Byzantine fault tolerance capabilities
    Task<ConsensusVote> VoteOnProposalAsync(Proposal proposal);
    Task<bool> DetectByzantineBehaviorAsync(AgentMessage message);
    Task<DigitalSignature> SignMessageAsync(AgentMessage message);
    Task<bool> VerifyMessageSignatureAsync(AgentMessage message);
    
    // Reputation and trust management
    Task UpdateReputationAsync(ReputationUpdate update);
    Task<TrustScore> CalculateTrustScoreAsync(string targetAgentId);
    
    // Hierarchical coordination
    Task DelegateTaskAsync(AgentTask task, List<string> subordinateIds);
    Task ReportToSupervisorAsync(AgentResponse result);
    
    // Health and monitoring
    Task<bool> PerformHealthCheckAsync();
    Task SendHeartbeatAsync();
    Task<Dictionary<string, object>> GetMetricsAsync();
}