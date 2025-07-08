using System.Collections.Concurrent;
using HiveMindSwarmDotnet.Console.Agents;
using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace HiveMindSwarmDotnet.Console.Core;

public class AgentPool : IAgentPool
{
    private readonly Kernel _kernel;
    private readonly ISwarmConfiguration _configuration;
    private readonly ICommunicationHub _communicationHub;
    private readonly ILogger<AgentPool> _logger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly ConcurrentDictionary<AgentRole, Queue<ISwarmAgent>> _availableAgents = new();
    private readonly ConcurrentDictionary<string, ISwarmAgent> _activeAgents = new();
    private readonly ConcurrentDictionary<AgentRole, int> _agentCounts = new();
    private readonly SemaphoreSlim _poolSemaphore = new(1, 1);

    public AgentPool(
        Kernel kernel,
        ISwarmConfiguration configuration,
        ICommunicationHub communicationHub,
        ILogger<AgentPool> logger,
        ILoggerFactory loggerFactory)
    {
        _kernel = kernel;
        _configuration = configuration;
        _communicationHub = communicationHub;
        _logger = logger;
        _loggerFactory = loggerFactory;
    }

    public async Task<ISwarmAgent> GetAgentAsync(AgentRole role, CancellationToken cancellationToken = default)
    {
        await _poolSemaphore.WaitAsync(cancellationToken);
        
        try
        {
            if (!_availableAgents.TryGetValue(role, out var agentQueue))
            {
                agentQueue = new Queue<ISwarmAgent>();
                _availableAgents[role] = agentQueue;
            }

            ISwarmAgent agent = null;
            
            if (agentQueue.Count > 0)
            {
                agent = agentQueue.Dequeue();
                _logger.LogDebug("Retrieved existing agent {AgentId} for role {Role}", agent.Id, role);
            }
            else
            {
                var currentCount = _agentCounts.GetValueOrDefault(role, 0);
                if (currentCount < _configuration.MaxAgents)
                {
                    agent = await CreateAgentAsync(role, cancellationToken);
                    _agentCounts[role] = currentCount + 1;
                    _logger.LogInformation("Created new agent {AgentId} for role {Role}", agent.Id, role);
                }
                else
                {
                    _logger.LogWarning("Maximum agents reached for role {Role}", role);
                    return null;
                }
            }

            if (agent != null)
            {
                _activeAgents[agent.Id] = agent;
                await _communicationHub.RegisterAgentAsync(agent.Id, cancellationToken);
            }

            return agent;
        }
        finally
        {
            _poolSemaphore.Release();
        }
    }

    public async Task ReturnAgentAsync(ISwarmAgent agent, CancellationToken cancellationToken = default)
    {
        if (agent == null)
            return;

        await _poolSemaphore.WaitAsync(cancellationToken);
        
        try
        {
            _activeAgents.TryRemove(agent.Id, out _);
            await _communicationHub.UnregisterAgentAsync(agent.Id, cancellationToken);

            if (agent.IsAvailable)
            {
                if (!_availableAgents.TryGetValue(agent.Role, out var agentQueue))
                {
                    agentQueue = new Queue<ISwarmAgent>();
                    _availableAgents[agent.Role] = agentQueue;
                }

                agentQueue.Enqueue(agent);
                _logger.LogDebug("Returned agent {AgentId} to pool", agent.Id);
            }
            else
            {
                _logger.LogWarning("Agent {AgentId} returned in unavailable state", agent.Id);
                await agent.DisposeAsync();
                _agentCounts[agent.Role] = Math.Max(0, _agentCounts.GetValueOrDefault(agent.Role, 0) - 1);
            }
        }
        finally
        {
            _poolSemaphore.Release();
        }
    }

    public async Task<IEnumerable<ISwarmAgent>> GetAvailableAgentsAsync(CancellationToken cancellationToken = default)
    {
        var availableAgents = new List<ISwarmAgent>();
        
        foreach (var roleQueue in _availableAgents.Values)
        {
            availableAgents.AddRange(roleQueue);
        }

        return await Task.FromResult(availableAgents);
    }

    public async Task<SwarmMetrics> GetPoolMetricsAsync()
    {
        var metrics = new SwarmMetrics
        {
            SwarmId = "agent_pool",
            LastUpdated = DateTime.UtcNow
        };

        foreach (var (role, count) in _agentCounts)
        {
            metrics.AgentUtilization[role] = count;
        }

        var totalAgents = _agentCounts.Values.Sum();
        var availableAgents = _availableAgents.Values.Sum(q => q.Count);
        
        metrics.TotalTasks = totalAgents;
        metrics.CompletedTasks = availableAgents;
        metrics.FailedTasks = totalAgents - availableAgents;

        return await Task.FromResult(metrics);
    }

