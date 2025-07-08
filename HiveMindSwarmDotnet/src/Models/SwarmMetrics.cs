namespace HiveMindSwarmDotnet.Console.Models;

public class SwarmMetrics
{
    public string SwarmId { get; set; } = string.Empty;
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int FailedTasks { get; set; }
    public double AverageExecutionTime { get; set; }
    public double AverageConfidence { get; set; }
    public int TotalCommunications { get; set; }
    public Dictionary<AgentRole, int> AgentUtilization { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}