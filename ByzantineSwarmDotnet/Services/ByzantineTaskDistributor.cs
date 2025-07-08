using Microsoft.Extensions.Logging;
using ByzantineSwarmDotnet.Models;
using ByzantineSwarmDotnet.Interfaces;

namespace ByzantineSwarmDotnet.Services;

public class ByzantineTaskDistributor : IByzantineTaskDistributor
{
    private readonly ILogger<ByzantineTaskDistributor> _logger;
    private readonly IReputationSystem _reputationSystem;
    private readonly IHierarchicalAgentPool _agentPool;
    private readonly ILoadBalancer _loadBalancer;

    public ByzantineTaskDistributor(
        ILogger<ByzantineTaskDistributor> logger,
        IReputationSystem reputationSystem,
        IHierarchicalAgentPool agentPool,
        ILoadBalancer loadBalancer)
    {
        _logger = logger;
        _reputationSystem = reputationSystem;
        _agentPool = agentPool;
        _loadBalancer = loadBalancer;
    }

    public async Task<List<AgentTask>> DistributeTaskAsync(SwarmTask swarmTask)
    {
        try
        {
            _logger.LogInformation("Distributing task {TaskId}: {Description}", swarmTask.Id, swarmTask.Description);

            // Step 1: Analyze task complexity
            var taskComplexity = await AnalyzeTaskComplexityAsync(swarmTask);
            _logger.LogDebug("Task complexity analysis: {Complexity}", string.Join(", ", taskComplexity.Select(kvp => $"{kvp.Key}: {kvp.Value}")));

            // Step 2: Select candidate agents
            var candidateAgents = await SelectCandidateAgentsAsync(swarmTask, taskComplexity);
            _logger.LogInformation("Selected {Count} candidate agents for task {TaskId}", candidateAgents.Count, swarmTask.Id);

            // Step 3: Ensure Byzantine fault tolerance
            var selectedAgents = await EnsureByzantineFaultToleranceAsync(candidateAgents, swarmTask);
            _logger.LogInformation("Final selection: {Count} agents with Byzantine fault tolerance", selectedAgents.Count);

            // Step 4: Create redundant tasks for fault tolerance
            var distributedTasks = await CreateRedundantTasksAsync(swarmTask, selectedAgents);
            _logger.LogInformation("Created {Count} distributed tasks for execution", distributedTasks.Count);

            // Step 5: Validate task distribution
            var isValid = await ValidateTaskDistributionAsync(distributedTasks);
            if (!isValid)
            {
                throw new TaskDistributionFailedException($"Task distribution validation failed for task {swarmTask.Id}");
            }

            return distributedTasks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error distributing task {TaskId}", swarmTask.Id);
            throw;
        }
    }

    public async Task<List<AgentInfo>> SelectCandidateAgentsAsync(SwarmTask task, Dictionary<string, object> taskComplexity)
    {
        try
        {
            var allAgents = await _agentPool.GetActiveAgentsAsync();
            var candidateAgents = new List<AgentInfo>();

            // Filter agents by required roles
            if (task.RequiredRoles?.Any() == true)
            {
                var requiredRoles = task.RequiredRoles.Select(r => Enum.Parse<AgentRole>(r, true)).ToList();
                
                foreach (var role in requiredRoles)
                {
                    var roleAgents = await _agentPool.GetAgentsByRoleAsync(role);
                    candidateAgents.AddRange(roleAgents);
                }
            }
            else
            {
                // If no specific roles required, use all active agents
                candidateAgents.AddRange(allAgents);
            }

            // Remove duplicates
            candidateAgents = candidateAgents.DistinctBy(a => a.Id).ToList();

            // Filter by reputation and trust
            var trustedCandidates = new List<AgentInfo>();
            foreach (var agent in candidateAgents)
            {
                var trustScore = await _reputationSystem.CalculateTrustScoreAsync(agent.Id);
                if (trustScore.IsTrusted && agent.ReputationScore >= 0.3) // Minimum reputation threshold
                {
                    trustedCandidates.Add(agent);
                }
            }

            // Filter by load and availability
            var availableCandidates = trustedCandidates
                .Where(a => a.CurrentLoad < 80) // Don't overload agents
                .Where(a => DateTime.UtcNow - a.LastHeartbeat < TimeSpan.FromMinutes(2)) // Recent heartbeat
                .ToList();

            // Sort by suitability score
            var scoredCandidates = await ScoreAgentSuitabilityAsync(availableCandidates, task, taskComplexity);
            
            return scoredCandidates
                .OrderByDescending(a => a.suitabilityScore)
                .Select(a => a.agent)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error selecting candidate agents for task {TaskId}", task.Id);
            throw;
        }
    }

