using HiveMindSwarmDotnet.Console.Models;

namespace HiveMindSwarmDotnet.Console.Interfaces;

public interface ISwarmConfiguration
{
    int MaxAgents { get; }
    string DefaultModel { get; }
    OptimizationStrategy OptimizationStrategy { get; }
    int CommunicationTimeout { get; }
    int TaskTimeout { get; }
    bool EnableRiskAssessmentAgent { get; }
    Dictionary<AgentRole, AgentRoleConfiguration> AgentRoles { get; }
    OllamaConfiguration Ollama { get; }
}