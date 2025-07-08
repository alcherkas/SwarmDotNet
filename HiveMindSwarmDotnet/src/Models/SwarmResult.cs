namespace HiveMindSwarmDotnet.Console.Models;

public class SwarmResult
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TaskId { get; set; } = string.Empty;
    public string FinalAnswer { get; set; } = string.Empty;
    public List<AgentResponse> AgentResponses { get; set; } = new();
    public List<AgentMessage> Communications { get; set; } = new();
    public double OverallConfidence { get; set; } = 0.0;
    public TimeSpan ExecutionTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsSuccessful { get; set; } = true;
    public List<string> Errors { get; set; } = new();
    public Dictionary<string, object> Metrics { get; set; } = new();
}