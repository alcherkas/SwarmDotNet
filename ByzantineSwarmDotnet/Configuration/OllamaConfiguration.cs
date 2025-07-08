namespace ByzantineSwarmDotnet.Configuration;

public class OllamaConfiguration
{
    public string Endpoint { get; set; } = "http://localhost:11434";
    public List<string> Models { get; set; } = new();
    public ConnectionPoolConfiguration ConnectionPool { get; set; } = new();
    public LoadBalancingConfiguration LoadBalancing { get; set; } = new();
}