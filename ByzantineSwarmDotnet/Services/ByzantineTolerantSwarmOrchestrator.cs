using Microsoft.Extensions.Logging;
using ByzantineSwarmDotnet.Models;
using ByzantineSwarmDotnet.Interfaces;
using ByzantineSwarmDotnet.Configuration;

namespace ByzantineSwarmDotnet.Services;

public class ByzantineTolerantSwarmOrchestrator : IByzantineTolerantSwarmOrchestrator
{
    private readonly ILogger<ByzantineTolerantSwarmOrchestrator> _logger;
    private readonly IHierarchicalAgentPool _agentPool;
    private readonly IByzantineTaskDistributor _taskDistributor;
    private readonly IFaultTolerantCommunicationHub _communicationHub;
    private readonly IPBFTConsensusEngine _consensusEngine;
    private readonly IAdvancedFaultDetector _faultDetector;
    private readonly IReputationSystem _reputationSystem;
    private readonly ICircuitBreakerRegistry _circuitBreakers;
    private readonly SwarmConfiguration _configuration;
    private readonly Dictionary<string, SwarmTask> _activeTasks;
    private readonly Dictionary<string, SwarmResult> _taskResults;
    private readonly object _lockObject = new();

    public ByzantineTolerantSwarmOrchestrator(
        ILogger<ByzantineTolerantSwarmOrchestrator> logger,
        IHierarchicalAgentPool agentPool,
        IByzantineTaskDistributor taskDistributor,
        IFaultTolerantCommunicationHub communicationHub,
        IPBFTConsensusEngine consensusEngine,
        IAdvancedFaultDetector faultDetector,
        IReputationSystem reputationSystem,
        ICircuitBreakerRegistry circuitBreakers,
        SwarmConfiguration configuration)
    {
        _logger = logger;
        _agentPool = agentPool;
        _taskDistributor = taskDistributor;
        _communicationHub = communicationHub;
        _consensusEngine = consensusEngine;
        _faultDetector = faultDetector;
        _reputationSystem = reputationSystem;
        _circuitBreakers = circuitBreakers;
        _configuration = configuration;
        _activeTasks = new Dictionary<string, SwarmTask>();
        _taskResults = new Dictionary<string, SwarmResult>();

        // Start background monitoring
        _ = Task.Run(async () => await StartBackgroundMonitoringAsync());
    }

    public async Task<SwarmResult> ExecuteTaskAsync(SwarmTask task)
    {
        var startTime = DateTime.UtcNow;
        _logger.LogInformation("Starting execution of task {TaskId}: {Description}", task.Id, task.Description);

        try
        {
            // Register task as active
            lock (_lockObject)
            {
                _activeTasks[task.Id] = task;
            }

            // Step 1: Validate Byzantine fault tolerance requirements
            await ValidateByzantineToleranceRequirements();

            // Step 2: Execute PBFT consensus for task acceptance (if required)
            if (task.RequiredConsensus)
            {
                var taskProposal = new Proposal
                {
                    Type = ProposalType.TaskExecution,
                    ProposerId = "SwarmOrchestrator",
                    SequenceNumber = DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                    Data = new Dictionary<string, object>
                    {
                        ["task"] = task,
                        ["timestamp"] = DateTime.UtcNow
                    }
                };

                var consensus = await _consensusEngine.ReachConsensusAsync(taskProposal);
                if (!consensus.IsAccepted)
                {
                    throw new ConsensusFailedException($"Task {task.Id} rejected by consensus");
                }

                _logger.LogInformation("Task {TaskId} accepted by consensus with {Votes} votes", 
                    task.Id, consensus.AcceptVotes);
            }

            // Step 3: Create fault-tolerant execution plan
            var distributedTasks = await _taskDistributor.DistributeTaskAsync(task);
            _logger.LogInformation("Task {TaskId} distributed into {SubtaskCount} subtasks", 
                task.Id, distributedTasks.Count);

            // Step 4: Execute with Byzantine fault tolerance
            var result = await ExecuteWithByzantineToleranceAsync(task, distributedTasks);

            // Step 5: Store result and clean up
            var executionTime = DateTime.UtcNow - startTime;
            result = result with { TotalExecutionTime = executionTime };

            lock (_lockObject)
            {
                _taskResults[task.Id] = result;
                _activeTasks.Remove(task.Id);
            }

            _logger.LogInformation("Task {TaskId} completed successfully in {Duration:F2} seconds", 
                task.Id, executionTime.TotalSeconds);

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error executing task {TaskId}", task.Id);

            lock (_lockObject)
            {
                _activeTasks.Remove(task.Id);
            }

            var errorResult = new SwarmResult
            {
                TaskId = task.Id,
                Success = false,
                ConsensusReached = false,
                TrustScore = 0,
                TotalExecutionTime = DateTime.UtcNow - startTime,
                Errors = new List<string> { ex.Message }
            };

            return errorResult;
        }
    }

