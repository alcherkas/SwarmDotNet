namespace ByzantineSwarmDotnet.Configuration;

public class ConnectionPoolConfiguration
{
    public int MaxConnections { get; set; } = 20;
    public int ConnectionTimeout { get; set; } = 10000;
    public int RetryAttempts { get; set; } = 3;
    public int HealthCheckInterval { get; set; } = 30000;
}