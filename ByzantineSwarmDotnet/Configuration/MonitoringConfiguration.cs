namespace ByzantineSwarmDotnet.Configuration;

public class MonitoringConfiguration
{
    public bool EnableDistributedTracing { get; set; } = true;
    public int MetricsCollectionInterval { get; set; } = 5000;
    public bool EnableAnomalyDetection { get; set; } = true;
    public AlertThresholdsConfiguration AlertThresholds { get; set; } = new();
}