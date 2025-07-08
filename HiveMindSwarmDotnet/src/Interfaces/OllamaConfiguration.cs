namespace HiveMindSwarmDotnet.Console.Interfaces;

public class OllamaConfiguration
{
    public string Endpoint { get; set; } = "http://localhost:11434";
    public List<string> Models { get; set; } = new();
    public int TimeoutSeconds { get; set; } = 30;
    public int MaxRetries { get; set; } = 3;
}