    public async Task<List<AgentTask>> CreateRedundantTasksAsync(SwarmTask swarmTask, List<AgentInfo> selectedAgents)
    {
        try
        {
            var distributedTasks = new List<AgentTask>();

            // Determine redundancy level based on fault tolerance requirements
            var redundancyFactor = swarmTask.FaultToleranceLevel switch
            {
                FaultToleranceLevel.Basic => 1,
                FaultToleranceLevel.Byzantine => 3, // Execute with 3 agents for Byzantine fault tolerance
                FaultToleranceLevel.Quantum => 5,   // Higher redundancy for quantum-resistant scenarios
                _ => 1
            };

            // Create task decomposition strategy
            var decompositionStrategy = DetermineDecompositionStrategy(swarmTask, selectedAgents.Count);
            
            switch (decompositionStrategy)
            {
                case TaskDecompositionStrategy.Parallel:
                    distributedTasks.AddRange(await CreateParallelTasksAsync(swarmTask, selectedAgents, redundancyFactor));
                    break;
                
                case TaskDecompositionStrategy.Sequential:
                    distributedTasks.AddRange(await CreateSequentialTasksAsync(swarmTask, selectedAgents, redundancyFactor));
                    break;
                
                case TaskDecompositionStrategy.Hierarchical:
                    distributedTasks.AddRange(await CreateHierarchicalTasksAsync(swarmTask, selectedAgents, redundancyFactor));
                    break;
                
                case TaskDecompositionStrategy.Redundant:
                default:
                    distributedTasks.AddRange(await CreateRedundantTasksAsync(swarmTask, selectedAgents, redundancyFactor));
                    break;
            }

            // Add task dependencies for Byzantine fault tolerance
            await AddByzantineTaskDependenciesAsync(distributedTasks);

            return distributedTasks;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating redundant tasks for {TaskId}", swarmTask.Id);
            throw;
        }
    }

