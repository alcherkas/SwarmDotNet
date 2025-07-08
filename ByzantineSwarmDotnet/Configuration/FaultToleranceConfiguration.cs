namespace ByzantineSwarmDotnet.Configuration;

public class FaultToleranceConfiguration
{
    public int CircuitBreakerThreshold { get; set; } = 5;
    public int CircuitBreakerTimeout { get; set; } = 60000;
    public int MaxRetryAttempts { get; set; } = 3;
    public double RetryBackoffMultiplier { get; set; } = 2.0;
    public int HealthCheckInterval { get; set; } = 10000;
    public bool EnableBulkheadIsolation { get; set; } = true;
    public int MaxConcurrentOperations { get; set; } = 100;
}