using ByzantineSwarmDotnet.Models;

namespace ByzantineSwarmDotnet.Interfaces;

// Core Orchestrator Interface
public interface IByzantineTolerantSwarmOrchestrator
{
    Task<SwarmResult> ExecuteTaskAsync(SwarmTask task);
    Task<bool> ValidateByzantineToleranceRequirements();
    Task<SwarmMetrics> GetRealTimeMetricsAsync();
    Task InitiateRecoveryAsync();
    Task QuarantineMaliciousAgentsAsync(List<string> maliciousAgents);
    Task InvestigateConsensusIssuesAsync();
    Task EscalateByzantineDetectionAsync();
}

// Agent Pool Interface

// Task Distribution Interface

// Communication Interface

// PBFT Consensus Interface

// Reputation System Interface

// Fault Detector Interface

// Circuit Breaker Interface

// Cryptographic Services Interface

// Byzantine Detector Interface

// Gossip Protocol Interface

// Network Partition Detector Interface

// Agent Interface

// Specialized Agent Interfaces

// Storage Interfaces

// Monitoring Interfaces

// Load Balancing Interface