using System.Collections.Concurrent;
using System.Diagnostics;
using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;

namespace HiveMindSwarmDotnet.Console.Choreography;

public class ChoreographyCoordinator : IChoreographyCoordinator
{
    private readonly IEventBus _eventBus;
    private readonly IAgentPool _agentPool;
    private readonly IChoreographyConfiguration _configuration;
    private readonly ILogger<ChoreographyCoordinator> _logger;
    private readonly ConcurrentDictionary<string, SwarmMetrics> _metrics = new();
    private readonly ConcurrentDictionary<string, IAutonomousAgent> _autonomousAgents = new();
    private readonly ConcurrentDictionary<string, SwarmTaskContext> _taskContexts = new();

    public ChoreographyCoordinator(
        IEventBus eventBus,
        IAgentPool agentPool,
        IChoreographyConfiguration configuration,
        ILogger<ChoreographyCoordinator> logger)
    {
        _eventBus = eventBus;
        _agentPool = agentPool;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Initializing Choreography Coordinator");
        
        await _eventBus.SubscribeAsync<TaskCompletedEvent>(OnTaskCompletedAsync, cancellationToken);
        await _eventBus.SubscribeAsync<CollaborationRequestEvent>(OnCollaborationRequestAsync, cancellationToken);
        await _eventBus.SubscribeAsync<AgentCapabilityEvent>(OnAgentCapabilityAsync, cancellationToken);

        await InitializeAutonomousAgentsAsync(cancellationToken);
    }

    public async Task<SwarmResult> ExecuteTaskAsync(SwarmTask task, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var swarmResult = new SwarmResult
        {
            TaskId = task.Id,
            CreatedAt = DateTime.UtcNow
        };

        try
        {
            _logger.LogInformation("Starting choreographed execution of swarm task {TaskId}: {Description}", 
                task.Id, task.Description);

            var taskContext = new SwarmTaskContext
            {
                SwarmTask = task,
                StartTime = DateTime.UtcNow,
                ExpectedRoles = task.RequiredRoles.ToList(),
                CompletedResponses = new List<AgentResponse>(),
                PendingRoles = task.RequiredRoles.ToList()
            };

            _taskContexts.TryAdd(task.Id, taskContext);

            await PublishTaskEventsAsync(task, cancellationToken);

            var result = await WaitForTaskCompletionAsync(task.Id, cancellationToken);
            
            swarmResult.AgentResponses = result.responses;
            swarmResult.Communications = result.communications;
            swarmResult.FinalAnswer = await SynthesizeResultsAsync(result.responses, cancellationToken);
            swarmResult.OverallConfidence = CalculateOverallConfidence(result.responses);

            UpdateMetrics(task, swarmResult);
            
            _logger.LogInformation("Completed choreographed swarm task {TaskId} in {ElapsedMs}ms with confidence {Confidence}", 
                task.Id, stopwatch.ElapsedMilliseconds, swarmResult.OverallConfidence);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing choreographed swarm task {TaskId}", task.Id);
            swarmResult.IsSuccessful = false;
            swarmResult.Errors.Add(ex.Message);
        }
        finally
        {
            stopwatch.Stop();
            swarmResult.ExecutionTime = stopwatch.Elapsed;
            _taskContexts.TryRemove(task.Id, out _);
        }

        return swarmResult;
    }

    public async Task<SwarmMetrics> GetMetricsAsync()
    {
        var overallMetrics = new SwarmMetrics
        {
            SwarmId = "choreography_overall",
            LastUpdated = DateTime.UtcNow
        };

        foreach (var metric in _metrics.Values)
        {
            overallMetrics.TotalTasks += metric.TotalTasks;
            overallMetrics.CompletedTasks += metric.CompletedTasks;
            overallMetrics.FailedTasks += metric.FailedTasks;
            overallMetrics.TotalCommunications += metric.TotalCommunications;
            
            foreach (var utilization in metric.AgentUtilization)
            {
                overallMetrics.AgentUtilization[utilization.Key] = 
                    overallMetrics.AgentUtilization.GetValueOrDefault(utilization.Key, 0) + utilization.Value;
            }
        }

        if (_metrics.Values.Any())
        {
            overallMetrics.AverageExecutionTime = _metrics.Values.Average(m => m.AverageExecutionTime);
            overallMetrics.AverageConfidence = _metrics.Values.Average(m => m.AverageConfidence);
        }

        return await Task.FromResult(overallMetrics);
    }

