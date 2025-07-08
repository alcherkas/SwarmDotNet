using ByzantineSwarmDotnet.Models;

namespace ByzantineSwarmDotnet.Interfaces;

// Core Orchestrator Interface
public interface IByzantineTolerantSwarmOrchestrator
{
    Task<SwarmResult> ExecuteTaskAsync(SwarmTask task);
    Task<bool> ValidateByzantineToleranceRequirements();
    Task<SwarmMetrics> GetRealTimeMetricsAsync();
    Task InitiateRecoveryAsync();
    Task QuarantineMaliciousAgentsAsync(List<string> maliciousAgents);
    Task InvestigateConsensusIssuesAsync();
    Task EscalateByzantineDetectionAsync();
}

// Agent Pool Interface
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

// Task Distribution Interface
public interface IByzantineTaskDistributor
{
    Task<List<AgentTask>> DistributeTaskAsync(SwarmTask swarmTask);
    Task<List<AgentInfo>> SelectCandidateAgentsAsync(SwarmTask task, Dictionary<string, object> taskComplexity);
    Task<List<AgentTask>> CreateRedundantTasksAsync(SwarmTask swarmTask, List<AgentInfo> selectedAgents);
    Task<bool> ValidateTaskDistributionAsync(List<AgentTask> distributedTasks);
}

// Communication Interface
public interface IFaultTolerantCommunicationHub
{
    // Basic communication
    Task SendMessageAsync(string fromAgent, string toAgent, SignedMessage message);
    Task BroadcastAsync(string fromAgent, SignedMessage message);
    Task<List<SignedMessage>> GetMessagesAsync(string agentId);
    
    // Byzantine fault tolerant communication
    Task<bool> ReliableMulticastAsync(string fromAgent, SignedMessage message, List<string> recipients);
    Task<ConsensusResult> InitiateConsensusAsync(string initiator, Proposal proposal);
    Task<bool> GossipProtocolAsync(string fromAgent, SignedMessage message);
    
    // Security and verification
    Task<bool> VerifyMessageIntegrityAsync(SignedMessage message);
    Task<bool> AuthenticateAgentAsync(string agentId, AuthenticationToken token);
    
    // Fault detection and recovery
    Task<List<string>> DetectPartitionedAgentsAsync();
    Task InitiateRecoveryProtocolAsync(List<string> faultyAgents);
}

// PBFT Consensus Interface
public interface IPBFTConsensusEngine
{
    Task<ConsensusResult> ReachConsensusAsync(object proposal);
    Task<ConsensusVote> VoteOnProposalAsync(string agentId, Proposal proposal);
    Task<bool> ValidateProposalAsync(Proposal proposal);
    Task<ConsensusResult> ExecuteThreePhaseProtocolAsync(Proposal proposal);
}

// Reputation System Interface
public interface IReputationSystem
{
    Task<double> GetReputationAsync(string agentId);
    Task<bool> UpdateReputationAsync(string agentId, ReputationUpdate update);
    Task<TrustScore> CalculateTrustScoreAsync(string agentId);
    Task<List<string>> GetTrustedAgentsAsync(double minThreshold);
    Task<List<string>> GetSuspiciousAgentsAsync();
    Task<bool> ValidateReputationUpdateAsync(ReputationUpdate update);
}

// Fault Detector Interface
public interface IAdvancedFaultDetector
{
    Task<bool> DetectByzantineFaultAsync(string agentId);
    Task<List<string>> GetFaultyAgentsAsync();
    Task<bool> ValidateAgentBehaviorAsync(string agentId, List<AgentMessage> recentMessages);
    Task<double> CalculateSuspiciousActivityScoreAsync(string agentId);
    Task NotifyFaultDetectedAsync(string agentId, string faultType);
}

// Circuit Breaker Interface
public interface ICircuitBreakerRegistry
{
    ICircuitBreaker GetOrCreate(string key);
    Task<bool> IsOpenAsync(string key);
    Task ResetAsync(string key);
    Task<Dictionary<string, string>> GetAllStatesAsync();
}

public interface ICircuitBreaker
{
    Task<T> ExecuteAsync<T>(Func<Task<T>> operation);
    Task ExecuteAsync(Func<Task> operation);
    bool IsOpen { get; }
    bool IsClosed { get; }
    bool IsHalfOpen { get; }
}

