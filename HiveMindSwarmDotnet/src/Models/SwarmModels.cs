namespace HiveMindSwarmDotnet.Console.Models;

public enum AgentRole
{
    Orchestrator,
    PRExtractor,
    JiraContext,
    CodeAnalyzer,
    RequirementMapper,
    TestCoverage,
    RiskAssessment,
    IntegrationAnalyzer,
    SummaryGenerator,
    Learning
}

public enum OptimizationStrategy
{
    Darwinian,
    Lamarckian
}

public enum CommunicationTopology
{
    FullyConnected,
    Dynamic,
    Hierarchical
}

public class SwarmTask
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Description { get; set; } = string.Empty;
    public AgentRole[] RequiredRoles { get; set; } = Array.Empty<AgentRole>();
    public TimeSpan MaxExecutionTime { get; set; } = TimeSpan.FromMinutes(5);
    public Dictionary<string, object> Parameters { get; set; } = new();
    public int Priority { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class AgentTask
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string SwarmTaskId { get; set; } = string.Empty;
    public string AgentId { get; set; } = string.Empty;
    public AgentRole Role { get; set; }
    public string Prompt { get; set; } = string.Empty;
    public Dictionary<string, object> Context { get; set; } = new();
    public List<string> Dependencies { get; set; } = new();
    public SwarmTaskStatus Status { get; set; } = SwarmTaskStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class AgentResponse
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TaskId { get; set; } = string.Empty;
    public string AgentId { get; set; } = string.Empty;
    public AgentRole Role { get; set; }
    public string Content { get; set; } = string.Empty;
    public double Confidence { get; set; } = 0.0;
    public Dictionary<string, object> Metadata { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsValid { get; set; } = true;
    public List<string> ValidationErrors { get; set; } = new();
}

public class AgentMessage
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string FromAgentId { get; set; } = string.Empty;
    public string ToAgentId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public MessageType Type { get; set; } = MessageType.Information;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Dictionary<string, object> Metadata { get; set; } = new();
}

public enum MessageType
{
    Information,
    Request,
    Response,
    Validation,
    Error
}

public class SwarmResult
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string TaskId { get; set; } = string.Empty;
    public string FinalAnswer { get; set; } = string.Empty;
    public List<AgentResponse> AgentResponses { get; set; } = new();
    public List<AgentMessage> Communications { get; set; } = new();
    public double OverallConfidence { get; set; } = 0.0;
    public TimeSpan ExecutionTime { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsSuccessful { get; set; } = true;
    public List<string> Errors { get; set; } = new();
    public Dictionary<string, object> Metrics { get; set; } = new();
}

public class CommunicationLink
{
    public string FromAgentId { get; set; } = string.Empty;
    public string ToAgentId { get; set; } = string.Empty;
    public double Weight { get; set; } = 1.0;
    public double Probability { get; set; } = 1.0;
    public int MessageCount { get; set; } = 0;
    public DateTime LastUsed { get; set; } = DateTime.UtcNow;
}

public class SwarmMetrics
{
    public string SwarmId { get; set; } = string.Empty;
    public int TotalTasks { get; set; }
    public int CompletedTasks { get; set; }
    public int FailedTasks { get; set; }
    public double AverageExecutionTime { get; set; }
    public double AverageConfidence { get; set; }
    public int TotalCommunications { get; set; }
    public Dictionary<AgentRole, int> AgentUtilization { get; set; } = new();
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
}

public enum SwarmTaskStatus
{
    Pending,
    InProgress,
    Completed,
    Failed,
    Cancelled
}