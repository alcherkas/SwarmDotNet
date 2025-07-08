namespace ByzantineSwarmDotnet.Configuration;

public class LoadBalancingConfiguration
{
    public string Strategy { get; set; } = "RoundRobin";
    public bool EnableFailover { get; set; } = true;
    public string HealthCheckEndpoint { get; set; } = "/api/health";
}