    public async Task DisposeAsync()
    {
        _logger.LogInformation("Disposing Choreography Coordinator");
        
        foreach (var agent in _autonomousAgents.Values)
        {
            await _agentPool.ReturnAgentAsync(agent as ISwarmAgent, CancellationToken.None);
        }
        
        _autonomousAgents.Clear();
        await Task.CompletedTask;
    }

    private async Task InitializeAutonomousAgentsAsync(CancellationToken cancellationToken)
    {
        var roles = Enum.GetValues<AgentRole>();
        
        foreach (var role in roles)
        {
            var baseAgent = await _agentPool.GetAgentAsync(role, cancellationToken);
            if (baseAgent != null)
            {
                var autonomousAgent = new AutonomousAgent(baseAgent, _logger as ILogger<AutonomousAgent>);
                await autonomousAgent.InitializeAsync(_eventBus, cancellationToken);
                
                _autonomousAgents.TryAdd(autonomousAgent.AgentId, autonomousAgent);
                _logger.LogInformation("Initialized autonomous agent {AgentId} with role {Role}", 
                    autonomousAgent.AgentId, role);
            }
        }
    }

    private async Task PublishTaskEventsAsync(SwarmTask task, CancellationToken cancellationToken)
    {
        foreach (var role in task.RequiredRoles)
        {
            var taskEvent = new TaskAvailableEvent
            {
                SourceAgentId = "choreography_coordinator",
                RequiredRole = role,
                TaskDescription = task.Description,
                Priority = 5,
                Payload = task.Parameters
            };

            await _eventBus.PublishAsync(taskEvent, cancellationToken);
            _logger.LogDebug("Published task event for role {Role}", role);
        }

        if (_configuration.EnableAutoCollaboration)
        {
            var collaborationEvent = new CollaborationRequestEvent
            {
                SourceAgentId = "choreography_coordinator",
                RequestingAgentId = "choreography_coordinator",
                RequiredRoles = task.RequiredRoles.ToList(),
                Context = $"Auto-collaboration for task: {task.Description}"
            };

            await _eventBus.PublishAsync(collaborationEvent, cancellationToken);
        }
    }

    private async Task<(List<AgentResponse> responses, List<AgentMessage> communications)> WaitForTaskCompletionAsync(
        string taskId, CancellationToken cancellationToken)
    {
        var responses = new List<AgentResponse>();
        var communications = new List<AgentMessage>();
        var timeout = DateTime.UtcNow.Add(_configuration.TaskTimeout);

        while (DateTime.UtcNow < timeout && !cancellationToken.IsCancellationRequested)
        {
            if (_taskContexts.TryGetValue(taskId, out var context))
            {
                if (context.PendingRoles.Count == 0)
                {
                    responses = context.CompletedResponses;
                    break;
                }
            }

            await Task.Delay(100, cancellationToken);
        }

        return (responses, communications);
    }

    private async Task OnTaskCompletedAsync(TaskCompletedEvent taskEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Task {TaskId} completed by {Role} with confidence {Confidence}", 
            taskEvent.TaskId, taskEvent.CompletedByRole, taskEvent.Confidence);

