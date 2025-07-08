namespace HiveMindSwarmDotnet.Console.Models;

public class AgentTask
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SwarmTaskId { get; set; } = string.Empty;
    public string AgentId { get; set; } = string.Empty;
    public AgentRole Role { get; set; }
    public string Prompt { get; set; } = string.Empty;
    public Dictionary<string, object> Context { get; set; } = new();
    public List<string> Dependencies { get; set; } = new();
    public SwarmTaskStatus Status { get; set; } = SwarmTaskStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}