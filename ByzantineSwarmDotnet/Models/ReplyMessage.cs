namespace ByzantineSwarmDotnet.Models;

public record ReplyMessage
{
    public string ProposalId { get; init; } = string.Empty;
    public bool Committed { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}