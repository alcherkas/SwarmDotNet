using ByzantineSwarmDotnet.Interfaces;
using ByzantineSwarmDotnet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace ByzantineSwarmDotnet.Agents;

public class SummaryGeneratorAgent : EnhancedSwarmAgentBase
{
    public SummaryGeneratorAgent(
        string id,
        Kernel kernel,
        ILogger<SummaryGeneratorAgent> logger,
        ICryptographicSigner signer,
        IByzantineDetector byzantineDetector,
        IReputationSystem reputationSystem,
        IFaultTolerantCommunicationHub communicationHub)
        : base(id, AgentRole.SummaryGenerator, AgentHierarchyLevel.Coordinator, kernel, logger, signer, byzantineDetector, reputationSystem, communicationHub)
    {
    }

    public override async Task<AgentResponse> ProcessTaskAsync(AgentTask task)
    {
        try
        {
            var summaryPrompt = $@"
You are a summary generator agent specialized in synthesizing and documenting findings.
Create a comprehensive summary of the following analysis:

Task: {task.Description}
Parameters: {string.Join(", ", task.Parameters.Select(p => $"{p.Key}: {p.Value}"))}

Please provide:
1. Executive summary of findings
2. Key insights and recommendations
3. Structured synthesis of all analysis
4. Stakeholder-specific reports
5. Review checklists
6. Action items and next steps
7. Risk summary and mitigation strategies

Focus on comprehensive synthesis and clear communication.
";

            var result = await _kernel.InvokePromptAsync(summaryPrompt);
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
            _logger.LogError(ex, "Error in SummaryGeneratorAgent processing task {TaskId}", task.Id);
            return this.CreateErrorResponse(task.Id, ex.Message);
        }
    }
}