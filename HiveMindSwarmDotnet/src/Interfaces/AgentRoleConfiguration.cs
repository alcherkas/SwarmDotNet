namespace HiveMindSwarmDotnet.Console.Interfaces;

public class AgentRoleConfiguration
{
    public string ModelId { get; set; } = string.Empty;
    public string SystemPrompt { get; set; } = string.Empty;
    public int MaxTokens { get; set; } = 2000;
    public double Temperature { get; set; } = 0.7;
    public Dictionary<string, object> AdditionalParameters { get; set; } = new();
}