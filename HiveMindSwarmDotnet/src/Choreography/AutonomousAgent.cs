using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;

namespace HiveMindSwarmDotnet.Console.Choreography;

public class AutonomousAgent : IAutonomousAgent
{
    private readonly ILogger<AutonomousAgent> _logger;
    private readonly ISwarmAgent _baseAgent;
    private IEventBus _eventBus = null!;

    public string AgentId => _baseAgent.Id;
    public AgentRole Role => _baseAgent.Role;
    public AgentCapabilities Capabilities { get; private set; }
    public AgentState State { get; private set; }

    public AutonomousAgent(ISwarmAgent baseAgent, ILogger<AutonomousAgent> logger)
    {
        _baseAgent = baseAgent;
        _logger = logger;
        
        Capabilities = new AgentCapabilities
        {
            Skills = GetSkillsForRole(baseAgent.Role),
            MaxWorkload = 3.0,
            CurrentLoad = 0.0,
            PerformanceMetrics = new Dictionary<string, double>(),
            CanCollaborateWith = GetCollaborationRoles(baseAgent.Role)
        };

        State = new AgentState
        {
            Status = "Available",
            CurrentTasks = new List<string>(),
            Memory = new Dictionary<string, object>(),
            LastActivity = DateTime.UtcNow,
            ConfidenceLevel = 1.0
        };
    }

    public async Task InitializeAsync(IEventBus eventBus, CancellationToken cancellationToken = default)
    {
        _eventBus = eventBus;
        
        await _eventBus.SubscribeAsync<TaskAvailableEvent>(OnTaskAvailableAsync, cancellationToken);
        await _eventBus.SubscribeAsync<TaskCompletedEvent>(OnTaskCompletedAsync, cancellationToken);
        await _eventBus.SubscribeAsync<CollaborationRequestEvent>(OnCollaborationRequestAsync, cancellationToken);
        await _eventBus.SubscribeAsync<ResultSynthesisEvent>(OnResultSynthesisAsync, cancellationToken);

        var capabilityEvent = new AgentCapabilityEvent
        {
            SourceAgentId = AgentId,
            Role = Role,
            Capabilities = Capabilities.Skills,
            CurrentLoad = Capabilities.CurrentLoad
        };

        await _eventBus.PublishAsync(capabilityEvent, cancellationToken);
        
        _logger.LogInformation("Autonomous agent {AgentId} with role {Role} initialized", AgentId, Role);
    }

    public async Task<bool> CanHandleTaskAsync(TaskAvailableEvent taskEvent, CancellationToken cancellationToken = default)
    {
        if (taskEvent.RequiredRole != Role)
            return false;

        if (Capabilities.CurrentLoad >= Capabilities.MaxWorkload)
            return false;

        if (State.Status != "Available")
            return false;

        return await Task.FromResult(true);
    }

    public async Task<DecisionResult> MakeDecisionAsync(ISwarmEvent @event, CancellationToken cancellationToken = default)
    {
        var decision = new DecisionResult
        {
            Confidence = 0.8,
            Reasons = new List<string>(),
            AdditionalData = new Dictionary<string, object>()
        };

        switch (@event)
        {
            case TaskAvailableEvent taskEvent:
                decision = await DecideOnTaskAsync(taskEvent, cancellationToken);
                break;
                
            case CollaborationRequestEvent collaborationEvent:
                decision = await DecideOnCollaborationAsync(collaborationEvent, cancellationToken);
                break;
                
            case ResultSynthesisEvent synthesisEvent:
                decision = await DecideOnSynthesisAsync(synthesisEvent, cancellationToken);
                break;
                
            default:
                decision.Decision = DecisionType.Ignore.ToString();
                decision.Reasons.Add($"Unknown event type: {@event.EventType}");
                break;
        }

        return decision;
    }

