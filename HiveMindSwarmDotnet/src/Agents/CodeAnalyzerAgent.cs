using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace HiveMindSwarmDotnet.Console.Agents;

public class CodeAnalyzerAgent : SwarmAgent
{
    public CodeAnalyzerAgent(
        Kernel kernel,
        AgentRoleConfiguration configuration,
        ILogger<SwarmAgent> logger)
        : base(AgentRole.CodeAnalyzer, kernel, configuration, logger)
    {
    }

    protected override async Task<bool> ValidateRoleSpecificResponseAsync(AgentResponse response, CancellationToken cancellationToken)
    {
        var content = response.Content.ToLower();
        
        var hasCodeElements = content.Contains("code") || content.Contains("function") ||
                              content.Contains("class") || content.Contains("method") ||
                              content.Contains("api") || content.Contains("pattern");
        
        if (!hasCodeElements)
        {
            response.ValidationErrors.Add("Code Analyzer response should contain technical implementation details");
            return false;
        }

        var hasTechnicalDepth = content.Contains("complexity") || content.Contains("architecture") ||
                                content.Contains("performance") || content.Contains("quality");
        
        if (hasTechnicalDepth)
        {
            response.Confidence = Math.Min(1.0, response.Confidence + 0.2);
        }

        return await Task.FromResult(true);
    }
}