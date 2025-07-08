using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace HiveMindSwarmDotnet.Console.Agents;

public class PRExtractorAgent : SwarmAgent
{
    public PRExtractorAgent(
        Kernel kernel,
        AgentRoleConfiguration configuration,
        ILogger<SwarmAgent> logger)
        : base(AgentRole.PRExtractor, kernel, configuration, logger)
    {
    }

    protected override async Task<bool> ValidateRoleSpecificResponseAsync(AgentResponse response, CancellationToken cancellationToken)
    {
        var content = response.Content;
        
        var hasPRElements = content.Contains("PR") || content.Contains("pull request") ||
                            content.Contains("diff") || content.Contains("commit") ||
                            content.Contains("file") || content.Contains("change");
        
        if (!hasPRElements)
        {
            response.ValidationErrors.Add("PR Extractor response should contain pull request related information");
            return false;
        }

        var hasStructuredData = content.Contains("{") || content.Contains("title:") ||
                                content.Contains("files:") || content.Contains("jira");
        
        if (hasStructuredData)
        {
            response.Confidence = Math.Min(1.0, response.Confidence + 0.2);
        }

        return await Task.FromResult(true);
    }
}