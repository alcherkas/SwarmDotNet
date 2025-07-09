using System.Collections.Concurrent;
using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;

namespace HiveMindSwarmDotnet.Console.Choreography;

public class EventDrivenTaskDistributor : ITaskDistributor
{
    private readonly IEventBus _eventBus;
    private readonly ILogger<EventDrivenTaskDistributor> _logger;
    private readonly ConcurrentDictionary<string, AgentTask> _tasks = new();
    private readonly ConcurrentDictionary<string, TaskPublicationContext> _taskContexts = new();

    public EventDrivenTaskDistributor(IEventBus eventBus, ILogger<EventDrivenTaskDistributor> logger)
    {
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<List<AgentTask>> DistributeTaskAsync(SwarmTask swarmTask, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Event-driven distribution of swarm task {TaskId} with {RoleCount} required roles", 
            swarmTask.Id, swarmTask.RequiredRoles.Length);

        var agentTasks = new List<AgentTask>();
        
        var taskContext = new TaskPublicationContext
        {
            SwarmTask = swarmTask,
            PublishedTasks = new List<AgentTask>(),
            PendingRoles = swarmTask.RequiredRoles.ToList()
        };

        foreach (var role in swarmTask.RequiredRoles)
        {
            var agentTask = CreateAgentTask(swarmTask, role);
            _tasks.TryAdd(agentTask.Id, agentTask);
            agentTasks.Add(agentTask);
            taskContext.PublishedTasks.Add(agentTask);

            await PublishTaskEventAsync(agentTask, cancellationToken);
        }

        if (ShouldIncludeRiskAssessment(swarmTask))
        {
            var riskTask = CreateRiskAssessmentTask(swarmTask);
            _tasks.TryAdd(riskTask.Id, riskTask);
            agentTasks.Add(riskTask);
            taskContext.PublishedTasks.Add(riskTask);

            await PublishTaskEventAsync(riskTask, cancellationToken);
        }

        _taskContexts.TryAdd(swarmTask.Id, taskContext);
        
        _logger.LogInformation("Published {TaskCount} task events for swarm task {TaskId}", 
            agentTasks.Count, swarmTask.Id);

        return agentTasks;
    }

    public async Task<AgentTask> GetNextTaskAsync(string agentId, CancellationToken cancellationToken = default)
    {
        var availableTasks = _tasks.Values
            .Where(t => t.Status == SwarmTaskStatus.Pending)
            .OrderBy(t => t.Priority)
            .ToList();

        foreach (var task in availableTasks)
        {
            if (await CanAgentHandleTaskAsync(agentId, task, cancellationToken))
            {
                task.Status = SwarmTaskStatus.InProgress;
                task.StartedAt = DateTime.UtcNow;
                
                _logger.LogInformation("Assigned task {TaskId} to agent {AgentId}", task.Id, agentId);
                return task;
            }
        }

        return null;
    }

    public async Task UpdateTaskStatusAsync(string taskId, SwarmTaskStatus status, CancellationToken cancellationToken = default)
    {
        if (_tasks.TryGetValue(taskId, out var task))
        {
            task.Status = status;
            
            if (status == SwarmTaskStatus.Completed)
            {
                task.CompletedAt = DateTime.UtcNow;
                
                var completedEvent = new TaskCompletedEvent
                {
                    SourceAgentId = "task_distributor",
                    TaskId = taskId,
                    CompletedByRole = task.Role,
                    Result = "Task completed successfully",
                    Confidence = 1.0
                };

                await _eventBus.PublishAsync(completedEvent, cancellationToken);
                
                _logger.LogInformation("Task {TaskId} completed and event published", taskId);
            }
            else if (status == SwarmTaskStatus.Failed)
            {
                await HandleTaskFailureAsync(task, cancellationToken);
                _logger.LogWarning("Task {TaskId} failed", taskId);
            }
        }
    }

    private AgentTask CreateAgentTask(SwarmTask swarmTask, AgentRole role)
    {
        return new AgentTask
        {
            SwarmTaskId = swarmTask.Id,
            Role = role,
            Prompt = BuildRoleSpecificPrompt(swarmTask, role),
            Context = new Dictionary<string, object>(swarmTask.Parameters),
            Dependencies = GetTaskDependencies(role, swarmTask.RequiredRoles),
            Status = SwarmTaskStatus.Pending,
            Priority = CalculateTaskPriority(role, swarmTask)
        };
    }

    private AgentTask CreateRiskAssessmentTask(SwarmTask swarmTask)
    {
        return new AgentTask
        {
            SwarmTaskId = swarmTask.Id,
            Role = AgentRole.RiskAssessment,
            Prompt = BuildRiskAssessmentPrompt(swarmTask),
            Context = new Dictionary<string, object>(swarmTask.Parameters),
            Dependencies = new List<string>(),
            Status = SwarmTaskStatus.Pending,
            Priority = 7
        };
    }

