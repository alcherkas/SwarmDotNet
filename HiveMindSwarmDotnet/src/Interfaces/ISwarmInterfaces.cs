using HiveMindSwarmDotnet.Console.Models;

namespace HiveMindSwarmDotnet.Console.Interfaces;

public interface ISwarmAgent
{
    string Id { get; }
    AgentRole Role { get; }
    bool IsAvailable { get; }
    Task<AgentResponse> ProcessTaskAsync(AgentTask task, CancellationToken cancellationToken = default);
    Task<bool> ValidateResponseAsync(AgentResponse response, CancellationToken cancellationToken = default);
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task DisposeAsync();
}

public interface ISwarmOrchestrator
{
    Task<SwarmResult> ExecuteTaskAsync(SwarmTask task, CancellationToken cancellationToken = default);
    Task<SwarmMetrics> GetMetricsAsync();
    Task InitializeAsync(CancellationToken cancellationToken = default);
    Task DisposeAsync();
}

public interface IAgentPool
{
    Task<ISwarmAgent> GetAgentAsync(AgentRole role, CancellationToken cancellationToken = default);
    Task ReturnAgentAsync(ISwarmAgent agent, CancellationToken cancellationToken = default);
    Task<IEnumerable<ISwarmAgent>> GetAvailableAgentsAsync(CancellationToken cancellationToken = default);
    Task<SwarmMetrics> GetPoolMetricsAsync();
    Task InitializePoolAsync(CancellationToken cancellationToken = default);
    Task DisposeAsync();
}

public interface ITaskDistributor
{
    Task<List<AgentTask>> DistributeTaskAsync(SwarmTask swarmTask, CancellationToken cancellationToken = default);
    Task<AgentTask> GetNextTaskAsync(string agentId, CancellationToken cancellationToken = default);
    Task UpdateTaskStatusAsync(string taskId, SwarmTaskStatus status, CancellationToken cancellationToken = default);
}

public interface ICommunicationHub
{
    Task SendMessageAsync(AgentMessage message, CancellationToken cancellationToken = default);
    Task BroadcastAsync(string fromAgentId, AgentMessage message, CancellationToken cancellationToken = default);
    Task<List<AgentMessage>> GetMessagesAsync(string agentId, CancellationToken cancellationToken = default);
    Task<List<CommunicationLink>> GetCommunicationLinksAsync(string agentId, CancellationToken cancellationToken = default);
    Task OptimizeCommunicationAsync(CancellationToken cancellationToken = default);
    Task RegisterAgentAsync(string agentId, CancellationToken cancellationToken = default);
    Task UnregisterAgentAsync(string agentId, CancellationToken cancellationToken = default);
}

public interface IOptimizationEngine
{
    Task<List<CommunicationLink>> OptimizeCommunicationLinksAsync(
        List<CommunicationLink> links, 
        SwarmMetrics metrics,
        CancellationToken cancellationToken = default);
    
    Task<Dictionary<AgentRole, double>> CalculateAgentPerformanceAsync(
        List<AgentResponse> responses,
        CancellationToken cancellationToken = default);
    
    Task AdaptAgentBehaviorAsync(
        string agentId, 
        Dictionary<string, object> performanceData,
        CancellationToken cancellationToken = default);
}

public interface ISwarmConfiguration
{
    int MaxAgents { get; }
    string DefaultModel { get; }
    OptimizationStrategy OptimizationStrategy { get; }
    int CommunicationTimeout { get; }
    int TaskTimeout { get; }
    bool EnableRiskAssessmentAgent { get; }
    Dictionary<AgentRole, AgentRoleConfiguration> AgentRoles { get; }
    OllamaConfiguration Ollama { get; }
}

public interface ISwarmLogger
{
    Task LogTaskStartAsync(SwarmTask task);
    Task LogTaskCompletedAsync(SwarmTask task, SwarmResult result);
    Task LogAgentResponseAsync(AgentResponse response);
    Task LogCommunicationAsync(AgentMessage message);
    Task LogErrorAsync(string error, Exception? exception = null);
}

public class AgentRoleConfiguration
{
    public string ModelId { get; set; } = string.Empty;
    public string SystemPrompt { get; set; } = string.Empty;
    public int MaxTokens { get; set; } = 2000;
    public double Temperature { get; set; } = 0.7;
    public Dictionary<string, object> AdditionalParameters { get; set; } = new();
}

public class OllamaConfiguration
{
    public string Endpoint { get; set; } = "http://localhost:11434";
    public List<string> Models { get; set; } = new();
    public int TimeoutSeconds { get; set; } = 30;
    public int MaxRetries { get; set; } = 3;
}