        foreach (var context in _taskContexts.Values)
        {
            if (context.PendingRoles.Contains(taskEvent.CompletedByRole))
            {
                var response = new AgentResponse
                {
                    AgentId = taskEvent.SourceAgentId,
                    TaskId = taskEvent.TaskId,
                    Role = taskEvent.CompletedByRole,
                    Content = taskEvent.Result,
                    Confidence = taskEvent.Confidence,
                    IsValid = true,
                    CreatedAt = taskEvent.Timestamp
                };

                context.CompletedResponses.Add(response);
                context.PendingRoles.Remove(taskEvent.CompletedByRole);

                if (context.PendingRoles.Count == 0)
                {
                    await TriggerResultSynthesisAsync(context, cancellationToken);
                }
            }
        }
    }

    private async Task OnCollaborationRequestAsync(CollaborationRequestEvent collaborationEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Collaboration request from {RequestingAgent} for roles: {Roles}", 
            collaborationEvent.RequestingAgentId, string.Join(", ", collaborationEvent.RequiredRoles));

        if (_configuration.EnableAutoCollaboration)
        {
            foreach (var role in collaborationEvent.RequiredRoles)
            {
                var taskEvent = new TaskAvailableEvent
                {
                    SourceAgentId = "choreography_coordinator",
                    RequiredRole = role,
                    TaskDescription = $"Collaboration task: {collaborationEvent.Context}",
                    Priority = 7,
                    Payload = new Dictionary<string, object>
                    {
                        ["collaboration_request"] = true,
                        ["requesting_agent"] = collaborationEvent.RequestingAgentId
                    }
                };

                await _eventBus.PublishAsync(taskEvent, cancellationToken);
            }
        }
    }

    private async Task OnAgentCapabilityAsync(AgentCapabilityEvent capabilityEvent, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Agent {AgentId} with role {Role} reported capabilities: {Capabilities}", 
            capabilityEvent.SourceAgentId, capabilityEvent.Role, 
            string.Join(", ", capabilityEvent.Capabilities));

        await Task.CompletedTask;
    }

    private async Task TriggerResultSynthesisAsync(SwarmTaskContext context, CancellationToken cancellationToken)
    {
        var synthesisEvent = new ResultSynthesisEvent
        {
            SourceAgentId = "choreography_coordinator",
            SwarmTaskId = context.SwarmTask.Id,
            Responses = context.CompletedResponses
        };

        await _eventBus.PublishAsync(synthesisEvent, cancellationToken);
    }

    private async Task<string> SynthesizeResultsAsync(List<AgentResponse> responses, CancellationToken cancellationToken)
    {
        var summaryResponse = responses.FirstOrDefault(r => r.Role == AgentRole.SummaryGenerator);
        if (summaryResponse != null)
        {
            return summaryResponse.Content;
        }

        var highConfidenceResponses = responses
            .Where(r => r.Confidence > _configuration.MinConfidenceThreshold)
            .OrderByDescending(r => r.Confidence)
            .Take(3)
            .ToList();

        if (highConfidenceResponses.Any())
        {
            var synthesis = "Choreographed Analysis Summary:\n\n";
            foreach (var response in highConfidenceResponses)
            {
                synthesis += $"[{response.Role}] {response.Content}\n\n";
            }
            return synthesis;
        }

        return responses.FirstOrDefault()?.Content ?? "No valid responses generated in choreography";
    }

    private double CalculateOverallConfidence(List<AgentResponse> responses)
    {
        if (!responses.Any())
            return 0.0;

        var validResponses = responses.Where(r => r.IsValid).ToList();
        if (!validResponses.Any())
            return 0.0;

        return validResponses.Average(r => r.Confidence);
    }

    private void UpdateMetrics(SwarmTask task, SwarmResult result)
    {
        var metrics = _metrics.GetOrAdd(task.Id, _ => new SwarmMetrics { SwarmId = task.Id });
        
        metrics.TotalTasks++;
        if (result.IsSuccessful)
            metrics.CompletedTasks++;
        else
            metrics.FailedTasks++;

        metrics.AverageExecutionTime = (metrics.AverageExecutionTime + result.ExecutionTime.TotalMilliseconds) / 2;
        metrics.AverageConfidence = (metrics.AverageConfidence + result.OverallConfidence) / 2;
        metrics.TotalCommunications += result.Communications.Count;
        metrics.LastUpdated = DateTime.UtcNow;

        foreach (var response in result.AgentResponses)
        {
            metrics.AgentUtilization[response.Role] = metrics.AgentUtilization.GetValueOrDefault(response.Role, 0) + 1;
        }
    }

    private class SwarmTaskContext
    {
        public SwarmTask SwarmTask { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public List<AgentRole> ExpectedRoles { get; set; } = new();
        public List<AgentRole> PendingRoles { get; set; } = new();
        public List<AgentResponse> CompletedResponses { get; set; } = new();
    }
}