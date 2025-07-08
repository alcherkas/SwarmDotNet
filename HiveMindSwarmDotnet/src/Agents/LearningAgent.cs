using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace HiveMindSwarmDotnet.Console.Agents;

public class LearningAgent : SwarmAgent
{
    public LearningAgent(
        Kernel kernel,
        AgentRoleConfiguration configuration,
        ILogger<SwarmAgent> logger)
        : base(AgentRole.Learning, kernel, configuration, logger)
    {
    }

    protected override async Task<bool> ValidateRoleSpecificResponseAsync(AgentResponse response, CancellationToken cancellationToken)
    {
        var content = response.Content.ToLower();
        
        var hasLearningElements = content.Contains("learn") || content.Contains("improve") ||
                                  content.Contains("pattern") || content.Contains("feedback") ||
                                  content.Contains("optimize") || content.Contains("adapt");
        
        if (!hasLearningElements)
        {
            response.ValidationErrors.Add("Learning Agent response should contain learning and improvement insights");
            return false;
        }

        var hasMetaAnalysis = content.Contains("accuracy") || content.Contains("effectiveness") ||
                              content.Contains("performance") || content.Contains("trend") ||
                              content.Contains("outcome");
        
        if (hasMetaAnalysis)
        {
            response.Confidence = Math.Min(1.0, response.Confidence + 0.2);
        }

        return await Task.FromResult(true);
    }
}