using ByzantineSwarmDotnet.Interfaces;
using ByzantineSwarmDotnet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace ByzantineSwarmDotnet.Agents;

public class JiraContextAgent : EnhancedSwarmAgentBase
{
    public JiraContextAgent(
        string id,
        Kernel kernel,
        ILogger<JiraContextAgent> logger,
        ICryptographicSigner signer,
        IByzantineDetector byzantineDetector,
        IReputationSystem reputationSystem,
        IFaultTolerantCommunicationHub communicationHub)
        : base(id, AgentRole.JiraContext, AgentHierarchyLevel.Worker, kernel, logger, signer, byzantineDetector, reputationSystem, communicationHub)
    {
    }

    public override async Task<AgentResponse> ProcessTaskAsync(AgentTask task)
    {
        try
        {
            var contextPrompt = $@"
You are a Jira context agent specialized in business requirement analysis.
Analyze the following business context and requirements:

Task: {task.Description}
Parameters: {string.Join(", ", task.Parameters.Select(p => $"{p.Key}: {p.Value}"))}

Please provide:
1. Linked Jira tickets and epics analysis
2. Acceptance criteria extraction
3. User stories interpretation
4. Business value assessment
5. Related ticket dependencies
6. Historical context from comments
7. Stakeholder requirements mapping

Focus on business context and requirement analysis.
";

            var result = await _kernel.InvokePromptAsync(contextPrompt);
            var response = new AgentResponse
            {
                TaskId = task.Id,
                AgentId = Id,
                Success = true,
                Result = result.ToString(),
                ConfidenceScore = await this.CalculateConfidenceAsync(result.ToString()),
                Timestamp = DateTime.UtcNow
            };

            return await this.ValidateAndSignResponseAsync(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in JiraContextAgent processing task {TaskId}", task.Id);
            return this.CreateErrorResponse(task.Id, ex.Message);
        }
    }
}