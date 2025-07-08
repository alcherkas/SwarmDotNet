namespace ByzantineSwarmDotnet.Configuration;

public class ReputationSystemConfiguration
{
    public bool EnableReputationTracking { get; set; } = true;
    public double InitialReputation { get; set; } = 0.5;
    public double MinTrustThreshold { get; set; } = 0.3;
    public double ReputationDecayRate { get; set; } = 0.99;
    public double MaliciousDetectionThreshold { get; set; } = 0.2;
    public string ReputationAggregationMethod { get; set; } = "WeightedAverage";
}