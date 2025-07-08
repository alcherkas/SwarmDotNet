namespace ByzantineSwarmDotnet.Models;

public record SignedMessage
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string SenderId { get; init; } = string.Empty;
    public string ReceiverId { get; init; } = string.Empty;
    public string MessageType { get; init; } = string.Empty;
    public string Content { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public DigitalSignature Signature { get; init; } = new();
    public Dictionary<string, string> Headers { get; init; } = new();
}