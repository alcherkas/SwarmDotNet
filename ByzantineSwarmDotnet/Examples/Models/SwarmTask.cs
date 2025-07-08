namespace ByzantineSwarmDotnet.Examples.Models;

// Simplified models for examples - just enough to demonstrate functionality

public record SwarmTask
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string Description { get; init; } = string.Empty;
    public string[] RequiredRoles { get; init; } = Array.Empty<string>();
    public TimeSpan MaxExecutionTime { get; init; } = TimeSpan.FromMinutes(5);
    public bool RequiredConsensus { get; init; }
    public FaultToleranceLevel FaultToleranceLevel { get; init; } = FaultToleranceLevel.Basic;
    public SecurityLevel SecurityLevel { get; init; } = SecurityLevel.Medium;
    public Dictionary<string, object> Parameters { get; init; } = new();
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
    public string CreatedBy { get; init; } = string.Empty;
}