    private async Task<ISwarmAgent> CreateAgentAsync(AgentRole role, CancellationToken cancellationToken)
    {
        var roleConfig = _configuration.AgentRoles.GetValueOrDefault(role);
        if (roleConfig == null)
        {
            roleConfig = new AgentRoleConfiguration
            {
                ModelId = _configuration.DefaultModel,
                SystemPrompt = GetDefaultSystemPrompt(role),
                MaxTokens = 2000,
                Temperature = 0.7
            };
        }

        var logger = _loggerFactory.CreateLogger<SwarmAgent>();
        
        ISwarmAgent agent = role switch
        {
            AgentRole.Orchestrator => new OrchestratorAgent(_kernel, roleConfig, logger!),
            AgentRole.PRExtractor => new PRExtractorAgent(_kernel, roleConfig, logger!),
            AgentRole.JiraContext => new JiraContextAgent(_kernel, roleConfig, logger!),
            AgentRole.CodeAnalyzer => new CodeAnalyzerAgent(_kernel, roleConfig, logger!),
            AgentRole.RequirementMapper => new RequirementMapperAgent(_kernel, roleConfig, logger!),
            AgentRole.TestCoverage => new TestCoverageAgent(_kernel, roleConfig, logger!),
            AgentRole.RiskAssessment => new RiskAssessmentAgent(_kernel, roleConfig, logger!),
            AgentRole.IntegrationAnalyzer => new IntegrationAnalyzerAgent(_kernel, roleConfig, logger!),
            AgentRole.SummaryGenerator => new SummaryGeneratorAgent(_kernel, roleConfig, logger!),
            AgentRole.Learning => new LearningAgent(_kernel, roleConfig, logger!),
            _ => throw new ArgumentException($"Unknown agent role: {role}")
        };

        await agent.InitializeAsync(cancellationToken);
        return agent;
    }

    private string GetDefaultSystemPrompt(AgentRole role)
    {
        return role switch
        {
            AgentRole.Orchestrator => "You are a central coordinator and workflow manager. Coordinate tasks between agents and manage the overall analysis workflow.",
            AgentRole.PRExtractor => "You are a pull request data specialist. Extract PR metadata, file changes, diffs, and linked ticket information.",
            AgentRole.JiraContext => "You are a business requirement analyst. Extract Jira tickets, user stories, acceptance criteria, and business context.",
            AgentRole.CodeAnalyzer => "You are a technical implementation specialist. Analyze code changes, patterns, architecture, and quality metrics.",
            AgentRole.RequirementMapper => "You are a business-to-code alignment specialist. Map code changes to requirements and identify gaps.",
            AgentRole.TestCoverage => "You are a quality assurance specialist. Analyze test coverage, identify missing tests, and suggest test scenarios.",
            AgentRole.RiskAssessment => "You are a risk and impact analyzer. Identify breaking changes, security vulnerabilities, and performance risks.",
            AgentRole.IntegrationAnalyzer => "You are a system integration specialist. Analyze API dependencies, service interactions, and contract violations.",
            AgentRole.SummaryGenerator => "You are a communication specialist. Synthesize findings from all agents into clear, actionable summaries.",
            AgentRole.Learning => "You are a continuous improvement specialist. Learn from feedback and improve analysis accuracy over time.",
            _ => "You are a helpful AI assistant."
        };
    }

    public async Task InitializePoolAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Initializing agent pool");
        
        foreach (var role in Enum.GetValues<AgentRole>())
        {
            _availableAgents[role] = new Queue<ISwarmAgent>();
            _agentCounts[role] = 0;
        }

        await Task.CompletedTask;
    }

    public async Task DisposeAsync()
    {
        _logger.LogInformation("Disposing agent pool");
        
        var disposeTasks = new List<Task>();
        
        foreach (var agent in _activeAgents.Values)
        {
            disposeTasks.Add(agent.DisposeAsync());
        }

        foreach (var agentQueue in _availableAgents.Values)
        {
            while (agentQueue.Count > 0)
            {
                var agent = agentQueue.Dequeue();
                disposeTasks.Add(agent.DisposeAsync());
            }
        }

        await Task.WhenAll(disposeTasks);
        _poolSemaphore.Dispose();
    }
}