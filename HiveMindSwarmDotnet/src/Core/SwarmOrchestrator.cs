using System.Collections.Concurrent;
using System.Diagnostics;
using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;

namespace HiveMindSwarmDotnet.Console.Core;

public class SwarmOrchestrator : ISwarmOrchestrator
{
    private readonly IAgentPool _agentPool;
    private readonly ITaskDistributor _taskDistributor;
    private readonly ICommunicationHub _communicationHub;
    private readonly IOptimizationEngine _optimizationEngine;
    private readonly ISwarmConfiguration _configuration;
    private readonly ILogger<SwarmOrchestrator> _logger;
    private readonly ConcurrentDictionary<string, SwarmMetrics> _metrics = new();

    public SwarmOrchestrator(
        IAgentPool agentPool,
        ITaskDistributor taskDistributor,
        ICommunicationHub communicationHub,
        IOptimizationEngine optimizationEngine,
        ISwarmConfiguration configuration,
        ILogger<SwarmOrchestrator> logger)
    {
        _agentPool = agentPool;
        _taskDistributor = taskDistributor;
        _communicationHub = communicationHub;
        _optimizationEngine = optimizationEngine;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Initializing Swarm Orchestrator");
        await Task.CompletedTask;
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
            _logger.LogInformation("Starting execution of swarm task {TaskId}: {Description}", 
                task.Id, task.Description);

            var agentTasks = await _taskDistributor.DistributeTaskAsync(task, cancellationToken);
            
            var executionResults = await ExecuteAgentTasksAsync(agentTasks, cancellationToken);
            
            swarmResult.AgentResponses = executionResults.responses;
            swarmResult.Communications = executionResults.communications;
            
            await OptimizeSwarmPerformanceAsync(swarmResult, cancellationToken);
            
            swarmResult.FinalAnswer = await SynthesizeResultsAsync(swarmResult.AgentResponses, cancellationToken);
            swarmResult.OverallConfidence = CalculateOverallConfidence(swarmResult.AgentResponses);
            
            UpdateMetrics(task, swarmResult);
            
            _logger.LogInformation("Completed swarm task {TaskId} in {ElapsedMs}ms with confidence {Confidence}", 
                task.Id, stopwatch.ElapsedMilliseconds, swarmResult.OverallConfidence);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing swarm task {TaskId}", task.Id);
            swarmResult.IsSuccessful = false;
            swarmResult.Errors.Add(ex.Message);
        }
        finally
        {
            stopwatch.Stop();
            swarmResult.ExecutionTime = stopwatch.Elapsed;
        }

