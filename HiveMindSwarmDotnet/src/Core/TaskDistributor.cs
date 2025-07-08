using System.Collections.Concurrent;
using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;

namespace HiveMindSwarmDotnet.Console.Core;

public class TaskDistributor : ITaskDistributor
{
    private readonly ILogger<TaskDistributor> _logger;
    private readonly ConcurrentDictionary<string, AgentTask> _tasks = new();
    private readonly ConcurrentDictionary<string, Queue<AgentTask>> _agentQueues = new();
    private readonly ConcurrentDictionary<string, List<AgentTask>> _taskDependencies = new();

    public TaskDistributor(ILogger<TaskDistributor> logger)
    {
        _logger = logger;
    }

    public async Task<List<AgentTask>> DistributeTaskAsync(SwarmTask swarmTask, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Distributing swarm task {TaskId} with {RoleCount} required roles", 
            swarmTask.Id, swarmTask.RequiredRoles.Length);

        var agentTasks = new List<AgentTask>();
        var taskGraph = BuildTaskGraph(swarmTask);

        foreach (var role in swarmTask.RequiredRoles)
        {
            var agentTask = new AgentTask
            {
                SwarmTaskId = swarmTask.Id,
                Role = role,
                Prompt = BuildRoleSpecificPrompt(swarmTask, role),
                Context = new Dictionary<string, object>(swarmTask.Parameters),
                Dependencies = GetTaskDependencies(role, swarmTask.RequiredRoles),
                Status = SwarmTaskStatus.Pending
            };

            _tasks.TryAdd(agentTask.Id, agentTask);
            agentTasks.Add(agentTask);
        }

        if (ShouldIncludeRiskAssessment(swarmTask))
        {
            var riskAssessmentTask = new AgentTask
            {
                SwarmTaskId = swarmTask.Id,
                Role = AgentRole.RiskAssessment,
                Prompt = BuildRiskAssessmentPrompt(swarmTask),
                Context = new Dictionary<string, object>(swarmTask.Parameters),
                Dependencies = agentTasks.Select(t => t.Id).ToList(),
                Status = SwarmTaskStatus.Pending
            };

            _tasks.TryAdd(riskAssessmentTask.Id, riskAssessmentTask);
            agentTasks.Add(riskAssessmentTask);
        }

        await OptimizeTaskOrderAsync(agentTasks, cancellationToken);
        
        _logger.LogInformation("Created {TaskCount} agent tasks for swarm task {TaskId}", 
            agentTasks.Count, swarmTask.Id);

        return await Task.FromResult(agentTasks);
    }

    public async Task<AgentTask> GetNextTaskAsync(string agentId, CancellationToken cancellationToken = default)
    {
        if (!_agentQueues.TryGetValue(agentId, out var queue) || queue.Count == 0)
        {
            return null;
        }

        while (queue.Count > 0)
        {
            var task = queue.Dequeue();
            
            if (AreTaskDependenciesSatisfied(task))
            {
                task.Status = SwarmTaskStatus.InProgress;
                task.StartedAt = DateTime.UtcNow;
                
                _logger.LogInformation("Assigned task {TaskId} to agent {AgentId}", task.Id, agentId);
                return await Task.FromResult(task);
            }
            
            queue.Enqueue(task);
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
                _logger.LogInformation("Task {TaskId} completed", taskId);
            }
            else if (status == SwarmTaskStatus.Failed)
            {
                _logger.LogWarning("Task {TaskId} failed", taskId);
            }
        }

        await Task.CompletedTask;
    }

    private Dictionary<string, List<string>> BuildTaskGraph(SwarmTask swarmTask)
    {
        var graph = new Dictionary<string, List<string>>();
        
        foreach (var role in swarmTask.RequiredRoles)
        {
            graph[role.ToString()] = new List<string>();
        }

        if (swarmTask.RequiredRoles.Contains(AgentRole.CodeAnalyzer) && 
            swarmTask.RequiredRoles.Contains(AgentRole.RiskAssessment))
        {
            graph[AgentRole.RiskAssessment.ToString()].Add(AgentRole.CodeAnalyzer.ToString());
        }

        if (swarmTask.RequiredRoles.Contains(AgentRole.SummaryGenerator))
        {
            foreach (var role in swarmTask.RequiredRoles.Where(r => r != AgentRole.SummaryGenerator))
            {
                graph[AgentRole.SummaryGenerator.ToString()].Add(role.ToString());
            }
        }

        return graph;
    }

    private string BuildRoleSpecificPrompt(SwarmTask swarmTask, AgentRole role)
    {
        var basePrompt = swarmTask.Description;
        
        return role switch
        {
            AgentRole.Orchestrator => $"Coordinate and manage the overall workflow for: {basePrompt}",
            AgentRole.PRExtractor => $"Extract and analyze pull request data for: {basePrompt}",
            AgentRole.JiraContext => $"Gather business context and requirements for: {basePrompt}",
            AgentRole.CodeAnalyzer => $"Analyze code changes and technical implementation for: {basePrompt}",
            AgentRole.RequirementMapper => $"Map code changes to business requirements for: {basePrompt}",
            AgentRole.TestCoverage => $"Analyze test coverage and quality for: {basePrompt}",
            AgentRole.RiskAssessment => $"Assess risks and potential issues for: {basePrompt}",
            AgentRole.IntegrationAnalyzer => $"Analyze integration and system dependencies for: {basePrompt}",
            AgentRole.SummaryGenerator => $"Generate comprehensive summary and insights for: {basePrompt}",
            AgentRole.Learning => $"Learn and adapt from feedback for: {basePrompt}",
            _ => basePrompt
        };
    }

    private string BuildRiskAssessmentPrompt(SwarmTask swarmTask)
    {
        return $"Assess risks and potential issues for the following problem: {swarmTask.Description}. " +
               "Identify breaking changes, security vulnerabilities, performance risks, and integration issues.";
    }

    private List<string> GetTaskDependencies(AgentRole role, AgentRole[] allRoles)
    {
        var dependencies = new List<string>();
        
        if (role == AgentRole.RiskAssessment && allRoles.Contains(AgentRole.CodeAnalyzer))
        {
            dependencies.Add(AgentRole.CodeAnalyzer.ToString());
        }
        
        if (role == AgentRole.SummaryGenerator)
        {
            foreach (var otherRole in allRoles.Where(r => r != AgentRole.SummaryGenerator && r != AgentRole.RiskAssessment))
            {
                dependencies.Add(otherRole.ToString());
            }
        }

        return dependencies;
    }

    private bool ShouldIncludeRiskAssessment(SwarmTask swarmTask)
    {
        return swarmTask.RequiredRoles.Length > 1 && 
               !swarmTask.RequiredRoles.Contains(AgentRole.RiskAssessment);
    }

    private bool AreTaskDependenciesSatisfied(AgentTask task)
    {
        if (!task.Dependencies.Any())
            return true;

        return task.Dependencies.All(depId => 
            _tasks.Values.Any(t => t.Id == depId && t.Status == SwarmTaskStatus.Completed));
    }

    private async Task OptimizeTaskOrderAsync(List<AgentTask> tasks, CancellationToken cancellationToken)
    {
        var sortedTasks = tasks.OrderBy(t => t.Dependencies.Count).ToList();
        
        foreach (var task in sortedTasks)
        {
            if (!_agentQueues.ContainsKey(task.Role.ToString()))
            {
                _agentQueues[task.Role.ToString()] = new Queue<AgentTask>();
            }
            
            _agentQueues[task.Role.ToString()].Enqueue(task);
        }

        await Task.CompletedTask;
    }
}