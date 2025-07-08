namespace ByzantineSwarmDotnet.Models;

public record AgentResponse
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string TaskId { get; init; } = string.Empty;
    public string AgentId { get; init; } = string.Empty;
    public bool Success { get; init; }
    public string Result { get; init; } = string.Empty;
    public Dictionary<string, object> Metadata { get; init; } = new();
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public TimeSpan ExecutionTime { get; init; }
    public string ErrorMessage { get; init; } = string.Empty;
    public double ConfidenceScore { get; init; } = 1.0;
    public DigitalSignature? Signature { get; init; }
}