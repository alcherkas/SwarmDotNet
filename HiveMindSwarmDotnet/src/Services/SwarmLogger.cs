using System.Text.Json;
using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;

namespace HiveMindSwarmDotnet.Console.Services;

public class SwarmLogger : ISwarmLogger
{
    private readonly ILogger<SwarmLogger> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public SwarmLogger(ILogger<SwarmLogger> logger)
    {
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    public async Task LogTaskStartAsync(SwarmTask task)
    {
        _logger.LogInformation("🚀 Swarm Task Started: {TaskId}", task.Id);
        _logger.LogInformation("   Description: {Description}", task.Description);
        _logger.LogInformation("   Required Roles: {Roles}", string.Join(", ", task.RequiredRoles));
        _logger.LogInformation("   Priority: {Priority}", task.Priority);
        _logger.LogInformation("   Max Execution Time: {MaxTime}", task.MaxExecutionTime);
        
        if (task.Parameters.Any())
        {
            _logger.LogDebug("   Parameters: {Parameters}", JsonSerializer.Serialize(task.Parameters, _jsonOptions));
        }

        await Task.CompletedTask;
    }

    public async Task LogTaskCompletedAsync(SwarmTask task, SwarmResult result)
    {
        var statusEmoji = result.IsSuccessful ? "✅" : "❌";
        
        _logger.LogInformation("{StatusEmoji} Swarm Task Completed: {TaskId}", statusEmoji, task.Id);
        _logger.LogInformation("   Execution Time: {ExecutionTime}ms", result.ExecutionTime.TotalMilliseconds);
        _logger.LogInformation("   Overall Confidence: {Confidence:F2}", result.OverallConfidence);
        _logger.LogInformation("   Agent Responses: {ResponseCount}", result.AgentResponses.Count);
        _logger.LogInformation("   Communications: {CommunicationCount}", result.Communications.Count);
        
        if (result.Errors.Any())
        {
            _logger.LogWarning("   Errors: {ErrorCount}", result.Errors.Count);
            foreach (var error in result.Errors)
            {
                _logger.LogWarning("     - {Error}", error);
            }
        }

        if (result.Metrics.Any())
        {
            _logger.LogDebug("   Metrics: {Metrics}", JsonSerializer.Serialize(result.Metrics, _jsonOptions));
        }

        await Task.CompletedTask;
    }

    public async Task LogAgentResponseAsync(AgentResponse response)
    {
        var validityEmoji = response.IsValid ? "✅" : "❌";
        var roleEmoji = GetRoleEmoji(response.Role);
        
        _logger.LogInformation("{RoleEmoji} Agent Response: {AgentId} ({Role}) {ValidityEmoji}", 
            roleEmoji, response.AgentId, response.Role, validityEmoji);
        _logger.LogInformation("   Confidence: {Confidence:F2}", response.Confidence);
        _logger.LogInformation("   Content Length: {ContentLength} chars", response.Content.Length);
        
        if (response.ValidationErrors.Any())
        {
            _logger.LogWarning("   Validation Errors: {ErrorCount}", response.ValidationErrors.Count);
            foreach (var error in response.ValidationErrors)
            {
                _logger.LogWarning("     - {Error}", error);
            }
        }

        if (response.Metadata.Any())
        {
            _logger.LogDebug("   Metadata: {Metadata}", JsonSerializer.Serialize(response.Metadata, _jsonOptions));
        }

        await Task.CompletedTask;
    }

    public async Task LogCommunicationAsync(AgentMessage message)
    {
        var typeEmoji = GetMessageTypeEmoji(message.Type);
        
        _logger.LogInformation("{TypeEmoji} Communication: {FromAgent} → {ToAgent}", 
            typeEmoji, message.FromAgentId, message.ToAgentId);
        _logger.LogInformation("   Type: {MessageType}", message.Type);
        _logger.LogInformation("   Content: {Content}", 
            message.Content.Length > 100 ? message.Content.Substring(0, 100) + "..." : message.Content);
        
        if (message.Metadata.Any())
        {
            _logger.LogDebug("   Metadata: {Metadata}", JsonSerializer.Serialize(message.Metadata, _jsonOptions));
        }

        await Task.CompletedTask;
    }

    public async Task LogErrorAsync(string error, Exception? exception = null)
    {
        if (exception != null)
        {
            _logger.LogError(exception, "🔥 Swarm Error: {Error}", error);
        }
        else
        {
            _logger.LogError("🔥 Swarm Error: {Error}", error);
        }

        await Task.CompletedTask;
    }

    private string GetRoleEmoji(AgentRole role)
    {
        return role switch
        {
            AgentRole.Orchestrator => "🎼",
            AgentRole.PRExtractor => "📋",
            AgentRole.JiraContext => "🎯",
            AgentRole.CodeAnalyzer => "🔍",
            AgentRole.RequirementMapper => "🗺️",
            AgentRole.TestCoverage => "🧪",
            AgentRole.RiskAssessment => "⚠️",
            AgentRole.IntegrationAnalyzer => "🔗",
            AgentRole.SummaryGenerator => "📝",
            AgentRole.Learning => "🧠",
            _ => "🤖"
        };
    }

    private string GetMessageTypeEmoji(MessageType type)
    {
        return type switch
        {
            MessageType.Information => "ℹ️",
            MessageType.Request => "❓",
            MessageType.Response => "💬",
            MessageType.Validation => "✅",
            MessageType.Error => "❌",
            _ => "📨"
        };
    }
}