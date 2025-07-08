using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace HiveMindSwarmDotnet.Console.Agents;

public class JiraContextAgent : SwarmAgent
{
    public JiraContextAgent(
        Kernel kernel,
        AgentRoleConfiguration configuration,
        ILogger<SwarmAgent> logger)
        : base(AgentRole.JiraContext, kernel, configuration, logger)
    {
    }

    protected override async Task<bool> ValidateRoleSpecificResponseAsync(AgentResponse response, CancellationToken cancellationToken)
    {
        var content = response.Content.ToLower();
        
        var hasJiraElements = content.Contains("ticket") || content.Contains("epic") ||
                              content.Contains("story") || content.Contains("acceptance") ||
                              content.Contains("requirement") || content.Contains("jira");
        
        if (!hasJiraElements)
        {
            response.ValidationErrors.Add("Jira Context response should contain business requirement information");
            return false;
        }

        var hasBusinessContext = content.Contains("business") || content.Contains("user") ||
                                 content.Contains("criteria") || content.Contains("value");
        
        if (hasBusinessContext)
        {
            response.Confidence = Math.Min(1.0, response.Confidence + 0.15);
        }

        return await Task.FromResult(true);
    }
}