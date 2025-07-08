using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Configuration;

namespace HiveMindSwarmDotnet.Console.Configuration;

public class SwarmConfiguration : ISwarmConfiguration
{
    public int MaxAgents { get; set; } = 10;
    public string DefaultModel { get; set; } = "gemma3:4b-it-q8_0";
    public OptimizationStrategy OptimizationStrategy { get; set; } = OptimizationStrategy.Lamarckian;
    public int CommunicationTimeout { get; set; } = 30000;
    public int TaskTimeout { get; set; } = 300000;
    public bool EnableRiskAssessmentAgent { get; set; } = true;
    public Dictionary<AgentRole, AgentRoleConfiguration> AgentRoles { get; set; } = new();
    public OllamaConfiguration Ollama { get; set; } = new();

    public static SwarmConfiguration LoadFromConfiguration(IConfiguration configuration)
    {
        var swarmConfig = new SwarmConfiguration();
        
        var swarmSection = configuration.GetSection("SwarmConfiguration");
        if (swarmSection.Exists())
        {
            swarmConfig.MaxAgents = swarmSection.GetValue<int>("MaxAgents", 10);
            swarmConfig.DefaultModel = swarmSection.GetValue<string>("DefaultModel") ?? "gemma3:4b-it-q8_0";
            swarmConfig.OptimizationStrategy = Enum.Parse<OptimizationStrategy>(
                swarmSection.GetValue<string>("OptimizationStrategy") ?? "Lamarckian");
            swarmConfig.CommunicationTimeout = swarmSection.GetValue<int>("CommunicationTimeout", 30000);
            swarmConfig.TaskTimeout = swarmSection.GetValue<int>("TaskTimeout", 300000);
            swarmConfig.EnableRiskAssessmentAgent = swarmSection.GetValue<bool>("EnableRiskAssessmentAgent", true);
        }

        var ollamaSection = configuration.GetSection("Ollama");
        if (ollamaSection.Exists())
        {
            swarmConfig.Ollama = new OllamaConfiguration
            {
                Endpoint = ollamaSection.GetValue<string>("Endpoint") ?? "http://localhost:11434",
                Models = ollamaSection.GetSection("Models").Get<List<string>>() ?? new List<string>(),
                TimeoutSeconds = ollamaSection.GetValue<int>("TimeoutSeconds", 30),
                MaxRetries = ollamaSection.GetValue<int>("MaxRetries", 3)
            };
        }

        var agentRolesSection = configuration.GetSection("AgentRoles");
        if (agentRolesSection.Exists())
        {
            foreach (var roleSection in agentRolesSection.GetChildren())
            {
                if (Enum.TryParse<AgentRole>(roleSection.Key, out var role))
                {
                    swarmConfig.AgentRoles[role] = new AgentRoleConfiguration
                    {
                        ModelId = roleSection.GetValue<string>("ModelId") ?? swarmConfig.DefaultModel,
                        SystemPrompt = roleSection.GetValue<string>("SystemPrompt") ?? string.Empty,
                        MaxTokens = roleSection.GetValue<int>("MaxTokens", 2000),
                        Temperature = roleSection.GetValue<double>("Temperature", 0.7)
                    };
                }
            }
        }

        return swarmConfig;
    }
}