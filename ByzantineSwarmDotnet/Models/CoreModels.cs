using System.Security.Cryptography;

namespace ByzantineSwarmDotnet.Models;

// Enums
public enum AgentRole
{
    Supervisor,
    Validator,
    ReputationManager,
    LoadBalancer,
    Security,
    Monitoring,
    Recovery,
    Worker
}

public enum AgentHierarchyLevel
{
    Executive = 1,
    Supervisor = 2,
    Worker = 3
}

public enum TaskType
{
    General,
    ReputationUpdate,
    SecurityValidation,
    ConsensusParticipation,
    PerformanceMonitoring,
    FaultRecovery
}

public enum ProposalType
{
    TaskExecution,
    AgentRegistration,
    AgentDeregistration,
    ConfigurationUpdate,
    ReputationUpdate,
    SecurityUpdate
}

public enum FaultToleranceLevel
{
    Basic,
    Byzantine,
    Quantum
}

public enum SecurityLevel
{
    Low,
    Medium,
    High,
    Critical
}

public enum OptimizationStrategy
{
    Darwinian,
    Lamarckian,
    HybridLamarckian
}

// Core Data Models
public record AgentInfo
{
    public string Id { get; init; } = string.Empty;
    public AgentRole Role { get; init; }
    public AgentHierarchyLevel HierarchyLevel { get; init; }
    public double ReputationScore { get; init; }
    public List<string> Capabilities { get; init; } = new();
    public DateTime LastHeartbeat { get; init; }
    public bool IsActive { get; init; }
    public string PublicKey { get; init; } = string.Empty;
    public int CurrentLoad { get; init; }
    public TimeSpan AverageResponseTime { get; init; }
}

public record SwarmTask
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Description { get; init; } = string.Empty;
    public string[] RequiredRoles { get; init; } = Array.Empty<string>();
    public TimeSpan MaxExecutionTime { get; init; } = TimeSpan.FromMinutes(5);
    public bool RequiredConsensus { get; init; }
    public FaultToleranceLevel FaultToleranceLevel { get; init; } = FaultToleranceLevel.Basic;
    public SecurityLevel SecurityLevel { get; init; } = SecurityLevel.Medium;
    public Dictionary<string, object> Parameters { get; init; } = new();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public string CreatedBy { get; init; } = string.Empty;
}

public record AgentTask
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string SwarmTaskId { get; init; } = string.Empty;
    public string AssignedAgentId { get; init; } = string.Empty;
    public TaskType Type { get; init; }
    public string Description { get; init; } = string.Empty;
    public Dictionary<string, object> Parameters { get; init; } = new();
    public List<string> Dependencies { get; init; } = new();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public bool IsCompleted { get; init; }
    public bool IsSuccessful { get; init; }
    
    public T GetPayload<T>() where T : class
    {
        if (Parameters.TryGetValue("payload", out var payload) && payload is T typedPayload)
        {
            return typedPayload;
        }
        throw new InvalidOperationException($"Payload of type {typeof(T).Name} not found");
    }
}

public record AgentResponse
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string TaskId { get; init; } = string.Empty;
    public string AgentId { get; init; } = string.Empty;
    public bool Success { get; init; }
    public string Result { get; init; } = string.Empty;
    public Dictionary<string, object> Metadata { get; init; } = new();
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public TimeSpan ExecutionTime { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;
    public double ConfidenceScore { get; init; } = 1.0;
    public DigitalSignature? Signature { get; init; }
}

public record SwarmResult
{
    public string TaskId { get; init; } = string.Empty;
    public bool Success { get; init; }
    public bool ConsensusReached { get; init; }
    public double TrustScore { get; init; }
    public int ParticipatingAgents { get; init; }
    public double ConsensusConfidence { get; init; }
    public double AverageReputationScore { get; init; }
    public int ByzantineFaultsDetected { get; init; }
    public string Result { get; init; } = string.Empty;
    public Dictionary<string, object> Metadata { get; init; } = new();
    public DateTime CompletedAt { get; init; } = DateTime.UtcNow;
    public TimeSpan TotalExecutionTime { get; init; }
    public List<string> Errors { get; init; } = new();
    public List<string> Warnings { get; init; } = new();
}

// Communication Models
public record SignedMessage
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string SenderId { get; init; } = string.Empty;
    public string ReceiverId { get; init; } = string.Empty;
    public string MessageType { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public DigitalSignature Signature { get; init; } = new();
    public Dictionary<string, string> Headers { get; init; } = new();
}

