namespace ByzantineSwarmDotnet.Models;

public record AgentTask
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string SwarmTaskId { get; init; } = string.Empty;
    public string AssignedAgentId { get; init; } = string.Empty;
    public TaskType Type { get; init; }
    public string Description { get; init; } = string.Empty;
    public Dictionary<string, object> Parameters { get; init; } = new();
    public List<string> Dependencies { get; init; } = new();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public DateTime? StartedAt { get; init; }
    public DateTime? CompletedAt { get; init; }
    public bool IsCompleted { get; init; }
    public bool IsSuccessful { get; init; }
    
    public T GetPayload<T>() where T : class
    {
        if (Parameters.TryGetValue("payload", out var payload) && payload is T typedPayload)
        {
            return typedPayload;
        }
        throw new InvalidOperationException($"Payload of type {typeof(T).Name} not found");
    }
}