namespace HiveMindSwarmDotnet.Console.Models;

public class AgentMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FromAgentId { get; set; } = string.Empty;
    public string ToAgentId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public MessageType Type { get; set; } = MessageType.Information;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Metadata { get; set; } = new();
}