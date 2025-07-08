namespace ByzantineSwarmDotnet.Examples.Models;

// Simplified models for examples - just enough to demonstrate functionality

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

public class ByzantineAttackDetectedException : Exception
{
    public List<string> MaliciousAgents { get; }
    
    public ByzantineAttackDetectedException(List<string> maliciousAgents, string message) : base(message)
    {
        MaliciousAgents = maliciousAgents;
    }
}