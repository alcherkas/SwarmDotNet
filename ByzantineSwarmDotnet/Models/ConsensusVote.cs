namespace ByzantineSwarmDotnet.Models;

public record ConsensusVote
{
    public string VoterId { get; init; } = string.Empty;
    public string ProposalId { get; init; } = string.Empty;
    public bool Accept { get; init; }
    public string Justification { get; init; } = string.Empty;
    public DateTime VotedAt { get; init; } = DateTime.UtcNow;
    public DigitalSignature Signature { get; init; } = new();
}