using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace HiveMindSwarmDotnet.Console.Agents;

public class SummaryGeneratorAgent : SwarmAgent
{
    public SummaryGeneratorAgent(
        Kernel kernel,
        AgentRoleConfiguration configuration,
        ILogger<SwarmAgent> logger)
        : base(AgentRole.SummaryGenerator, kernel, configuration, logger)
    {
    }

    protected override async Task<bool> ValidateRoleSpecificResponseAsync(AgentResponse response, CancellationToken cancellationToken)
    {
        var content = response.Content.ToLower();
        
        var hasSummaryElements = content.Contains("summary") || content.Contains("overview") ||
                                 content.Contains("synthesis") || content.Contains("conclusion") ||
                                 content.Contains("recommendation") || content.Contains("findings");
        
        if (!hasSummaryElements)
        {
            response.ValidationErrors.Add("Summary Generator response should contain synthesis and summary language");
            return false;
        }

        if (content.Length < 100)
        {
            response.ValidationErrors.Add("Summary should be comprehensive and detailed");
            return false;
        }

        var hasStructure = content.Contains("1.") || content.Contains("•") || 
                           content.Contains("-") || content.Contains("summary:") ||
                           content.Contains("recommendations:");
        
        if (hasStructure)
        {
            response.Confidence = Math.Min(1.0, response.Confidence + 0.1);
        }

        return await Task.FromResult(true);
    }
}