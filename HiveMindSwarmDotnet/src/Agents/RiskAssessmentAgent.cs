using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace HiveMindSwarmDotnet.Console.Agents;

public class RiskAssessmentAgent : SwarmAgent
{
    public RiskAssessmentAgent(
        Kernel kernel,
        AgentRoleConfiguration configuration,
        ILogger<SwarmAgent> logger)
        : base(AgentRole.RiskAssessment, kernel, configuration, logger)
    {
    }

    protected override async Task<bool> ValidateRoleSpecificResponseAsync(AgentResponse response, CancellationToken cancellationToken)
    {
        var content = response.Content.ToLower();
        
        var hasRiskElements = content.Contains("risk") || content.Contains("breaking") ||
                              content.Contains("impact") || content.Contains("vulnerability") ||
                              content.Contains("security") || content.Contains("performance");
        
        if (!hasRiskElements)
        {
            response.ValidationErrors.Add("Risk Assessment response should contain risk and impact analysis");
            return false;
        }

        var hasSeverityIndicators = content.Contains("high") || content.Contains("medium") ||
                                    content.Contains("low") || content.Contains("critical") ||
                                    content.Contains("severe") || content.Contains("moderate");
        
        if (hasSeverityIndicators)
        {
            response.Confidence = Math.Min(1.0, response.Confidence + 0.2);
        }

        return await Task.FromResult(true);
    }
}