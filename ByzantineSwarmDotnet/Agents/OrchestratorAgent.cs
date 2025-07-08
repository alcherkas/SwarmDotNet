using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;
using ByzantineSwarmDotnet.Models;
using ByzantineSwarmDotnet.Interfaces;

namespace ByzantineSwarmDotnet.Agents;

public class OrchestratorAgent : EnhancedSwarmAgentBase
{
    public OrchestratorAgent(
        string id,
        Kernel kernel,
        ILogger<OrchestratorAgent> logger,
        ICryptographicSigner signer,
        IByzantineDetector byzantineDetector,
        IReputationSystem reputationSystem,
        IFaultTolerantCommunicationHub communicationHub)
        : base(id, AgentRole.Orchestrator, AgentHierarchyLevel.Coordinator, kernel, logger, signer, byzantineDetector, reputationSystem, communicationHub)
    {
    }

    public override async Task<AgentResponse> ProcessTaskAsync(AgentTask task)
    {
        try
        {
            var orchestrationPrompt = $@"
You are an orchestrator agent responsible for coordinating and managing workflows.
Analyze the following task and create a coordination strategy:

Task: {task.Description}
Type: {task.Type}
Parameters: {string.Join(", ", task.Parameters.Select(p => $"{p.Key}: {p.Value}"))}

Please provide:
1. Workflow orchestration plan
2. Task sequencing and dependencies
3. Resource allocation strategy
4. Risk mitigation measures
5. Success criteria and monitoring points

Focus on coordination, management, and workflow optimization.
";

            var result = await _kernel.InvokePromptAsync(orchestrationPrompt);
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
            _logger.LogError(ex, "Error in OrchestratorAgent processing task {TaskId}", task.Id);
            return this.CreateErrorResponse(task.Id, ex.Message);
        }
    }

    private async Task<bool> ValidateOrchestratorResponse(string content)
    {
        var lowerContent = content.ToLower();
        return lowerContent.Contains("orchestrate") || lowerContent.Contains("coordinate") ||
               lowerContent.Contains("manage") || lowerContent.Contains("workflow");
    }
}