// Cryptographic Services Interface
public interface ICryptographicSigner
{
    Task<DigitalSignature> SignAsync(object message);
    Task<bool> VerifySignatureAsync(object message, DigitalSignature signature);
    Task<string> GenerateKeyPairAsync();
    Task<byte[]> HashAsync(string data);
}

// Byzantine Detector Interface
public interface IByzantineDetector
{
    Task<bool> DetectByzantineBehaviorAsync(AgentMessage message);
    Task<double> CalculateByzantineRiskScoreAsync(string agentId);
    Task<List<string>> IdentifyMaliciousAgentsAsync();
    Task NotifyByzantineAttackAsync(List<string> maliciousAgents);
}

// Gossip Protocol Interface
public interface IGossipProtocol
{
    Task DisseminateAsync(SignedMessage message, List<string> targets);
    Task<bool> ReceiveGossipAsync(SignedMessage message, string fromAgent);
    Task StartGossipRoundAsync();
    Task<List<SignedMessage>> GetUndeliveredMessagesAsync();
}

// Network Partition Detector Interface
public interface INetworkPartitionDetector
{
    Task<List<string>> DetectPartitionedAgentsAsync();
    Task<bool> IsAgentReachableAsync(string agentId);
    Task<Dictionary<string, List<string>>> GetNetworkTopologyAsync();
    Task NotifyPartitionDetectedAsync(List<string> partitionedAgents);
}

// Agent Interface
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

// Specialized Agent Interfaces
public interface ISupervisorAgent : IEnhancedSwarmAgent
{
    Task<List<AgentResponse>> ExecuteSubtasksWithConsensusAsync(List<AgentTask> subtasks);
    Task<AgentTask> DecomposeTaskAsync(SwarmTask task);
    Task<AgentResponse> AggregateResponsesWithValidationAsync(List<AgentResponse> responses);
}

public interface IValidatorAgent : IEnhancedSwarmAgent
{
    Task<bool> ValidateAgentResponseAsync(AgentResponse response);
    Task<bool> ValidateConsensusProposalAsync(Proposal proposal);
    Task<double> CalculateResponseConfidenceAsync(AgentResponse response);
}

public interface IReputationManagerAgent : IEnhancedSwarmAgent
{
    Task ProcessReputationUpdateAsync(ReputationUpdate update);
    Task<bool> ValidateReputationUpdateWithConsensusAsync(ReputationUpdate update);
    Task<Dictionary<string, double>> GetReputationSnapshotAsync();
}

// Storage Interfaces
public interface IDistributedReputationStore
{
    Task<double> GetReputationAsync(string agentId);
    Task UpdateReputationAsync(ReputationUpdate update);
    Task<Dictionary<string, double>> GetAllReputationsAsync();
    Task<List<ReputationUpdate>> GetReputationHistoryAsync(string agentId);
}

public interface IMessageStore
{
    Task StoreMessageAsync(SignedMessage message);
    Task<List<SignedMessage>> GetMessagesAsync(string agentId);
    Task<List<SignedMessage>> GetMessagesByTypeAsync(string messageType);
    Task<SignedMessage?> GetMessageAsync(string messageId);
}

// Monitoring Interfaces
public interface ISwarmMonitor
{
    Task<SwarmMetrics> GetCurrentMetricsAsync();
    Task<List<SwarmMetrics>> GetMetricsHistoryAsync(TimeSpan period);
    Task RecordMetricAsync(string metricName, double value);
    Task<bool> CheckAlertThresholdsAsync();
    Task NotifyAlertAsync(string alertType, string message);
}

public interface IPerformanceMonitor
{
    Task<TimeSpan> GetAverageResponseTimeAsync(string agentId);
    Task<double> GetSuccessRateAsync(string agentId);
    Task<int> GetCurrentLoadAsync(string agentId);
    Task RecordTaskExecutionAsync(string agentId, TimeSpan executionTime, bool success);
}

// Load Balancing Interface
public interface ILoadBalancer
{
    Task<string> SelectOptimalAgentAsync(List<string> candidateAgents, AgentTask task);
    Task<Dictionary<string, int>> GetCurrentLoadDistributionAsync();
    Task RebalanceLoadAsync();
    Task<bool> IsAgentOverloadedAsync(string agentId);
}
