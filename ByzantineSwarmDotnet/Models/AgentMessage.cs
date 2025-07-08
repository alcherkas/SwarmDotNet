namespace ByzantineSwarmDotnet.Models;

public record AgentMessage : SignedMessage
{
    public AgentTask? Task { get; init; }
    public AgentResponse? Response { get; init; }
    public string Priority { get; init; } = "Normal";
}