using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace HiveMindSwarmDotnet.Console.Agents;

public class RequirementMapperAgent : SwarmAgent
{
    public RequirementMapperAgent(
        Kernel kernel,
        AgentRoleConfiguration configuration,
        ILogger<SwarmAgent> logger)
        : base(AgentRole.RequirementMapper, kernel, configuration, logger)
    {
    }

    protected override async Task<bool> ValidateRoleSpecificResponseAsync(AgentResponse response, CancellationToken cancellationToken)
    {
        var content = response.Content.ToLower();
        
        var hasMappingElements = content.Contains("map") || content.Contains("trace") ||
                                 content.Contains("align") || content.Contains("match") ||
                                 content.Contains("requirement") || content.Contains("criteria");
        
        if (!hasMappingElements)
        {
            response.ValidationErrors.Add("Requirement Mapper response should contain mapping and traceability information");
            return false;
        }

        var hasGapAnalysis = content.Contains("gap") || content.Contains("missing") ||
                             content.Contains("unimplemented") || content.Contains("coverage");
        
        if (hasGapAnalysis)
        {
            response.Confidence = Math.Min(1.0, response.Confidence + 0.15);
        }

        return await Task.FromResult(true);
    }
}