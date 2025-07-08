namespace ByzantineSwarmDotnet.Models;

public record ReputationUpdate
{
    public string AgentId { get; init; } = string.Empty;
    public string UpdatedBy { get; init; } = string.Empty;
    public double ScoreChange { get; init; }
    public string Reason { get; init; } = string.Empty;
    public DateTime UpdatedAt { get; init; } = DateTime.UtcNow;
    public Dictionary<string, object> Evidence { get; init; } = new();
}