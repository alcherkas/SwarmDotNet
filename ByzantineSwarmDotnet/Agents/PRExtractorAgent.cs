using ByzantineSwarmDotnet.Interfaces;
using ByzantineSwarmDotnet.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace ByzantineSwarmDotnet.Agents;

public class PRExtractorAgent : EnhancedSwarmAgentBase
{
    public PRExtractorAgent(
        string id,
        Kernel kernel,
        ILogger<PRExtractorAgent> logger,
        ICryptographicSigner signer,
        IByzantineDetector byzantineDetector,
        IReputationSystem reputationSystem,
        IFaultTolerantCommunicationHub communicationHub)
        : base(id, AgentRole.PRExtractor, AgentHierarchyLevel.Worker, kernel, logger, signer, byzantineDetector, reputationSystem, communicationHub)
    {
    }

    public override async Task<AgentResponse> ProcessTaskAsync(AgentTask task)
    {
        try
        {
            var extractionPrompt = $@"
You are a PR extractor agent specialized in analyzing pull request data.
Extract and analyze the following PR information:

Task: {task.Description}
Parameters: {string.Join(", ", task.Parameters.Select(p => $"{p.Key}: {p.Value}"))}

Please provide:
1. PR metadata extraction (title, description, labels)
2. Modified files and diff analysis
3. Linked Jira tickets identification
4. Commit message analysis
5. Change impact assessment
6. Structured data format for further processing

Focus on thorough data extraction and analysis.
";

            var result = await _kernel.InvokePromptAsync(extractionPrompt);
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
            _logger.LogError(ex, "Error in PRExtractorAgent processing task {TaskId}", task.Id);
            return this.CreateErrorResponse(task.Id, ex.Message);
        }
    }
}