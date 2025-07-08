namespace HiveMindSwarmDotnet.Console.Models;

public class CommunicationLink
{
    public string FromAgentId { get; set; } = string.Empty;
    public string ToAgentId { get; set; } = string.Empty;
    public double Weight { get; set; } = 1.0;
    public double Probability { get; set; } = 1.0;
    public int MessageCount { get; set; } = 0;
    public DateTime LastUsed { get; set; } = DateTime.UtcNow;
}