using ByzantineSwarmDotnet.Models;

namespace ByzantineSwarmDotnet.Agents;

public static class AgentExtensions
{
    public static async Task<AgentResponse> ValidateAndSignResponseAsync(this EnhancedSwarmAgentBase agent, AgentResponse response)
    {
        var isValid = await agent.ValidateResponseAsync(response);
        if (!isValid)
        {
            response = response with { Success = false, ErrorMessage = "Response validation failed" };
        }

        var message = new AgentMessage { Response = response };
        var signature = await agent.SignMessageAsync(message);
        
        return response with { Signature = signature };
    }

    public static AgentResponse CreateErrorResponse(this EnhancedSwarmAgentBase agent, string taskId, string errorMessage)
    {
        return new AgentResponse
        {
            TaskId = taskId,
            AgentId = agent.Id,
            Success = false,
            ErrorMessage = errorMessage,
            Timestamp = DateTime.UtcNow
        };
    }

    public static async Task<double> CalculateConfidenceAsync(this EnhancedSwarmAgentBase agent, string result)
    {
        if (string.IsNullOrEmpty(result))
            return 0.0;

        // Basic confidence calculation based on result length and structure
        var baseConfidence = 0.5;
        var lengthFactor = Math.Min(result.Length / 1000.0, 0.3); // Up to 0.3 for good length
        var structureFactor = (result.Contains("1.") || result.Contains("•") || result.Contains("-")) ? 0.2 : 0.0;
        
        return Math.Min(1.0, baseConfidence + lengthFactor + structureFactor);
    }
}