namespace ByzantineSwarmDotnet.Models;

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