namespace ByzantineSwarmDotnet.Examples.Models;

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