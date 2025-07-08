using ByzantineSwarmDotnet.Interfaces;
using ByzantineSwarmDotnet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace ByzantineSwarmDotnet.Agents;

public class LearningAgent : EnhancedSwarmAgentBase
{
    public LearningAgent(
        string id,
        Kernel kernel,
        ILogger<LearningAgent> logger,
        ICryptographicSigner signer,
        IByzantineDetector byzantineDetector,
        IReputationSystem reputationSystem,
        IFaultTolerantCommunicationHub communicationHub)
        : base(id, AgentRole.Learning, AgentHierarchyLevel.Coordinator, kernel, logger, signer, byzantineDetector, reputationSystem, communicationHub)
    {
    }

    public override async Task<AgentResponse> ProcessTaskAsync(AgentTask task)
    {
        try
        {
            var learningPrompt = $@"
You are a learning agent specialized in continuous improvement and optimization.
Analyze the following for learning opportunities and improvements:

Task: {task.Description}
Parameters: {string.Join(", ", task.Parameters.Select(p => $"{p.Key}: {p.Value}"))}

Please provide:
1. Pattern recognition and analysis
2. Feedback processing and insights
3. Performance improvement recommendations
4. Process optimization suggestions
5. Accuracy and effectiveness analysis
6. Learning outcomes and adaptations
7. Future enhancement recommendations

Focus on learning, improvement, and optimization.
";

            var result = await _kernel.InvokePromptAsync(learningPrompt);
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
            _logger.LogError(ex, "Error in LearningAgent processing task {TaskId}", task.Id);
            return this.CreateErrorResponse(task.Id, ex.Message);
        }
    }
}