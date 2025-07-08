namespace ByzantineSwarmDotnet.Models;

public record AuthenticationToken
{
    public string Token { get; init; } = string.Empty;
    public string AgentId { get; init; } = string.Empty;
    public DateTime IssuedAt { get; init; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; init; } = DateTime.UtcNow.AddHours(1);
    public List<string> Permissions { get; init; } = new();
}