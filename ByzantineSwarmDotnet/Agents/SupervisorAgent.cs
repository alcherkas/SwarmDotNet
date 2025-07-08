using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;
using ByzantineSwarmDotnet.Models;
using ByzantineSwarmDotnet.Interfaces;

namespace ByzantineSwarmDotnet.Agents;

public class SupervisorAgent : EnhancedSwarmAgentBase, ISupervisorAgent
{
    private readonly ILoadBalancer _loadBalancer;
    private readonly List<string> _subordinateIds;

    public SupervisorAgent(
        string id,
        Kernel kernel,
        ILogger<SupervisorAgent> logger,
        ICryptographicSigner signer,
        IByzantineDetector byzantineDetector,
        IReputationSystem reputationSystem,
        IFaultTolerantCommunicationHub communicationHub,
        ILoadBalancer loadBalancer) 
        : base(id, AgentRole.Supervisor, AgentHierarchyLevel.Supervisor, kernel, logger, signer, byzantineDetector, reputationSystem, communicationHub)
    {
        _loadBalancer = loadBalancer;
        _subordinateIds = new List<string>();
    }

    public override async Task<AgentResponse> ProcessTaskAsync(AgentTask task)
    {
        try
        {
            _logger.LogInformation("SupervisorAgent {AgentId} processing task {TaskId}", Id, task.Id);
            
            var startTime = DateTime.UtcNow;
            
            // Decompose task into subtasks
            var subtasks = await DecomposeTaskIntoSubtasksAsync(task);
            
            // Execute subtasks with consensus
            var responses = await ExecuteSubtasksWithConsensusAsync(subtasks);
            
            // Aggregate responses
            var finalResponse = await AggregateResponsesWithValidationAsync(responses);
            
            var executionTime = DateTime.UtcNow - startTime;
            UpdateMetric("LastExecutionTime", executionTime);
            UpdateMetric("TasksCompleted", (int)(GetMetricsAsync().Result.GetValueOrDefault("TasksCompleted", 0)) + 1);
            
            return finalResponse with 
            { 
                ExecutionTime = executionTime,
                Signature = await SignMessageAsync(new AgentMessage { Response = finalResponse })
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing task {TaskId} in SupervisorAgent {AgentId}", task.Id, Id);
            return new AgentResponse
            {
                TaskId = task.Id,
                AgentId = Id,
                Success = false,
                ErrorMessage = ex.Message,
                Timestamp = DateTime.UtcNow
            };
        }
    }

    public async Task<AgentTask> DecomposeTaskAsync(SwarmTask task)
    {
        try
        {
            var prompt = $@"
You are a supervisor agent responsible for decomposing complex tasks.
Analyze the following task and break it down into smaller, manageable subtasks:

Task: {task.Description}
Required Roles: {string.Join(", ", task.RequiredRoles)}
Security Level: {task.SecurityLevel}
Fault Tolerance Level: {task.FaultToleranceLevel}

Please decompose this task into 3-5 specific subtasks that can be executed in parallel or sequence.
Focus on creating clear, actionable subtasks with defined inputs and expected outputs.
";

            var result = await _kernel.InvokePromptAsync(prompt);
            
            return new AgentTask
            {
                Id = Guid.NewGuid().ToString(),
                SwarmTaskId = task.Id,
                AssignedAgentId = Id,
                Type = TaskType.General,
                Description = result.ToString(),
                Parameters = new Dictionary<string, object> { ["decomposition"] = result.ToString() }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error decomposing task {TaskId}", task.Id);
            throw;
        }
    }

    public async Task<List<AgentResponse>> ExecuteSubtasksWithConsensusAsync(List<AgentTask> subtasks)
    {
        var responses = new List<AgentResponse>();
        
        foreach (var subtask in subtasks)
        {
            try
            {
                // Select multiple agents for Byzantine fault tolerance
                var selectedAgents = await SelectAgentsForSubtaskAsync(subtask);
                
                // Execute subtask with multiple agents
                var candidateResponses = await ExecuteWithMultipleAgentsAsync(subtask, selectedAgents);
                
                // Reach consensus on the best response
                var consensusResponse = await ReachConsensusOnResponseAsync(candidateResponses);
                
                responses.Add(consensusResponse);
                
                _logger.LogInformation("Subtask {SubtaskId} completed with consensus", subtask.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing subtask {SubtaskId}", subtask.Id);
                responses.Add(new AgentResponse
                {
                    TaskId = subtask.Id,
                    AgentId = Id,
                    Success = false,
                    ErrorMessage = ex.Message
                });
            }
        }
        
        return responses;
    }

    public async Task<AgentResponse> AggregateResponsesWithValidationAsync(List<AgentResponse> responses)
    {
        try
        {
            var successfulResponses = responses.Where(r => r.Success).ToList();
            
            if (!successfulResponses.Any())
            {
                return new AgentResponse
                {
                    AgentId = Id,
                    Success = false,
                    ErrorMessage = "No successful responses to aggregate",
                    Timestamp = DateTime.UtcNow
                };
            }

            // Create aggregation prompt
            var aggregationPrompt = $@"
You are a supervisor agent responsible for aggregating results from multiple subtasks.
Analyze the following responses and create a comprehensive final result:

Responses:
{string.Join("\n---\n", successfulResponses.Select(r => $"Agent: {r.AgentId}\nResult: {r.Result}\nConfidence: {r.ConfidenceScore:F2}"))}

Please provide a well-structured, comprehensive result that combines the insights from all responses.
Highlight any consensus or conflicts between responses.
";

            var aggregatedResult = await _kernel.InvokePromptAsync(aggregationPrompt);
            
            var avgConfidence = successfulResponses.Average(r => r.ConfidenceScore);
            var totalExecutionTime = responses.Max(r => r.ExecutionTime);
            
            return new AgentResponse
            {
                AgentId = Id,
                Success = true,
                Result = aggregatedResult.ToString(),
                ConfidenceScore = avgConfidence,
                ExecutionTime = totalExecutionTime,
                Timestamp = DateTime.UtcNow,
                Metadata = new Dictionary<string, object>
                {
                    ["AggregatedResponses"] = successfulResponses.Count,
                    ["TotalResponses"] = responses.Count,
                    ["AverageConfidence"] = avgConfidence
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error aggregating responses");
            throw;
        }
    }

    private async Task<List<AgentTask>> DecomposeTaskIntoSubtasksAsync(AgentTask task)
    {
        // Simplified decomposition - in real implementation would use AI
        var subtasks = new List<AgentTask>();
        
        // Create 3 parallel subtasks by default
        for (int i = 0; i < 3; i++)
        {
            subtasks.Add(new AgentTask
            {
                Id = Guid.NewGuid().ToString(),
                SwarmTaskId = task.SwarmTaskId,
                Type = TaskType.General,
                Description = $"Subtask {i + 1} of {task.Description}",
                Parameters = new Dictionary<string, object>(task.Parameters)
                {
                    ["subtask_index"] = i,
                    ["total_subtasks"] = 3
                }
            });
        }
        
        return subtasks;
    }

    private async Task<List<string>> SelectAgentsForSubtaskAsync(AgentTask subtask)
    {
        // For Byzantine fault tolerance, select at least 3 agents
        var requiredAgents = Math.Max(3, (int)Math.Ceiling(_subordinateIds.Count * 0.3));
        
        var selectedAgents = new List<string>();
        foreach (var agentId in _subordinateIds.Take(requiredAgents))
        {
            var trustScore = await CalculateTrustScoreAsync(agentId);
            if (trustScore.IsTrusted)
            {
                selectedAgents.Add(agentId);
            }
        }
        
        return selectedAgents;
    }

    private async Task<List<AgentResponse>> ExecuteWithMultipleAgentsAsync(AgentTask subtask, List<string> selectedAgents)
    {
        var responses = new List<AgentResponse>();
        var tasks = new List<Task<AgentResponse>>();
        
        // Delegate subtask to multiple agents
        foreach (var agentId in selectedAgents)
        {
            await DelegateTaskAsync(subtask, new List<string> { agentId });
        }
        
        // Wait for responses (simplified - in real implementation would handle timeouts)
        await Task.Delay(TimeSpan.FromSeconds(10));
        
        // Simulate responses for demo purposes
        foreach (var agentId in selectedAgents)
        {
            responses.Add(new AgentResponse
            {
                TaskId = subtask.Id,
                AgentId = agentId,
                Success = true,
                Result = $"Response from {agentId} for subtask {subtask.Id}",
                ConfidenceScore = 0.8 + (new Random().NextDouble() * 0.2),
                Timestamp = DateTime.UtcNow
            });
        }
        
        return responses;
    }

    private async Task<AgentResponse> ReachConsensusOnResponseAsync(List<AgentResponse> candidateResponses)
    {
        if (!candidateResponses.Any())
        {
            throw new InvalidOperationException("No candidate responses for consensus");
        }

        // Simple consensus: select response with highest confidence from most trusted agent
        var bestResponse = candidateResponses
            .Where(r => r.Success)
            .OrderByDescending(r => r.ConfidenceScore)
            .FirstOrDefault();

        if (bestResponse == null)
        {
            bestResponse = candidateResponses.First();
        }

        return bestResponse with
        {
            Metadata = bestResponse.Metadata.Union(new Dictionary<string, object>
            {
                ["ConsensusReached"] = true,
                ["CandidateResponses"] = candidateResponses.Count,
                ["ConsensusConfidence"] = candidateResponses.Average(r => r.ConfidenceScore)
            }).ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
        };
    }

    public void AddSubordinate(string agentId)
    {
        if (!_subordinateIds.Contains(agentId))
        {
            _subordinateIds.Add(agentId);
        }
    }

    public void RemoveSubordinate(string agentId)
    {
        _subordinateIds.Remove(agentId);
    }
}
