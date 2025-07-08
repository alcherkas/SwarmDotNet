using System.Text.Json;
using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace HiveMindSwarmDotnet.Console.Agents;

public class OrchestratorAgent : SwarmAgent
{
    public OrchestratorAgent(
        Kernel kernel,
        AgentRoleConfiguration configuration,
        ILogger<SwarmAgent> logger)
        : base(AgentRole.Orchestrator, kernel, configuration, logger)
    {
    }

    protected override async Task<bool> ValidateRoleSpecificResponseAsync(AgentResponse response, CancellationToken cancellationToken)
    {
        var content = response.Content.ToLower();
        
        var hasCoordinationWords = content.Contains("orchestrate") || content.Contains("coordinate") ||
                                   content.Contains("manage") || content.Contains("sequence") ||
                                   content.Contains("workflow") || content.Contains("distribute");
        
        if (!hasCoordinationWords)
        {
            response.ValidationErrors.Add("Orchestrator response should contain coordination and workflow management language");
            return false;
        }

        return await Task.FromResult(true);
    }
}