    private async Task PublishTaskEventAsync(AgentTask agentTask, CancellationToken cancellationToken)
    {
        var taskEvent = new TaskAvailableEvent
        {
            SourceAgentId = "task_distributor",
            RequiredRole = agentTask.Role,
            TaskDescription = agentTask.Prompt,
            Priority = agentTask.Priority,
            Payload = new Dictionary<string, object>
            {
                ["task_id"] = agentTask.Id,
                ["swarm_task_id"] = agentTask.SwarmTaskId,
                ["dependencies"] = agentTask.Dependencies,
                ["context"] = agentTask.Context
            }
        };

        await _eventBus.PublishAsync(taskEvent, cancellationToken);
        
        _logger.LogDebug("Published task event for {Role}: {TaskId}", agentTask.Role, agentTask.Id);
    }

    private async Task<bool> CanAgentHandleTaskAsync(string agentId, AgentTask task, CancellationToken cancellationToken)
    {
        if (task.Dependencies.Any())
        {
            var dependenciesSatisfied = task.Dependencies.All(depId => 
                _tasks.Values.Any(t => t.Id == depId && t.Status == SwarmTaskStatus.Completed));
            
            if (!dependenciesSatisfied)
                return false;
        }

        return await Task.FromResult(true);
    }

    private async Task HandleTaskFailureAsync(AgentTask task, CancellationToken cancellationToken)
    {
        task.RetryCount++;
        
        if (task.RetryCount < 3)
        {
            task.Status = SwarmTaskStatus.Pending;
            task.Priority += 1;
            
            await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, task.RetryCount)), cancellationToken);
            await PublishTaskEventAsync(task, cancellationToken);
            
            _logger.LogInformation("Retrying task {TaskId} (attempt {RetryCount})", task.Id, task.RetryCount);
        }
        else
        {
            _logger.LogError("Task {TaskId} failed after {RetryCount} attempts", task.Id, task.RetryCount);
        }
    }

    private string BuildRoleSpecificPrompt(SwarmTask swarmTask, AgentRole role)
    {
        var basePrompt = swarmTask.Description;
        
        return role switch
        {
            AgentRole.Orchestrator => $"[Event-Driven] Coordinate workflow for: {basePrompt}",
            AgentRole.PRExtractor => $"[Event-Driven] Extract PR data for: {basePrompt}",
            AgentRole.JiraContext => $"[Event-Driven] Gather business context for: {basePrompt}",
            AgentRole.CodeAnalyzer => $"[Event-Driven] Analyze code changes for: {basePrompt}",
            AgentRole.RequirementMapper => $"[Event-Driven] Map requirements for: {basePrompt}",
            AgentRole.TestCoverage => $"[Event-Driven] Analyze test coverage for: {basePrompt}",
            AgentRole.RiskAssessment => $"[Event-Driven] Assess risks for: {basePrompt}",
            AgentRole.IntegrationAnalyzer => $"[Event-Driven] Analyze integration for: {basePrompt}",
            AgentRole.SummaryGenerator => $"[Event-Driven] Generate summary for: {basePrompt}",
            AgentRole.Learning => $"[Event-Driven] Learn from feedback for: {basePrompt}",
            _ => basePrompt
        };
    }

    private string BuildRiskAssessmentPrompt(SwarmTask swarmTask)
    {
        return $"[Event-Driven] Assess risks and potential issues for: {swarmTask.Description}. " +
               "Identify breaking changes, security vulnerabilities, performance risks, and integration issues.";
    }

    private List<string> GetTaskDependencies(AgentRole role, AgentRole[] allRoles)
    {
        var dependencies = new List<string>();
        
        if (role == AgentRole.SummaryGenerator)
        {
            foreach (var otherRole in allRoles.Where(r => r != AgentRole.SummaryGenerator))
            {
                dependencies.Add(otherRole.ToString());
            }
        }

        return dependencies;
    }

    private int CalculateTaskPriority(AgentRole role, SwarmTask swarmTask)
    {
        return role switch
        {
            AgentRole.PRExtractor => 1,
            AgentRole.JiraContext => 2,
            AgentRole.CodeAnalyzer => 3,
            AgentRole.RequirementMapper => 4,
            AgentRole.TestCoverage => 5,
            AgentRole.IntegrationAnalyzer => 6,
            AgentRole.RiskAssessment => 7,
            AgentRole.SummaryGenerator => 8,
            AgentRole.Learning => 9,
            _ => 5
        };
    }

    private bool ShouldIncludeRiskAssessment(SwarmTask swarmTask)
    {
        return swarmTask.RequiredRoles.Length > 2 && 
               !swarmTask.RequiredRoles.Contains(AgentRole.RiskAssessment);
    }

    private class TaskPublicationContext
    {
        public SwarmTask SwarmTask { get; set; } = null!;
        public List<AgentTask> PublishedTasks { get; set; } = new();
        public List<AgentRole> PendingRoles { get; set; } = new();
    }
}