namespace ByzantineSwarmDotnet.Models;

public record TaskDistributionProposal
{
    public List<AgentTask> Tasks { get; init; } = new();
    public Dictionary<string, object> DistributionStrategy { get; init; } = new();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}