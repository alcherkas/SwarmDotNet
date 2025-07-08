using ByzantineSwarmDotnet.Interfaces;
using ByzantineSwarmDotnet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace ByzantineSwarmDotnet.Agents;

public class CodeAnalyzerAgent : EnhancedSwarmAgentBase
{
    public CodeAnalyzerAgent(
        string id,
        Kernel kernel,
        ILogger<CodeAnalyzerAgent> logger,
        ICryptographicSigner signer,
        IByzantineDetector byzantineDetector,
        IReputationSystem reputationSystem,
        IFaultTolerantCommunicationHub communicationHub)
        : base(id, AgentRole.CodeAnalyzer, AgentHierarchyLevel.Worker, kernel, logger, signer, byzantineDetector, reputationSystem, communicationHub)
    {
    }

    public override async Task<AgentResponse> ProcessTaskAsync(AgentTask task)
    {
        try
        {
            var analysisPrompt = $@"
You are a code analyzer agent specialized in technical implementation analysis.
Analyze the following code changes and technical aspects:

Task: {task.Description}
Parameters: {string.Join(", ", task.Parameters.Select(p => $"{p.Key}: {p.Value}"))}

Please provide:
1. Static code analysis results
2. Design patterns identification
3. Architectural changes assessment
4. API modifications analysis
5. Database schema changes
6. Code complexity metrics
7. Quality assessment and recommendations
8. Security implications

Focus on technical depth and implementation analysis.
";

            var result = await _kernel.InvokePromptAsync(analysisPrompt);
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
            _logger.LogError(ex, "Error in CodeAnalyzerAgent processing task {TaskId}", task.Id);
            return this.CreateErrorResponse(task.Id, ex.Message);
        }
    }
}