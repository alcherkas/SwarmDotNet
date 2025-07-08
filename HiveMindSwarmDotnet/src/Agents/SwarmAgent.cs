using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;

namespace HiveMindSwarmDotnet.Console.Agents;

public abstract class SwarmAgent : ISwarmAgent
{
    protected readonly Kernel _kernel;
    protected readonly ILogger<SwarmAgent> _logger;
    protected readonly AgentRoleConfiguration _configuration;
    protected readonly IChatCompletionService _chatService;

    public string Id { get; }
    public AgentRole Role { get; }
    public bool IsAvailable { get; protected set; } = true;

    protected SwarmAgent(
        AgentRole role,
        Kernel kernel,
        AgentRoleConfiguration configuration,
        ILogger<SwarmAgent> logger)
    {
        Id = Guid.NewGuid().ToString();
        Role = role;
        _kernel = kernel;
        _configuration = configuration;
        _logger = logger;
        _chatService = kernel.GetRequiredService<IChatCompletionService>();
    }

    public virtual async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Initializing agent {AgentId} with role {Role}", Id, Role);
        IsAvailable = true;
        await Task.CompletedTask;
    }

    public virtual async Task<AgentResponse> ProcessTaskAsync(AgentTask task, CancellationToken cancellationToken = default)
    {
        if (!IsAvailable)
        {
            throw new InvalidOperationException($"Agent {Id} is not available");
        }

        IsAvailable = false;
        
        try
        {
            _logger.LogInformation("Agent {AgentId} processing task {TaskId}", Id, task.Id);
            
            var chatHistory = new ChatHistory();
            chatHistory.AddSystemMessage(_configuration.SystemPrompt);
            chatHistory.AddUserMessage(BuildPrompt(task));

            var response = await _chatService.GetChatMessageContentAsync(
                chatHistory,
                new PromptExecutionSettings
                {
                    ExtensionData = new Dictionary<string, object>
                    {
                        ["max_tokens"] = _configuration.MaxTokens,
                        ["temperature"] = _configuration.Temperature
                    }
                },
                _kernel,
                cancellationToken);

            var agentResponse = new AgentResponse
            {
                TaskId = task.Id,
                AgentId = Id,
                Role = Role,
                Content = response.Content ?? string.Empty,
                Confidence = CalculateConfidence(response.Content ?? string.Empty),
                Metadata = new Dictionary<string, object>
                {
                    ["model"] = _configuration.ModelId,
                    ["tokens"] = response.Metadata?.Count ?? 0,
                    ["processing_time"] = DateTime.UtcNow.Subtract(task.CreatedAt).TotalMilliseconds
                }
            };

            await ValidateResponseInternalAsync(agentResponse, cancellationToken);
            
            _logger.LogInformation("Agent {AgentId} completed task {TaskId} with confidence {Confidence}", 
                Id, task.Id, agentResponse.Confidence);
            
            return agentResponse;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing task {TaskId} with agent {AgentId}", task.Id, Id);
            
            return new AgentResponse
            {
                TaskId = task.Id,
                AgentId = Id,
                Role = Role,
                Content = $"Error processing task: {ex.Message}",
                Confidence = 0.0,
                IsValid = false,
                ValidationErrors = new List<string> { ex.Message }
            };
        }
        finally
        {
            IsAvailable = true;
        }
    }

    public virtual async Task<bool> ValidateResponseAsync(AgentResponse response, CancellationToken cancellationToken = default)
    {
        if (response.Role != Role)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(response.Content))
        {
            response.ValidationErrors.Add("Response content is empty");
            return false;
        }

        return await ValidateRoleSpecificResponseAsync(response, cancellationToken);
    }

    protected virtual string BuildPrompt(AgentTask task)
    {
        var prompt = task.Prompt;
        
        if (task.Context.Any())
        {
            prompt += "\n\nContext:\n";
            foreach (var context in task.Context)
            {
                prompt += $"- {context.Key}: {context.Value}\n";
            }
        }

        return prompt;
    }

    protected virtual double CalculateConfidence(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
            return 0.0;

        var confidence = 0.5;
        
        if (response.Contains("I'm confident") || response.Contains("I'm certain"))
            confidence += 0.3;
        else if (response.Contains("I think") || response.Contains("probably"))
            confidence += 0.1;
        else if (response.Contains("I'm not sure") || response.Contains("uncertain"))
            confidence -= 0.2;

        if (response.Length > 100)
            confidence += 0.1;

        return Math.Max(0.0, Math.Min(1.0, confidence));
    }

    protected virtual async Task ValidateResponseInternalAsync(AgentResponse response, CancellationToken cancellationToken)
    {
        response.IsValid = await ValidateResponseAsync(response, cancellationToken);
    }

    protected abstract Task<bool> ValidateRoleSpecificResponseAsync(AgentResponse response, CancellationToken cancellationToken);

    public virtual async Task DisposeAsync()
    {
        _logger.LogInformation("Disposing agent {AgentId}", Id);
        IsAvailable = false;
        await Task.CompletedTask;
    }
}