using HiveMindSwarmDotnet.Console.Models;

namespace HiveMindSwarmDotnet.Console.Choreography;

public interface ISwarmEvent
{
    string EventId { get; }
    string EventType { get; }
    string SourceAgentId { get; }
    DateTime Timestamp { get; }
    Dictionary<string, object> Payload { get; }
}

public abstract class SwarmEvent : ISwarmEvent
{
    public string EventId { get; } = Guid.NewGuid().ToString();
    public abstract string EventType { get; }
    public string SourceAgentId { get; set; } = string.Empty;
    public DateTime Timestamp { get; } = DateTime.UtcNow;
    public Dictionary<string, object> Payload { get; set; } = new();
}

public class TaskAvailableEvent : SwarmEvent
{
    public override string EventType => "TaskAvailable";
    public AgentRole RequiredRole { get; set; }
    public string TaskDescription { get; set; } = string.Empty;
    public int Priority { get; set; }
}

public class TaskCompletedEvent : SwarmEvent
{
    public override string EventType => "TaskCompleted";
    public string TaskId { get; set; } = string.Empty;
    public AgentRole CompletedByRole { get; set; }
    public string Result { get; set; } = string.Empty;
    public double Confidence { get; set; }
}

public class AgentCapabilityEvent : SwarmEvent
{
    public override string EventType => "AgentCapability";
    public AgentRole Role { get; set; }
    public List<string> Capabilities { get; set; } = new();
    public double CurrentLoad { get; set; }
}

public class CollaborationRequestEvent : SwarmEvent
{
    public override string EventType => "CollaborationRequest";
    public string RequestingAgentId { get; set; } = string.Empty;
    public List<AgentRole> RequiredRoles { get; set; } = new();
    public string Context { get; set; } = string.Empty;
}

public class ResultSynthesisEvent : SwarmEvent
{
    public override string EventType => "ResultSynthesis";
    public List<AgentResponse> Responses { get; set; } = new();
    public string SwarmTaskId { get; set; } = string.Empty;
}