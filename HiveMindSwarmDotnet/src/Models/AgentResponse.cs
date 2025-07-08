namespace HiveMindSwarmDotnet.Console.Models;

public class AgentResponse
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TaskId { get; set; } = string.Empty;
    public string AgentId { get; set; } = string.Empty;
    public AgentRole Role { get; set; }
    public string Content { get; set; } = string.Empty;
    public double Confidence { get; set; } = 0.0;
    public Dictionary<string, object> Metadata { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsValid { get; set; } = true;
    public List<string> ValidationErrors { get; set; } = new();
}