        return swarmResult;
    }

    private async Task<(List<AgentResponse> responses, List<AgentMessage> communications)> ExecuteAgentTasksAsync(
        List<AgentTask> agentTasks, 
        CancellationToken cancellationToken)
    {
        var responses = new List<AgentResponse>();
        var communications = new List<AgentMessage>();
        var completedTasks = new HashSet<string>();
        
        var maxIterations = agentTasks.Count * 2;
        var iteration = 0;

        while (completedTasks.Count < agentTasks.Count && iteration < maxIterations)
        {
            var availableTasks = agentTasks.Where(t => 
                !completedTasks.Contains(t.Id) && 
                t.Status == SwarmTaskStatus.Pending &&
                AreTaskDependenciesSatisfied(t.Id, completedTasks)).ToList();

            if (!availableTasks.Any())
            {
                await Task.Delay(100, cancellationToken);
                iteration++;
                continue;
            }

            var executionTasks = availableTasks.Select(async agentTask =>
            {
                try
                {
                    var agent = await _agentPool.GetAgentAsync(agentTask.Role, cancellationToken);
                    if (agent == null)
                    {
                        _logger.LogWarning("No agent available for role {Role}", agentTask.Role);
                        return null;
                    }

                    var response = await agent.ProcessTaskAsync(agentTask, cancellationToken);
                    
                    await _taskDistributor.UpdateTaskStatusAsync(agentTask.Id, SwarmTaskStatus.Completed, cancellationToken);
                    await _agentPool.ReturnAgentAsync(agent, cancellationToken);
                    
                    await FacilitateCommunicationAsync(response, communications, cancellationToken);
                    
                    return response;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing agent task {TaskId}", agentTask.Id);
                    await _taskDistributor.UpdateTaskStatusAsync(agentTask.Id, SwarmTaskStatus.Failed, cancellationToken);
                    return null;
                }
            });

            var batchResults = await Task.WhenAll(executionTasks);
            
            foreach (var result in batchResults.Where(r => r != null))
            {
                responses.Add(result);
                completedTasks.Add(result.TaskId);
            }

            iteration++;
        }

        return (responses, communications);
    }

    private async Task FacilitateCommunicationAsync(
        AgentResponse response, 
        List<AgentMessage> communications, 
        CancellationToken cancellationToken)
    {
        if (response.Role == AgentRole.RiskAssessment)
        {
            var riskMessage = new AgentMessage
            {
                FromAgentId = response.AgentId,
                Content = $"Risk Assessment: {response.Content}",
                Type = MessageType.Request
            };

            await _communicationHub.BroadcastAsync(response.AgentId, riskMessage, cancellationToken);
            communications.Add(riskMessage);
        }
        else if (response.Role == AgentRole.TestCoverage)
        {
            var testMessage = new AgentMessage
            {
                FromAgentId = response.AgentId,
                Content = $"Test Coverage: {response.Content}",
                Type = MessageType.Validation
            };

            await _communicationHub.BroadcastAsync(response.AgentId, testMessage, cancellationToken);
            communications.Add(testMessage);
        }
    }

    private async Task OptimizeSwarmPerformanceAsync(SwarmResult result, CancellationToken cancellationToken)
    {
        try
        {
            var performanceData = await _optimizationEngine.CalculateAgentPerformanceAsync(
                result.AgentResponses, cancellationToken);

            foreach (var (role, performance) in performanceData)
            {
                result.Metrics[$"{role}_performance"] = performance;
            }

            if (_configuration.OptimizationStrategy == OptimizationStrategy.Lamarckian)
            {
                await ApplyLamarckianOptimizationAsync(result, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during swarm optimization");
        }
    }

    private async Task ApplyLamarckianOptimizationAsync(SwarmResult result, CancellationToken cancellationToken)
    {
        var lowPerformanceResponses = result.AgentResponses
            .Where(r => r.Confidence < 0.6)
            .ToList();

        foreach (var response in lowPerformanceResponses)
        {
            var improvementData = new Dictionary<string, object>
            {
                ["low_confidence"] = response.Confidence,
                ["validation_errors"] = response.ValidationErrors.Count,
                ["task_complexity"] = response.Content.Length
            };

            await _optimizationEngine.AdaptAgentBehaviorAsync(
                response.AgentId, 
                improvementData, 
                cancellationToken);
        }
    }

    private async Task<string> SynthesizeResultsAsync(List<AgentResponse> responses, CancellationToken cancellationToken)
    {
        var summaryResponse = responses.FirstOrDefault(r => r.Role == AgentRole.SummaryGenerator);
        if (summaryResponse != null)
        {
            return summaryResponse.Content;
        }

        var highConfidenceResponses = responses
            .Where(r => r.Confidence > 0.7)
            .OrderByDescending(r => r.Confidence)
            .Take(3)
            .ToList();

        if (highConfidenceResponses.Any())
        {
            var synthesis = "Combined Analysis:\n\n";
            foreach (var response in highConfidenceResponses)
            {
                synthesis += $"From {response.Role}: {response.Content}\n\n";
            }
            return synthesis;
        }

        return responses.FirstOrDefault()?.Content ?? "No valid responses generated";
    }

    private double CalculateOverallConfidence(List<AgentResponse> responses)
    {
        if (!responses.Any())
            return 0.0;

        var validResponses = responses.Where(r => r.IsValid).ToList();
        if (!validResponses.Any())
            return 0.0;

        var weightedConfidence = validResponses.Sum(r => r.Confidence * GetRoleWeight(r.Role));
        var totalWeight = validResponses.Sum(r => GetRoleWeight(r.Role));

        return weightedConfidence / totalWeight;
    }

    private double GetRoleWeight(AgentRole role)
    {
        return role switch
        {
            AgentRole.Orchestrator => 1.4,
            AgentRole.PRExtractor => 1.0,
            AgentRole.JiraContext => 1.1,
            AgentRole.CodeAnalyzer => 1.2,
            AgentRole.RequirementMapper => 1.1,
            AgentRole.TestCoverage => 1.2,
            AgentRole.RiskAssessment => 1.3,
            AgentRole.IntegrationAnalyzer => 1.1,
            AgentRole.SummaryGenerator => 1.4,
            AgentRole.Learning => 1.0,
            _ => 1.0
        };
    }

    private bool AreTaskDependenciesSatisfied(string taskId, HashSet<string> completedTasks)
    {
        return true;
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

    public async Task<SwarmMetrics> GetMetricsAsync()
    {
        var overallMetrics = new SwarmMetrics
        {
            SwarmId = "overall",
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
        _logger.LogInformation("Disposing Swarm Orchestrator");
        await Task.CompletedTask;
    }
}