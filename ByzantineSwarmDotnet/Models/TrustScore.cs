namespace ByzantineSwarmDotnet.Models;

public record TrustScore
{
    public string AgentId { get; init; } = string.Empty;
    public double Score { get; init; }
    public bool IsTrusted { get; init; }
    public DateTime LastUpdated { get; init; } = DateTime.UtcNow;
    public List<string> TrustFactors { get; init; } = new();
}