public record AgentMessage : SignedMessage
{
    public AgentTask? Task { get; init; }
    public AgentResponse? Response { get; init; }
    public string Priority { get; init; } = "Normal";
}

public record DigitalSignature
{
    public string Algorithm { get; init; } = "ECDSA-P256";
    public byte[] SignatureData { get; init; } = Array.Empty<byte>();
    public string PublicKey { get; init; } = string.Empty;
    public DateTime SignedAt { get; init; } = DateTime.UtcNow;
}

// Consensus Models
public record Proposal
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public ProposalType Type { get; init; }
    public string ProposerId { get; init; } = string.Empty;
    public long SequenceNumber { get; init; }
    public Dictionary<string, object> Data { get; init; } = new();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; init; } = DateTime.UtcNow.AddMinutes(5);
}

public record ConsensusVote
{
    public string VoterId { get; init; } = string.Empty;
    public string ProposalId { get; init; } = string.Empty;
    public bool Accept { get; init; }
    public string Justification { get; init; } = string.Empty;
    public DateTime VotedAt { get; init; } = DateTime.UtcNow;
    public DigitalSignature Signature { get; init; } = new();
}

public record ConsensusResult
{
    public string ProposalId { get; init; } = string.Empty;
    public bool IsAccepted { get; init; }
    public bool ConsensusReached { get; init; }
    public bool SafetyMaintained { get; init; }
    public int TotalVotes { get; init; }
    public int AcceptVotes { get; init; }
    public int RejectVotes { get; init; }
    public int HonestParticipants { get; init; }
    public List<string> ParticipantIds { get; init; } = new();
    public DateTime CompletedAt { get; init; } = DateTime.UtcNow;
    public TimeSpan Duration { get; init; }
    
    public static ConsensusResult Success() => new() { IsAccepted = true, ConsensusReached = true, SafetyMaintained = true };
    public static ConsensusResult Failed() => new() { IsAccepted = false, ConsensusReached = false, SafetyMaintained = false };
}

// Reputation Models
public record ReputationUpdate
{
    public string AgentId { get; init; } = string.Empty;
    public string UpdatedBy { get; init; } = string.Empty;
    public double ScoreChange { get; init; }
    public string Reason { get; init; } = string.Empty;
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
    public Dictionary<string, object> Evidence { get; init; } = new();
}

public record TrustScore
{
    public string AgentId { get; init; } = string.Empty;
    public double Score { get; init; }
    public bool IsTrusted { get; init; }
    public DateTime LastUpdated { get; init; } = DateTime.UtcNow;
    public List<string> TrustFactors { get; init; } = new();
}

// Message Types for PBFT
public record PrepareMessage
{
    public string ProposalId { get; init; } = string.Empty;
    public long SequenceNumber { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record CommitMessage
{
    public string ProposalId { get; init; } = string.Empty;
    public long SequenceNumber { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

public record ReplyMessage
{
    public string ProposalId { get; init; } = string.Empty;
    public bool Committed { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}

// Authentication Models
public record AuthenticationToken
{
    public string Token { get; init; } = string.Empty;
    public string AgentId { get; init; } = string.Empty;
    public DateTime IssuedAt { get; init; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; init; } = DateTime.UtcNow.AddHours(1);
    public List<string> Permissions { get; init; } = new();
}

// Task Distribution Models
public record TaskDistributionProposal
{
    public List<AgentTask> Tasks { get; init; } = new();
    public Dictionary<string, object> DistributionStrategy { get; init; } = new();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}

// Exception Models
public class ConsensusFailedException : Exception
{
    public ConsensusFailedException(string message) : base(message) { }
    public ConsensusFailedException(string message, Exception innerException) : base(message, innerException) { }
}

public class TaskDistributionFailedException : Exception
{
    public TaskDistributionFailedException(string message) : base(message) { }
    public TaskDistributionFailedException(string message, Exception innerException) : base(message, innerException) { }
}

public class ByzantineAttackDetectedException : Exception
{
    public List<string> MaliciousAgents { get; }
    
    public ByzantineAttackDetectedException(List<string> maliciousAgents, string message) : base(message)
    {
        MaliciousAgents = maliciousAgents;
    }
}

// Metrics Models
public record SwarmMetrics
{
    public double ConsensusParticipation { get; init; }
    public double SuspiciousActivityScore { get; init; }
    public int ActiveAgents { get; init; }
    public double AverageResponseTime { get; init; }
    public int TasksCompleted { get; init; }
    public int TasksFailed { get; init; }
    public double SystemLoad { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}
