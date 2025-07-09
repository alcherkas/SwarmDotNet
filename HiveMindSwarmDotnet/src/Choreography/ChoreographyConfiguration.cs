namespace HiveMindSwarmDotnet.Console.Choreography;

public class ChoreographyConfiguration : IChoreographyConfiguration
{
    public int MaxConcurrentTasks { get; set; } = 10;
    public TimeSpan TaskTimeout { get; set; } = TimeSpan.FromMinutes(5);
    public int MaxRetries { get; set; } = 3;
    public double MinConfidenceThreshold { get; set; } = 0.6;
    public bool EnableAutoCollaboration { get; set; } = true;
    public bool EnableSelfOrganization { get; set; } = true;
}