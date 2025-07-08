using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace HiveMindSwarmDotnet.Console.Agents;

public class IntegrationAnalyzerAgent : SwarmAgent
{
    public IntegrationAnalyzerAgent(
        Kernel kernel,
        AgentRoleConfiguration configuration,
        ILogger<SwarmAgent> logger)
        : base(AgentRole.IntegrationAnalyzer, kernel, configuration, logger)
    {
    }

    protected override async Task<bool> ValidateRoleSpecificResponseAsync(AgentResponse response, CancellationToken cancellationToken)
    {
        var content = response.Content.ToLower();
        
        var hasIntegrationElements = content.Contains("integration") || content.Contains("api") ||
                                     content.Contains("service") || content.Contains("dependency") ||
                                     content.Contains("contract") || content.Contains("interface");
        
        if (!hasIntegrationElements)
        {
            response.ValidationErrors.Add("Integration Analyzer response should contain system integration information");
            return false;
        }

        var hasSystemMapping = content.Contains("downstream") || content.Contains("upstream") ||
                               content.Contains("mesh") || content.Contains("endpoint") ||
                               content.Contains("contract");
        
        if (hasSystemMapping)
        {
            response.Confidence = Math.Min(1.0, response.Confidence + 0.15);
        }

        return await Task.FromResult(true);
    }
}