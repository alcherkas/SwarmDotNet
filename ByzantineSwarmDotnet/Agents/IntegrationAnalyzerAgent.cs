using ByzantineSwarmDotnet.Interfaces;
using ByzantineSwarmDotnet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace ByzantineSwarmDotnet.Agents;

public class IntegrationAnalyzerAgent : EnhancedSwarmAgentBase
{
    public IntegrationAnalyzerAgent(
        string id,
        Kernel kernel,
        ILogger<IntegrationAnalyzerAgent> logger,
        ICryptographicSigner signer,
        IByzantineDetector byzantineDetector,
        IReputationSystem reputationSystem,
        IFaultTolerantCommunicationHub communicationHub)
        : base(id, AgentRole.IntegrationAnalyzer, AgentHierarchyLevel.Worker, kernel, logger, signer, byzantineDetector, reputationSystem, communicationHub)
    {
    }

    public override async Task<AgentResponse> ProcessTaskAsync(AgentTask task)
    {
        try
        {
            var integrationPrompt = $@"
You are an integration analyzer agent specialized in system integration analysis.
Analyze the following for integration aspects and dependencies:

Task: {task.Description}
Parameters: {string.Join(", ", task.Parameters.Select(p => $"{p.Key}: {p.Value}"))}

Please provide:
1. API dependencies mapping
2. Downstream services impact analysis
3. Contract violations detection
4. Service mesh integration analysis
5. Integration test scenarios suggestions
6. System interface analysis
7. Communication protocol assessment

Focus on integration, dependencies, and system connectivity.
";

            var result = await _kernel.InvokePromptAsync(integrationPrompt);
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
            _logger.LogError(ex, "Error in IntegrationAnalyzerAgent processing task {TaskId}", task.Id);
            return this.CreateErrorResponse(task.Id, ex.Message);
        }
    }
}