    public async Task<AgentResponse> ExecuteTaskAsync(AgentTask task, CancellationToken cancellationToken = default)
    {
        State.CurrentTasks.Add(task.Id);
        State.Status = "Busy";
        Capabilities.CurrentLoad += 1.0;

        try
        {
            var response = await _baseAgent.ProcessTaskAsync(task, cancellationToken);
            
            var completedEvent = new TaskCompletedEvent
            {
                SourceAgentId = AgentId,
                TaskId = task.Id,
                CompletedByRole = Role,
                Result = response.Content,
                Confidence = response.Confidence
            };

            await _eventBus.PublishAsync(completedEvent, cancellationToken);

            UpdatePerformanceMetrics(response.Confidence);
            
            return response;
        }
        finally
        {
            State.CurrentTasks.Remove(task.Id);
            Capabilities.CurrentLoad = Math.Max(0, Capabilities.CurrentLoad - 1.0);
            
            if (State.CurrentTasks.Count == 0)
            {
                State.Status = "Available";
            }
        }
    }

    public async Task UpdateStateAsync(AgentState newState, CancellationToken cancellationToken = default)
    {
        State = newState;
        State.LastActivity = DateTime.UtcNow;
        await Task.CompletedTask;
    }

    public async Task<List<AgentRole>> RequestCollaborationAsync(string context, CancellationToken cancellationToken = default)
    {
        var collaborationEvent = new CollaborationRequestEvent
        {
            SourceAgentId = AgentId,
            RequestingAgentId = AgentId,
            RequiredRoles = Capabilities.CanCollaborateWith,
            Context = context
        };

        await _eventBus.PublishAsync(collaborationEvent, cancellationToken);
        
        return Capabilities.CanCollaborateWith;
    }

    private async Task OnTaskAvailableAsync(TaskAvailableEvent taskEvent, CancellationToken cancellationToken)
    {
        var canHandle = await CanHandleTaskAsync(taskEvent, cancellationToken);
        if (!canHandle)
            return;

        var decision = await MakeDecisionAsync(taskEvent, cancellationToken);
        
        if (decision.Decision == DecisionType.AcceptTask.ToString())
        {
            var agentTask = new AgentTask
            {
                SwarmTaskId = taskEvent.EventId,
                Role = Role,
                Prompt = taskEvent.TaskDescription,
                Context = taskEvent.Payload,
                Status = SwarmTaskStatus.Pending
            };

            await ExecuteTaskAsync(agentTask, cancellationToken);
        }
    }

    private async Task OnTaskCompletedAsync(TaskCompletedEvent taskEvent, CancellationToken cancellationToken)
    {
        State.Memory[$"task_{taskEvent.TaskId}"] = new
        {
            Result = taskEvent.Result,
            Confidence = taskEvent.Confidence,
            CompletedBy = taskEvent.CompletedByRole,
            CompletedAt = taskEvent.Timestamp
        };

        await Task.CompletedTask;
    }

    private async Task OnCollaborationRequestAsync(CollaborationRequestEvent collaborationEvent, CancellationToken cancellationToken)
    {
        if (collaborationEvent.RequiredRoles.Contains(Role) && 
            collaborationEvent.RequestingAgentId != AgentId)
        {
            var decision = await MakeDecisionAsync(collaborationEvent, cancellationToken);
            
            if (decision.Decision == DecisionType.AcceptTask.ToString())
            {
                _logger.LogInformation("Agent {AgentId} accepted collaboration request from {RequestingAgent}", 
                    AgentId, collaborationEvent.RequestingAgentId);
            }
        }
    }

    private async Task OnResultSynthesisAsync(ResultSynthesisEvent synthesisEvent, CancellationToken cancellationToken)
    {
        if (Role == AgentRole.SummaryGenerator)
        {
            var decision = await MakeDecisionAsync(synthesisEvent, cancellationToken);
            
            if (decision.Decision == DecisionType.AcceptTask.ToString())
            {
                _logger.LogInformation("Agent {AgentId} will synthesize results for task {TaskId}", 
                    AgentId, synthesisEvent.SwarmTaskId);
            }
        }
    }

