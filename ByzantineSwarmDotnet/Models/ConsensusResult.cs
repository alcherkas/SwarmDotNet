namespace ByzantineSwarmDotnet.Models;

public record ConsensusResult
{
    public string ProposalId { get; init; } = string.Empty;
    public bool IsAccepted { get; init; }
    public bool ConsensusReached { get; init; }
    public bool SafetyMaintained { get; init; }
    public int TotalVotes { get; init; }
    public int AcceptVotes { get; init; }
    public int RejectVotes { get; init; }
    public int HonestParticipants { get; init; }
    public List<string> ParticipantIds { get; init; } = new();
    public DateTime CompletedAt { get; init; } = DateTime.UtcNow;
    public TimeSpan Duration { get; init; }
    
    public static ConsensusResult Success() => new() { IsAccepted = true, ConsensusReached = true, SafetyMaintained = true };
    public static ConsensusResult Failed() => new() { IsAccepted = false, ConsensusReached = false, SafetyMaintained = false };
}