    public async Task<bool> ValidateByzantineToleranceRequirements()
    {
        try
        {
            var activeAgents = await _agentPool.GetActiveAgentsAsync();
            var maxFaults = _configuration.EnhancedSwarmConfiguration.MaxByzantineFaults;
            var requiredAgents = 3 * maxFaults + 1; // Byzantine fault tolerance formula: 3f + 1

            _logger.LogInformation("Byzantine tolerance validation: {ActiveAgents} active agents, {RequiredAgents} required for {MaxFaults} faults",
                activeAgents.Count, requiredAgents, maxFaults);

            if (activeAgents.Count < requiredAgents)
            {
                _logger.LogWarning("Insufficient agents for Byzantine fault tolerance. Scaling up...");
                var additionalAgents = requiredAgents - activeAgents.Count;
                await _agentPool.ScaleUpAsync(additionalAgents);

                // Verify scaling was successful
                activeAgents = await _agentPool.GetActiveAgentsAsync();
                if (activeAgents.Count < requiredAgents)
                {
                    throw new InvalidOperationException(
                        $"Failed to scale up to required agent count. Have {activeAgents.Count}, need {requiredAgents}");
                }
            }

            // Validate agent distribution across hierarchy levels
            var supervisors = await _agentPool.GetAgentsByHierarchyLevelAsync(AgentHierarchyLevel.Supervisor);
            var workers = await _agentPool.GetAgentsByHierarchyLevelAsync(AgentHierarchyLevel.Worker);

            var requiredSupervisors = Math.Max(1, (int)(activeAgents.Count * _configuration.EnhancedSwarmConfiguration.SupervisorRatio));
            if (supervisors.Count < requiredSupervisors)
            {
                _logger.LogWarning("Insufficient supervisor agents: {Current} current, {Required} required",
                    supervisors.Count, requiredSupervisors);
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating Byzantine tolerance requirements");
            return false;
        }
    }

    public async Task<SwarmMetrics> GetRealTimeMetricsAsync()
    {
        try
        {
            var activeAgents = await _agentPool.GetActiveAgentsAsync();
            var faultyAgents = await _faultDetector.GetFaultyAgentsAsync();
            var suspiciousAgents = await _reputationSystem.GetSuspiciousAgentsAsync();

            // Calculate consensus participation
            var consensusParticipation = activeAgents.Count > 0 
                ? (double)(activeAgents.Count - faultyAgents.Count) / activeAgents.Count 
                : 0;

            // Calculate suspicious activity score
            var suspiciousActivityScore = activeAgents.Count > 0 
                ? (double)suspiciousAgents.Count / activeAgents.Count 
                : 0;

            // Calculate average response time
            var avgResponseTime = 0.0;
            foreach (var agent in activeAgents)
            {
                avgResponseTime += agent.AverageResponseTime.TotalMilliseconds;
            }
            avgResponseTime = activeAgents.Count > 0 ? avgResponseTime / activeAgents.Count : 0;

            // Count completed and failed tasks
            var taskStats = GetTaskStatistics();

            return new SwarmMetrics
            {
                ConsensusParticipation = consensusParticipation,
                SuspiciousActivityScore = suspiciousActivityScore,
                ActiveAgents = activeAgents.Count,
                AverageResponseTime = avgResponseTime,
                TasksCompleted = taskStats.Completed,
                TasksFailed = taskStats.Failed,
                SystemLoad = CalculateSystemLoad(activeAgents),
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting real-time metrics");
            return new SwarmMetrics { Timestamp = DateTime.UtcNow };
        }
    }

    public async Task InitiateRecoveryAsync()
    {
        try
        {
            _logger.LogInformation("Initiating swarm recovery procedures");

            // Step 1: Identify failed agents
            var faultyAgents = await _faultDetector.GetFaultyAgentsAsync();
            _logger.LogInformation("Identified {Count} faulty agents for recovery", faultyAgents.Count);

            // Step 2: Quarantine suspicious agents
            var suspiciousAgents = await _reputationSystem.GetSuspiciousAgentsAsync();
            foreach (var agentId in suspiciousAgents)
            {
                await _agentPool.UpdateAgentStatusAsync(agentId, false);
                _logger.LogWarning("Quarantined suspicious agent {AgentId}", agentId);
            }

            // Step 3: Scale up healthy agents
            var requiredAgents = _configuration.EnhancedSwarmConfiguration.RequiredAgents;
            var activeAgents = await _agentPool.GetActiveAgentsAsync();
            var healthyAgents = activeAgents.Count - faultyAgents.Count - suspiciousAgents.Count;

            if (healthyAgents < requiredAgents)
            {
                var additionalAgents = requiredAgents - healthyAgents;
                await _agentPool.ScaleUpAsync(additionalAgents);
                _logger.LogInformation("Scaled up {Count} additional agents for recovery", additionalAgents);
            }

            // Step 4: Reset circuit breakers
            var circuitBreakerStates = await _circuitBreakers.GetAllStatesAsync();
            foreach (var kvp in circuitBreakerStates.Where(s => s.Value == "Open"))
            {
                await _circuitBreakers.ResetAsync(kvp.Key);
                _logger.LogInformation("Reset circuit breaker: {Key}", kvp.Key);
            }

            // Step 5: Validate recovery
            await ValidateByzantineToleranceRequirements();

            _logger.LogInformation("Swarm recovery completed successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during swarm recovery");
            throw;
        }
    }

    public async Task QuarantineMaliciousAgentsAsync(List<string> maliciousAgents)
    {
        try
        {
            _logger.LogCritical("Quarantining {Count} malicious agents: {Agents}", 
                maliciousAgents.Count, string.Join(", ", maliciousAgents));

            foreach (var agentId in maliciousAgents)
            {
                // Deactivate agent
                await _agentPool.UpdateAgentStatusAsync(agentId, false);

                // Update reputation to minimum
                var quarantineUpdate = new ReputationUpdate
                {
                    AgentId = agentId,
                    UpdatedBy = "SwarmOrchestrator",
                    ScoreChange = -1.0, // Set to minimum reputation
                    Reason = "Quarantined as malicious agent",
                    Evidence = new Dictionary<string, object>
                    {
                        ["QuarantineTime"] = DateTime.UtcNow,
                        ["QuarantineReason"] = "Byzantine attack detection"
                    }
                };

                await _reputationSystem.UpdateReputationAsync(agentId, quarantineUpdate);

                _logger.LogWarning("Agent {AgentId} quarantined and reputation zeroed", agentId);
            }

            // Ensure we still have enough agents for Byzantine fault tolerance
            await ValidateByzantineToleranceRequirements();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error quarantining malicious agents");
            throw;
        }
    }

    public async Task InvestigateConsensusIssuesAsync()
    {
        try
        {
            _logger.LogInformation("Investigating consensus participation issues");

            var activeAgents = await _agentPool.GetActiveAgentsAsync();
            var metrics = await GetRealTimeMetricsAsync();

            if (metrics.ConsensusParticipation < _configuration.Monitoring.AlertThresholds.LowConsensusParticipation)
            {
                _logger.LogWarning("Low consensus participation detected: {Participation:P2}", metrics.ConsensusParticipation);

                // Identify non-participating agents
                var faultyAgents = await _faultDetector.GetFaultyAgentsAsync();
                var suspiciousAgents = await _reputationSystem.GetSuspiciousAgentsAsync();

                var problemAgents = faultyAgents.Union(suspiciousAgents).ToList();

                foreach (var agentId in problemAgents)
                {
                    _logger.LogWarning("Problem agent identified: {AgentId}", agentId);

                    // Perform detailed analysis
                    var riskScore = await CalculateAgentRiskAsync(agentId);
                    if (riskScore > 0.5)
                    {
                        await _agentPool.UpdateAgentStatusAsync(agentId, false);
                        _logger.LogWarning("High-risk agent {AgentId} deactivated (risk: {Risk:F3})", agentId, riskScore);
                    }
                }

                // Scale up if needed
                if (activeAgents.Count - problemAgents.Count < _configuration.EnhancedSwarmConfiguration.MinAgents)
                {
                    var requiredAgents = _configuration.EnhancedSwarmConfiguration.RequiredAgents - activeAgents.Count + problemAgents.Count;
                    await _agentPool.ScaleUpAsync(requiredAgents);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error investigating consensus issues");
        }
    }

    public async Task EscalateByzantineDetectionAsync()
    {
        try
        {
            _logger.LogCritical("Escalating Byzantine attack detection procedures");

            // Get all potentially malicious agents
            var suspiciousAgents = await _reputationSystem.GetSuspiciousAgentsAsync();
            var maliciousAgents = new List<string>();

            foreach (var agentId in suspiciousAgents)
            {
                var riskScore = await CalculateAgentRiskAsync(agentId);
                if (riskScore > 0.7) // High threshold for escalation
                {
                    maliciousAgents.Add(agentId);
                }
            }

            if (maliciousAgents.Any())
            {
                // Immediate quarantine
                await QuarantineMaliciousAgentsAsync(maliciousAgents);

                // Notify Byzantine attack detection
                // In a real system, this would trigger security protocols
                _logger.LogCritical("BYZANTINE ATTACK CONFIRMED: {Count} malicious agents quarantined", maliciousAgents.Count);

                // Emergency recovery procedures
                await InitiateRecoveryAsync();
            }
            else
            {
                _logger.LogInformation("No malicious agents confirmed during escalation");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error escalating Byzantine detection");
        }
    }

    private async Task<SwarmResult> ExecuteWithByzantineToleranceAsync(SwarmTask swarmTask, List<AgentTask> distributedTasks)
    {
        var circuitBreaker = _circuitBreakers.GetOrCreate($"task-execution-{swarmTask.Id}");

        return await circuitBreaker.ExecuteAsync(async () =>
        {
            var responses = new List<AgentResponse>();
            var participatingAgents = new HashSet<string>();
            var byzantineFaultsDetected = 0;

            // Execute tasks with redundancy for Byzantine fault tolerance
            foreach (var task in distributedTasks)
            {
                try
                {
                    // Execute with timeout
                    using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(_configuration.EnhancedSwarmConfiguration.TaskTimeout));

                    // For Byzantine fault tolerance, execute each task with multiple agents
                    var redundantResponses = await ExecuteTaskWithRedundancyAsync(task);
                    
                    // Reach consensus on the best response
                    var consensusResponse = await SelectBestResponseAsync(redundantResponses);
                    if (consensusResponse != null)
                    {
                        responses.Add(consensusResponse);
                        participatingAgents.Add(consensusResponse.AgentId);
                    }

                    // Detect Byzantine faults
                    byzantineFaultsDetected += await DetectByzantineFaultsInResponsesAsync(redundantResponses);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error executing task {TaskId}", task.Id);
                }
            }

            // Calculate trust and consensus metrics
            var trustScore = await CalculateTrustScoreAsync(responses);
            var consensusReached = responses.Count >= (distributedTasks.Count / 2) + 1; // Majority
            var avgReputation = await CalculateAverageReputationAsync(participatingAgents);

            return new SwarmResult
            {
                TaskId = swarmTask.Id,
                Success = consensusReached && responses.Any(r => r.Success),
                ConsensusReached = consensusReached,
                TrustScore = trustScore,
                ParticipatingAgents = participatingAgents.Count,
                ConsensusConfidence = CalculateConsensusConfidence(responses),
                AverageReputationScore = avgReputation,
                ByzantineFaultsDetected = byzantineFaultsDetected,
                Result = AggregateResults(responses),
                CompletedAt = DateTime.UtcNow,
                Metadata = new Dictionary<string, object>
                {
                    ["DistributedTasks"] = distributedTasks.Count,
                    ["SuccessfulResponses"] = responses.Count(r => r.Success),
                    ["TotalResponses"] = responses.Count
                }
            };
        });
    }

    private async Task<List<AgentResponse>> ExecuteTaskWithRedundancyAsync(AgentTask task)
    {
        // Select multiple agents for redundancy (Byzantine fault tolerance)
        var agents = await _agentPool.GetActiveAgentsAsync();
        var trustedAgents = new List<AgentInfo>();

        foreach (var agent in agents)
        {
            var trustScore = await _reputationSystem.CalculateTrustScoreAsync(agent.Id);
            if (trustScore.IsTrusted)
            {
                trustedAgents.Add(agent);
            }
        }

        // Select up to 3 agents for redundancy
        var selectedAgents = trustedAgents.Take(3).ToList();
        var responses = new List<AgentResponse>();

        // Execute task with selected agents in parallel
        var executionTasks = selectedAgents.Select(async agent =>
        {
            try
            {
                // In a real implementation, this would delegate to the actual agent
                // For now, simulate agent execution
                await Task.Delay(Random.Shared.Next(100, 1000)); // Simulate processing time

                return new AgentResponse
                {
                    TaskId = task.Id,
                    AgentId = agent.Id,
                    Success = Random.Shared.NextDouble() > 0.1, // 90% success rate
                    Result = $"Agent {agent.Id} processed task {task.Id}",
                    ExecutionTime = TimeSpan.FromMilliseconds(Random.Shared.Next(100, 1000)),
                    ConfidenceScore = 0.7 + (Random.Shared.NextDouble() * 0.3),
                    Timestamp = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing task {TaskId} with agent {AgentId}", task.Id, agent.Id);
                return new AgentResponse
                {
                    TaskId = task.Id,
                    AgentId = agent.Id,
                    Success = false,
                    ErrorMessage = ex.Message,
                    Timestamp = DateTime.UtcNow
                };
            }
        });

        var results = await Task.WhenAll(executionTasks);
        return results.ToList();
    }

    private async Task<AgentResponse?> SelectBestResponseAsync(List<AgentResponse> responses)
    {
        if (!responses.Any())
            return null;

        // Filter successful responses
        var successfulResponses = responses.Where(r => r.Success).ToList();
        if (!successfulResponses.Any())
            return responses.First(); // Return first response if none successful

        // Select response with highest confidence from most trusted agent
        AgentResponse? bestResponse = null;
        double bestScore = 0;

        foreach (var response in successfulResponses)
        {
            var trustScore = await _reputationSystem.CalculateTrustScoreAsync(response.AgentId);
            var combinedScore = (trustScore.Score * 0.6) + (response.ConfidenceScore * 0.4);

            if (combinedScore > bestScore)
            {
                bestScore = combinedScore;
                bestResponse = response;
            }
        }

        return bestResponse ?? successfulResponses.First();
    }

    private async Task<int> DetectByzantineFaultsInResponsesAsync(List<AgentResponse> responses)
    {
        var byzantineFaults = 0;

        foreach (var response in responses)
        {
            // Check for suspicious patterns
            if (response.ExecutionTime < TimeSpan.FromMilliseconds(10)) // Too fast
            {
                byzantineFaults++;
                _logger.LogWarning("Suspicious response time from agent {AgentId}: {Time}ms", 
                    response.AgentId, response.ExecutionTime.TotalMilliseconds);
            }

            if (response.ConfidenceScore > 0.99) // Suspiciously high confidence
            {
                byzantineFaults++;
                _logger.LogWarning("Suspiciously high confidence from agent {AgentId}: {Confidence}", 
                    response.AgentId, response.ConfidenceScore);
            }

            // Check agent reputation
            var trustScore = await _reputationSystem.CalculateTrustScoreAsync(response.AgentId);
            if (!trustScore.IsTrusted)
            {
                byzantineFaults++;
                _logger.LogWarning("Response from untrusted agent {AgentId}", response.AgentId);
            }
        }

        return byzantineFaults;
    }

    private async Task<double> CalculateTrustScoreAsync(List<AgentResponse> responses)
    {
        if (!responses.Any())
            return 0;

        var trustScores = new List<double>();

        foreach (var response in responses)
        {
            var agentTrust = await _reputationSystem.CalculateTrustScoreAsync(response.AgentId);
            var responseTrust = response.Success ? response.ConfidenceScore : 0;
            var combinedTrust = (agentTrust.Score * 0.7) + (responseTrust * 0.3);
            trustScores.Add(combinedTrust);
        }

        return trustScores.Average();
    }

    private double CalculateConsensusConfidence(List<AgentResponse> responses)
    {
        if (!responses.Any())
            return 0;

        var successfulResponses = responses.Where(r => r.Success).ToList();
        var successRate = (double)successfulResponses.Count / responses.Count;
        var avgConfidence = successfulResponses.Any() ? successfulResponses.Average(r => r.ConfidenceScore) : 0;

        return (successRate * 0.6) + (avgConfidence * 0.4);
    }

    private async Task<double> CalculateAverageReputationAsync(HashSet<string> agentIds)
    {
        if (!agentIds.Any())
            return 0;

        var reputations = new List<double>();
        foreach (var agentId in agentIds)
        {
            var reputation = await _reputationSystem.GetReputationAsync(agentId);
            reputations.Add(reputation);
        }

        return reputations.Average();
    }

    private string AggregateResults(List<AgentResponse> responses)
    {
        var successfulResults = responses.Where(r => r.Success && !string.IsNullOrEmpty(r.Result)).ToList();
        
        if (!successfulResults.Any())
            return "No successful results to aggregate";

        // Simple aggregation - in a real implementation, this might use AI to combine results intelligently
        return string.Join("\n---\n", successfulResults.Select(r => 
            $"Agent {r.AgentId} (Confidence: {r.ConfidenceScore:F2}):\n{r.Result}"));
    }

    private async Task<double> CalculateAgentRiskAsync(string agentId)
    {
        var reputation = await _reputationSystem.GetReputationAsync(agentId);
        var trustScore = await _reputationSystem.CalculateTrustScoreAsync(agentId);
        
        // High risk = low reputation + low trust
        return (1.0 - reputation) * 0.6 + (1.0 - trustScore.Score) * 0.4;
    }

    private (int Completed, int Failed) GetTaskStatistics()
    {
        lock (_lockObject)
        {
            var completed = _taskResults.Values.Count(r => r.Success);
            var failed = _taskResults.Values.Count(r => !r.Success);
            return (completed, failed);
        }
    }

    private double CalculateSystemLoad(List<AgentInfo> agents)
    {
        if (!agents.Any())
            return 0;

        var totalLoad = agents.Sum(a => a.CurrentLoad);
        var maxPossibleLoad = agents.Count * 100; // Assuming 100 is max load per agent
        
        return maxPossibleLoad > 0 ? (double)totalLoad / maxPossibleLoad : 0;
    }

    private async Task StartBackgroundMonitoringAsync()
    {
        while (true)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(_configuration.Monitoring.MetricsCollectionInterval / 1000.0));
                
                var metrics = await GetRealTimeMetricsAsync();
                
                // Check alert thresholds
                if (metrics.ConsensusParticipation < _configuration.Monitoring.AlertThresholds.LowConsensusParticipation)
                {
                    _logger.LogWarning("Alert: Low consensus participation {Participation:P2}", metrics.ConsensusParticipation);
                }
                
                if (metrics.SuspiciousActivityScore > 0.3)
                {
                    _logger.LogWarning("Alert: High suspicious activity score {Score:P2}", metrics.SuspiciousActivityScore);
                }
                
                if (metrics.AverageResponseTime > _configuration.Monitoring.AlertThresholds.HighLatency)
                {
                    _logger.LogWarning("Alert: High average response time {ResponseTime}ms", metrics.AverageResponseTime);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in background monitoring");
            }
        }
    }
}
