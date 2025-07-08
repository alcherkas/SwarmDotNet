namespace HiveMindSwarmDotnet.Console.Models;

public class SwarmTask
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Description { get; set; } = string.Empty;
    public AgentRole[] RequiredRoles { get; set; } = Array.Empty<AgentRole>();
    public TimeSpan MaxExecutionTime { get; set; } = TimeSpan.FromMinutes(5);
    public Dictionary<string, object> Parameters { get; set; } = new();
    public int Priority { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}