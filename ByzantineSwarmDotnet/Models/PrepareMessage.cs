namespace ByzantineSwarmDotnet.Models;

public record PrepareMessage
{
    public string ProposalId { get; init; } = string.Empty;
    public long SequenceNumber { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
}