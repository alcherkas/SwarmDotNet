using HiveMindSwarmDotnet.Console.Models;

namespace HiveMindSwarmDotnet.Console.Choreography;

public interface IAutonomousAgent
{
    string AgentId { get; }
    AgentRole Role { get; }
    AgentCapabilities Capabilities { get; }
    AgentState State { get; }
    
    Task InitializeAsync(IEventBus eventBus, CancellationToken cancellationToken = default);
    Task<bool> CanHandleTaskAsync(TaskAvailableEvent taskEvent, CancellationToken cancellationToken = default);
    Task<DecisionResult> MakeDecisionAsync(ISwarmEvent @event, CancellationToken cancellationToken = default);
    Task<AgentResponse> ExecuteTaskAsync(AgentTask task, CancellationToken cancellationToken = default);
    Task UpdateStateAsync(AgentState newState, CancellationToken cancellationToken = default);
    Task<List<AgentRole>> RequestCollaborationAsync(string context, CancellationToken cancellationToken = default);
}

public class AgentCapabilities
{
    public List<string> Skills { get; set; } = new();
    public double MaxWorkload { get; set; } = 1.0;
    public double CurrentLoad { get; set; } = 0.0;
    public Dictionary<string, double> PerformanceMetrics { get; set; } = new();
    public List<AgentRole> CanCollaborateWith { get; set; } = new();
}

public class AgentState
{
    public string Status { get; set; } = "Available";
    public List<string> CurrentTasks { get; set; } = new();
    public Dictionary<string, object> Memory { get; set; } = new();
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
    public double ConfidenceLevel { get; set; } = 1.0;
}

public class DecisionResult
{
    public string Decision { get; set; } = string.Empty;
    public double Confidence { get; set; }
    public List<string> Reasons { get; set; } = new();
    public Dictionary<string, object> AdditionalData { get; set; } = new();
}

public enum DecisionType
{
    AcceptTask,
    RejectTask,
    RequestCollaboration,
    PublishResult,
    Ignore
}