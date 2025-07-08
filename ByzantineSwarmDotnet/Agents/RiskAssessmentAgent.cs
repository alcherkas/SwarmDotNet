using ByzantineSwarmDotnet.Interfaces;
using ByzantineSwarmDotnet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace ByzantineSwarmDotnet.Agents;

public class RiskAssessmentAgent : EnhancedSwarmAgentBase
{
    public RiskAssessmentAgent(
        string id,
        Kernel kernel,
        ILogger<RiskAssessmentAgent> logger,
        ICryptographicSigner signer,
        IByzantineDetector byzantineDetector,
        IReputationSystem reputationSystem,
        IFaultTolerantCommunicationHub communicationHub)
        : base(id, AgentRole.RiskAssessment, AgentHierarchyLevel.Worker, kernel, logger, signer, byzantineDetector, reputationSystem, communicationHub)
    {
    }

    public override async Task<AgentResponse> ProcessTaskAsync(AgentTask task)
    {
        try
        {
            var riskPrompt = $@"
You are a risk assessment agent specialized in identifying and analyzing risks.
Assess the following for potential risks and impacts:

Task: {task.Description}
Parameters: {string.Join(", ", task.Parameters.Select(p => $"{p.Key}: {p.Value}"))}

Please provide:
1. Breaking changes identification
2. Integration risks assessment
3. Performance impact analysis
4. Security vulnerability evaluation
5. Risk severity classification (High/Medium/Low)
6. Mitigation strategies
7. Impact assessment on downstream systems

Focus on risk analysis and impact assessment.
";

            var result = await _kernel.InvokePromptAsync(riskPrompt);
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
            _logger.LogError(ex, "Error in RiskAssessmentAgent processing task {TaskId}", task.Id);
            return this.CreateErrorResponse(task.Id, ex.Message);
        }
    }
}