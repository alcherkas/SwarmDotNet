namespace ByzantineSwarmDotnet.Models;

// Enums

// Core Data Models
public record AgentInfo
{
    public string Id { get; init; } = string.Empty;
    public AgentRole Role { get; init; }
    public AgentHierarchyLevel HierarchyLevel { get; init; }
    public double ReputationScore { get; init; }
    public List<string> Capabilities { get; init; } = new();
    public DateTime LastHeartbeat { get; init; }
    public bool IsActive { get; init; }
    public string PublicKey { get; init; } = string.Empty;
    public int CurrentLoad { get; init; }
    public TimeSpan AverageResponseTime { get; init; }
}

// Communication Models

// Consensus Models

// Reputation Models

// Message Types for PBFT

// Authentication Models

// Task Distribution Models

// Exception Models

// Metrics Models