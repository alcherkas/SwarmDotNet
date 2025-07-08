namespace ByzantineSwarmDotnet.Configuration;

public class AlertThresholdsConfiguration
{
    public int HighLatency { get; set; } = 5000;
    public double HighErrorRate { get; set; } = 0.05;
    public double LowConsensusParticipation { get; set; } = 0.7;
}