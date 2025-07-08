using ByzantineSwarmDotnet.Interfaces;
using ByzantineSwarmDotnet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace ByzantineSwarmDotnet.Agents;

public class RequirementMapperAgent : EnhancedSwarmAgentBase
{
    public RequirementMapperAgent(
        string id,
        Kernel kernel,
        ILogger<RequirementMapperAgent> logger,
        ICryptographicSigner signer,
        IByzantineDetector byzantineDetector,
        IReputationSystem reputationSystem,
        IFaultTolerantCommunicationHub communicationHub)
        : base(id, AgentRole.RequirementMapper, AgentHierarchyLevel.Worker, kernel, logger, signer, byzantineDetector, reputationSystem, communicationHub)
    {
    }

    public override async Task<AgentResponse> ProcessTaskAsync(AgentTask task)
    {
        try
        {
            var mappingPrompt = $@"
You are a requirement mapper agent specialized in business-to-code alignment.
Map and trace the following requirements to implementation:

Task: {task.Description}
Parameters: {string.Join(", ", task.Parameters.Select(p => $"{p.Key}: {p.Value}"))}

Please provide:
1. Code changes to acceptance criteria mapping
2. Requirement coverage analysis
3. Unimplemented requirements identification
4. Traceability matrix creation
5. Gap analysis and recommendations
6. Implementation completeness assessment
7. Business value alignment verification

Focus on mapping, traceability, and alignment analysis.
";

            var result = await _kernel.InvokePromptAsync(mappingPrompt);
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
            _logger.LogError(ex, "Error in RequirementMapperAgent processing task {TaskId}", task.Id);
            return this.CreateErrorResponse(task.Id, ex.Message);
        }
    }
}