    private async Task<DecisionResult> DecideOnTaskAsync(TaskAvailableEvent taskEvent, CancellationToken cancellationToken)
    {
        var decision = new DecisionResult
        {
            Decision = DecisionType.AcceptTask.ToString(),
            Confidence = CalculateTaskConfidence(taskEvent),
            Reasons = new List<string>()
        };

        if (taskEvent.RequiredRole == Role)
        {
            decision.Reasons.Add($"Role {Role} matches required role");
        }

        if (Capabilities.CurrentLoad < Capabilities.MaxWorkload * 0.8)
        {
            decision.Reasons.Add("Agent has available capacity");
        }

        if (State.ConfidenceLevel > 0.7)
        {
            decision.Reasons.Add("Agent has high confidence level");
        }

        return await Task.FromResult(decision);
    }

    private async Task<DecisionResult> DecideOnCollaborationAsync(CollaborationRequestEvent collaborationEvent, CancellationToken cancellationToken)
    {
        var decision = new DecisionResult
        {
            Decision = DecisionType.AcceptTask.ToString(),
            Confidence = 0.8,
            Reasons = new List<string>()
        };

        if (collaborationEvent.RequiredRoles.Contains(Role))
        {
            decision.Reasons.Add($"Role {Role} is required for collaboration");
        }

        return await Task.FromResult(decision);
    }

    private async Task<DecisionResult> DecideOnSynthesisAsync(ResultSynthesisEvent synthesisEvent, CancellationToken cancellationToken)
    {
        var decision = new DecisionResult
        {
            Decision = Role == AgentRole.SummaryGenerator ? DecisionType.AcceptTask.ToString() : DecisionType.Ignore.ToString(),
            Confidence = 0.9,
            Reasons = new List<string>()
        };

        if (Role == AgentRole.SummaryGenerator)
        {
            decision.Reasons.Add("SummaryGenerator role is responsible for synthesis");
        }

        return await Task.FromResult(decision);
    }

    private double CalculateTaskConfidence(TaskAvailableEvent taskEvent)
    {
        var confidence = 0.8;
        
        if (Capabilities.CurrentLoad < Capabilities.MaxWorkload * 0.5)
            confidence += 0.1;
        
        if (State.ConfidenceLevel > 0.8)
            confidence += 0.05;
        
        if (taskEvent.Priority > 5)
            confidence += 0.05;

        return Math.Min(1.0, confidence);
    }

    private void UpdatePerformanceMetrics(double taskConfidence)
    {
        var key = $"task_confidence_{Role}";
        if (Capabilities.PerformanceMetrics.ContainsKey(key))
        {
            Capabilities.PerformanceMetrics[key] = (Capabilities.PerformanceMetrics[key] + taskConfidence) / 2;
        }
        else
        {
            Capabilities.PerformanceMetrics[key] = taskConfidence;
        }

        State.ConfidenceLevel = Capabilities.PerformanceMetrics.Values.Average();
    }

    private List<string> GetSkillsForRole(AgentRole role)
    {
        return role switch
        {
            AgentRole.PRExtractor => new List<string> { "git", "pull_request_analysis", "code_diff" },
            AgentRole.CodeAnalyzer => new List<string> { "code_analysis", "static_analysis", "complexity_analysis" },
            AgentRole.TestCoverage => new List<string> { "test_analysis", "coverage_analysis", "quality_metrics" },
            AgentRole.RiskAssessment => new List<string> { "risk_analysis", "security_assessment", "impact_analysis" },
            AgentRole.SummaryGenerator => new List<string> { "summarization", "synthesis", "reporting" },
            _ => new List<string> { "general_analysis" }
        };
    }

    private List<AgentRole> GetCollaborationRoles(AgentRole role)
    {
        return role switch
        {
            AgentRole.CodeAnalyzer => new List<AgentRole> { AgentRole.TestCoverage, AgentRole.RiskAssessment },
            AgentRole.TestCoverage => new List<AgentRole> { AgentRole.CodeAnalyzer, AgentRole.RiskAssessment },
            AgentRole.RiskAssessment => new List<AgentRole> { AgentRole.CodeAnalyzer, AgentRole.TestCoverage },
            AgentRole.SummaryGenerator => Enum.GetValues<AgentRole>().Where(r => r != AgentRole.SummaryGenerator).ToList(),
            _ => new List<AgentRole>()
        };
    }
}