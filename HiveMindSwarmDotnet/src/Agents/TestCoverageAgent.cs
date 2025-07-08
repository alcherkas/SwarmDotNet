using System.Text.RegularExpressions;
using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace HiveMindSwarmDotnet.Console.Agents;

public class TestCoverageAgent : SwarmAgent
{
    public TestCoverageAgent(
        Kernel kernel,
        AgentRoleConfiguration configuration,
        ILogger<SwarmAgent> logger)
        : base(AgentRole.TestCoverage, kernel, configuration, logger)
    {
    }

    protected override async Task<bool> ValidateRoleSpecificResponseAsync(AgentResponse response, CancellationToken cancellationToken)
    {
        var content = response.Content.ToLower();
        
        var hasTestElements = content.Contains("test") || content.Contains("coverage") ||
                              content.Contains("scenario") || content.Contains("assertion") ||
                              content.Contains("quality") || content.Contains("validation");
        
        if (!hasTestElements)
        {
            response.ValidationErrors.Add("Test Coverage response should contain testing and quality assurance information");
            return false;
        }

        var hasMetrics = Regex.IsMatch(content, @"\d+%") || content.Contains("percentage") ||
                         content.Contains("metric") || content.Contains("untested");
        
        if (hasMetrics)
        {
            response.Confidence = Math.Min(1.0, response.Confidence + 0.2);
        }

        return await Task.FromResult(true);
    }
}