    public async Task<bool> ValidateTaskDistributionAsync(List<AgentTask> distributedTasks)
    {
        try
        {
            // Validate basic task structure
            if (!distributedTasks.Any())
            {
                _logger.LogWarning("No distributed tasks to validate");
                return false;
            }

            // Validate all tasks have valid agent assignments
            var agentIds = distributedTasks.Select(t => t.AssignedAgentId).Distinct().ToList();
            foreach (var agentId in agentIds)
            {
                var agent = await _agentPool.GetAgentAsync(agentId);
                if (agent == null || !agent.IsActive)
                {
                    _logger.LogWarning("Task assigned to inactive agent {AgentId}", agentId);
                    return false;
                }
            }

            // Validate Byzantine fault tolerance requirements
            var taskGroups = distributedTasks.GroupBy(t => t.SwarmTaskId).ToList();
            foreach (var group in taskGroups)
            {
                var tasks = group.ToList();
                var uniqueAgents = tasks.Select(t => t.AssignedAgentId).Distinct().Count();
                
                // For Byzantine fault tolerance, ensure at least 3 different agents
                if (uniqueAgents < 3)
                {
                    _logger.LogWarning("Insufficient agent diversity for Byzantine fault tolerance in task group {SwarmTaskId}", group.Key);
                    return false;
                }
            }

            // Validate task dependencies are acyclic
            if (!ValidateTaskDependencies(distributedTasks))
            {
                _logger.LogWarning("Cyclic dependencies detected in task distribution");
                return false;
            }

            // Validate load distribution
            var loadDistribution = await ValidateLoadDistributionAsync(distributedTasks);
            if (!loadDistribution)
            {
                _logger.LogWarning("Unbalanced load distribution detected");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating task distribution");
            return false;
        }
    }

    private async Task<Dictionary<string, object>> AnalyzeTaskComplexityAsync(SwarmTask task)
    {
        var complexity = new Dictionary<string, object>();

        // Analyze description complexity
        var descriptionLength = task.Description?.Length ?? 0;
        complexity["DescriptionComplexity"] = descriptionLength switch
        {
            < 100 => "Low",
            < 500 => "Medium",
            _ => "High"
        };

        // Analyze parameter complexity
        complexity["ParameterCount"] = task.Parameters.Count;
        complexity["HasNestedParameters"] = task.Parameters.Values.Any(v => v is Dictionary<string, object>);

        // Analyze security requirements
        complexity["SecurityLevel"] = task.SecurityLevel.ToString();
        complexity["RequiresConsensus"] = task.RequiredConsensus;

        // Analyze time constraints
        complexity["TimeConstraint"] = task.MaxExecutionTime.TotalMinutes switch
        {
            < 1 => "Urgent",
            < 5 => "Normal",
            _ => "Relaxed"
        };

        // Analyze role requirements
        complexity["RoleCount"] = task.RequiredRoles?.Length ?? 0;
        complexity["RequiresSpecializedRoles"] = task.RequiredRoles?.Any(r => 
            r is "Validator" or "ReputationManager" or "Security") ?? false;

        return complexity;
    }

    private async Task<List<(AgentInfo agent, double suitabilityScore)>> ScoreAgentSuitabilityAsync(
        List<AgentInfo> candidates, SwarmTask task, Dictionary<string, object> taskComplexity)
    {
        var scoredAgents = new List<(AgentInfo agent, double suitabilityScore)>();

        foreach (var agent in candidates)
        {
            var score = 0.0;

            // Reputation score (40% weight)
            score += agent.ReputationScore * 0.4;

            // Role suitability (30% weight)
            var roleSuitability = CalculateRoleSuitability(agent, task);
            score += roleSuitability * 0.3;

            // Load factor (20% weight) - prefer less loaded agents
            var loadFactor = 1.0 - (agent.CurrentLoad / 100.0);
            score += loadFactor * 0.2;

            // Response time factor (10% weight)
            var responseTimeFactor = CalculateResponseTimeFactor(agent.AverageResponseTime);
            score += responseTimeFactor * 0.1;

            scoredAgents.Add((agent, score));
        }

        return scoredAgents;
    }

    private double CalculateRoleSuitability(AgentInfo agent, SwarmTask task)
    {
        // Base suitability based on role
        var baseSuitability = agent.Role switch
        {
            AgentRole.Supervisor => 0.9,
            AgentRole.Validator => 0.8,
            AgentRole.Worker => 0.7,
            _ => 0.6
        };

        // Bonus for matching required roles
        if (task.RequiredRoles?.Contains(agent.Role.ToString()) == true)
        {
            baseSuitability += 0.2;
        }

        // Capability matching
        var capabilityBonus = 0.0;
        foreach (var capability in agent.Capabilities)
        {
            if (task.Description.Contains(capability, StringComparison.OrdinalIgnoreCase))
            {
                capabilityBonus += 0.05;
            }
        }

        return Math.Min(1.0, baseSuitability + capabilityBonus);
    }

    private double CalculateResponseTimeFactor(TimeSpan averageResponseTime)
    {
        var seconds = averageResponseTime.TotalSeconds;
        return seconds switch
        {
            < 1 => 1.0,
            < 3 => 0.9,
            < 5 => 0.8,
            < 10 => 0.6,
            _ => 0.4
        };
    }

    private async Task<List<AgentInfo>> EnsureByzantineFaultToleranceAsync(List<AgentInfo> candidates, SwarmTask task)
    {
        var requiredAgents = CalculateRequiredAgentsForByzantineFaultTolerance(task);
        
        if (candidates.Count < requiredAgents)
        {
            _logger.LogWarning("Insufficient candidates for Byzantine fault tolerance. Required: {Required}, Available: {Available}",
                requiredAgents, candidates.Count);
            
            // Try to get additional agents
            var additionalAgents = await _agentPool.GetActiveAgentsAsync();
            var newCandidates = additionalAgents
                .Where(a => !candidates.Any(c => c.Id == a.Id))
                .Where(a => a.ReputationScore >= 0.2) // Lower threshold for emergency scaling
                .Take(requiredAgents - candidates.Count)
                .ToList();
            
            candidates.AddRange(newCandidates);
        }

        // Ensure diversity in hierarchy levels and roles
        var selectedAgents = EnsureAgentDiversity(candidates.Take(requiredAgents).ToList());
        
        return selectedAgents;
    }

    private int CalculateRequiredAgentsForByzantineFaultTolerance(SwarmTask task)
    {
        return task.FaultToleranceLevel switch
        {
            FaultToleranceLevel.Basic => 1,
            FaultToleranceLevel.Byzantine => 10, // 3f + 1 where f = 3 (max 3 Byzantine faults)
            FaultToleranceLevel.Quantum => 16,   // Higher redundancy
            _ => 4
        };
    }

    private List<AgentInfo> EnsureAgentDiversity(List<AgentInfo> agents)
    {
        var diverseAgents = new List<AgentInfo>();
        
        // Ensure we have at least one of each critical role
        var criticalRoles = new[] { AgentRole.Supervisor, AgentRole.Validator };
        
        foreach (var role in criticalRoles)
        {
            var roleAgent = agents.FirstOrDefault(a => a.Role == role);
            if (roleAgent != null)
            {
                diverseAgents.Add(roleAgent);
                agents.Remove(roleAgent);
            }
        }
        
        // Add remaining agents to reach required count
        diverseAgents.AddRange(agents);
        
        return diverseAgents;
    }

    private TaskDecompositionStrategy DetermineDecompositionStrategy(SwarmTask task, int agentCount)
    {
        // Simple strategy determination based on task characteristics
        if (task.RequiredConsensus && task.FaultToleranceLevel == FaultToleranceLevel.Byzantine)
        {
            return TaskDecompositionStrategy.Redundant;
        }
        
        if (agentCount > 8)
        {
            return TaskDecompositionStrategy.Hierarchical;
        }
        
        if (task.MaxExecutionTime < TimeSpan.FromMinutes(2))
        {
            return TaskDecompositionStrategy.Parallel;
        }
        
        return TaskDecompositionStrategy.Sequential;
    }

    private async Task<List<AgentTask>> CreateParallelTasksAsync(SwarmTask swarmTask, List<AgentInfo> agents, int redundancyFactor)
    {
        var tasks = new List<AgentTask>();
        var taskParts = Math.Min(agents.Count, 4); // Split into max 4 parallel parts
        
        for (int i = 0; i < taskParts; i++)
        {
            var task = new AgentTask
            {
                SwarmTaskId = swarmTask.Id,
                AssignedAgentId = agents[i % agents.Count].Id,
                Type = TaskType.General,
                Description = $"Parallel task {i + 1}/{taskParts}: {swarmTask.Description}",
                Parameters = new Dictionary<string, object>(swarmTask.Parameters)
                {
                    ["parallel_index"] = i,
                    ["total_parallel_tasks"] = taskParts,
                    ["execution_strategy"] = "parallel"
                }
            };
            
            tasks.Add(task);
        }
        
        return tasks;
    }

    private async Task<List<AgentTask>> CreateSequentialTasksAsync(SwarmTask swarmTask, List<AgentInfo> agents, int redundancyFactor)
    {
        var tasks = new List<AgentTask>();
        var phases = new[] { "Analysis", "Processing", "Validation", "Synthesis" };
        
        for (int i = 0; i < phases.Length && i < agents.Count; i++)
        {
            var task = new AgentTask
            {
                SwarmTaskId = swarmTask.Id,
                AssignedAgentId = agents[i].Id,
                Type = TaskType.General,
                Description = $"{phases[i]} phase: {swarmTask.Description}",
                Parameters = new Dictionary<string, object>(swarmTask.Parameters)
                {
                    ["phase"] = phases[i],
                    ["phase_index"] = i,
                    ["execution_strategy"] = "sequential"
                },
                Dependencies = i > 0 ? new List<string> { tasks[i - 1].Id } : new List<string>()
            };
            
            tasks.Add(task);
        }
        
        return tasks;
    }

    private async Task<List<AgentTask>> CreateHierarchicalTasksAsync(SwarmTask swarmTask, List<AgentInfo> agents, int redundancyFactor)
    {
        var tasks = new List<AgentTask>();
        
        // Create supervisor task
        var supervisor = agents.FirstOrDefault(a => a.Role == AgentRole.Supervisor) ?? agents.First();
        var supervisorTask = new AgentTask
        {
            SwarmTaskId = swarmTask.Id,
            AssignedAgentId = supervisor.Id,
            Type = TaskType.General,
            Description = $"Supervisor coordination: {swarmTask.Description}",
            Parameters = new Dictionary<string, object>(swarmTask.Parameters)
            {
                ["role"] = "supervisor",
                ["execution_strategy"] = "hierarchical"
            }
        };
        tasks.Add(supervisorTask);
        
        // Create worker tasks
        var workers = agents.Where(a => a.Id != supervisor.Id).Take(6).ToList();
        foreach (var worker in workers)
        {
            var workerTask = new AgentTask
            {
                SwarmTaskId = swarmTask.Id,
                AssignedAgentId = worker.Id,
                Type = TaskType.General,
                Description = $"Worker task for {worker.Role}: {swarmTask.Description}",
                Parameters = new Dictionary<string, object>(swarmTask.Parameters)
                {
                    ["role"] = "worker",
                    ["supervisor_task_id"] = supervisorTask.Id,
                    ["execution_strategy"] = "hierarchical"
                },
                Dependencies = new List<string> { supervisorTask.Id }
            };
            tasks.Add(workerTask);
        }
        
        return tasks;
    }

    private async Task<List<AgentTask>> CreateRedundantTasksAsync(SwarmTask swarmTask, List<AgentInfo> agents, int redundancyFactor)
    {
        var tasks = new List<AgentTask>();
        
        // Create identical tasks for multiple agents (Byzantine fault tolerance)
        var selectedAgents = agents.Take(redundancyFactor).ToList();
        
        foreach (var agent in selectedAgents)
        {
            var task = new AgentTask
            {
                SwarmTaskId = swarmTask.Id,
                AssignedAgentId = agent.Id,
                Type = TaskType.General,
                Description = swarmTask.Description,
                Parameters = new Dictionary<string, object>(swarmTask.Parameters)
                {
                    ["redundancy_agent"] = agent.Id,
                    ["execution_strategy"] = "redundant"
                }
            };
            tasks.Add(task);
        }
        
        return tasks;
    }

    private async Task AddByzantineTaskDependenciesAsync(List<AgentTask> tasks)
    {
        // Add validation dependencies for Byzantine fault tolerance
        var validatorTasks = tasks.Where(t => t.Parameters.ContainsKey("role") && 
            t.Parameters["role"].ToString() == "validator").ToList();
        
        foreach (var task in tasks)
        {
            if (!validatorTasks.Contains(task))
            {
                // Add validator dependency for non-validator tasks
                var validator = validatorTasks.FirstOrDefault();
                if (validator != null && !task.Dependencies.Contains(validator.Id))
                {
                    task.Dependencies.Add(validator.Id);
                }
            }
        }
    }

    private bool ValidateTaskDependencies(List<AgentTask> tasks)
    {
        // Simple cycle detection using DFS
        var visited = new HashSet<string>();
        var recursionStack = new HashSet<string>();
        
        foreach (var task in tasks)
        {
            if (!visited.Contains(task.Id))
            {
                if (HasCycle(task, tasks, visited, recursionStack))
                {
                    return false;
                }
            }
        }
        
        return true;
    }

    private bool HasCycle(AgentTask task, List<AgentTask> allTasks, HashSet<string> visited, HashSet<string> recursionStack)
    {
        visited.Add(task.Id);
        recursionStack.Add(task.Id);
        
        foreach (var dependencyId in task.Dependencies)
        {
            var dependency = allTasks.FirstOrDefault(t => t.Id == dependencyId);
            if (dependency != null)
            {
                if (!visited.Contains(dependency.Id))
                {
                    if (HasCycle(dependency, allTasks, visited, recursionStack))
                    {
                        return true;
                    }
                }
                else if (recursionStack.Contains(dependency.Id))
                {
                    return true;
                }
            }
        }
        
        recursionStack.Remove(task.Id);
        return false;
    }

    private async Task<bool> ValidateLoadDistributionAsync(List<AgentTask> tasks)
    {
        var agentTaskCounts = tasks.GroupBy(t => t.AssignedAgentId)
            .ToDictionary(g => g.Key, g => g.Count());
        
        if (!agentTaskCounts.Any())
            return true;
        
        var maxTasks = agentTaskCounts.Values.Max();
        var minTasks = agentTaskCounts.Values.Min();
        
        // Load is considered balanced if the difference is not more than 2 tasks
        return (maxTasks - minTasks) <= 2;
    }

    private enum TaskDecompositionStrategy
    {
        Parallel,
        Sequential,
        Hierarchical,
        Redundant
    }
}
