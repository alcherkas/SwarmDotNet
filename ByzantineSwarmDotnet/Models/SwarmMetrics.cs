namespace ByzantineSwarmDotnet.Models;

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