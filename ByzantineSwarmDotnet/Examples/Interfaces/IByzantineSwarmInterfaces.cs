using ByzantineSwarmDotnet.Examples.Models;

namespace ByzantineSwarmDotnet.Examples.Interfaces;

// Simplified interfaces for examples
public interface IByzantineTolerantSwarmOrchestrator
{
    Task<SwarmResult> ExecuteTaskAsync(SwarmTask task);
}

// Stub implementation for examples
public class MockByzantineTolerantSwarmOrchestrator : IByzantineTolerantSwarmOrchestrator
{
    private readonly Random _random = new();

    public async Task<SwarmResult> ExecuteTaskAsync(SwarmTask task)
    {
        // Simulate processing time
        await Task.Delay(_random.Next(1000, 3000));

        // Simulate Byzantine swarm execution with realistic results
        var participatingAgents = _random.Next(8, 16);
        var byzantineFaults = task.FaultToleranceLevel == FaultToleranceLevel.Byzantine 
            ? _random.Next(0, Math.Max(1, participatingAgents / 4)) 
            : 0;

        var consensusReached = _random.NextDouble() > 0.1; // 90% success rate
        var trustScore = consensusReached ? 0.7 + (_random.NextDouble() * 0.3) : 0.3 + (_random.NextDouble() * 0.4);

        return new SwarmResult
        {
            TaskId = task.Id,
            Success = consensusReached && byzantineFaults < participatingAgents / 3,
            ConsensusReached = consensusReached,
            TrustScore = trustScore,
            ParticipatingAgents = participatingAgents,
            ConsensusConfidence = consensusReached ? 0.8 + (_random.NextDouble() * 0.2) : 0.4 + (_random.NextDouble() * 0.3),
            AverageReputationScore = 0.6 + (_random.NextDouble() * 0.4),
            ByzantineFaultsDetected = byzantineFaults,
            Result = GenerateExampleResult(task),
            TotalExecutionTime = TimeSpan.FromSeconds(_random.Next(5, 30)),
            Metadata = GenerateExampleMetadata(task, byzantineFaults),
            Warnings = byzantineFaults > 0 ? new List<string> { $"{byzantineFaults} Byzantine faults detected and handled" } : new(),
            Errors = consensusReached ? new() : new List<string> { "Consensus could not be reached within timeout" }
        };
    }

    private string GenerateExampleResult(SwarmTask task)
    {
        var results = new[]
        {
            $"✅ Analysis completed for: {task.Description}",
            $"🔍 Comprehensive review of {task.RequiredRoles.Length} specialized areas",
            $"🛡️ Byzantine fault tolerance: {task.FaultToleranceLevel}",
            $"🔐 Security level: {task.SecurityLevel}",
            "📊 All validation checks passed",
            "💡 No critical issues identified",
            "⚡ Performance impact: Minimal",
            "🎯 Recommendation: APPROVED for deployment"
        };

        return string.Join("\n", results);
    }

    private Dictionary<string, object> GenerateExampleMetadata(SwarmTask task, int byzantineFaults)
    {
        return new Dictionary<string, object>
        {
            ["security_validation"] = task.SecurityLevel != SecurityLevel.Low,
            ["cryptographic_integrity"] = true,
            ["signatures_valid"] = true,
            ["attacks_detected"] = byzantineFaults,
            ["attacks_blocked"] = byzantineFaults,
            ["malicious_agents_detected"] = byzantineFaults,
            ["recovery_actions"] = byzantineFaults > 0 ? byzantineFaults : 0,
            ["recovery_time_ms"] = byzantineFaults > 0 ? _random.Next(100, 500) : 0,
            ["consensus_impact"] = byzantineFaults > 0 ? _random.NextDouble() * 0.2 : 0.0,
            ["resilience_score"] = 0.9 - (byzantineFaults * 0.1),
            ["quarantine_actions"] = byzantineFaults,
            ["reputation_updates"] = _random.Next(0, 5),
            ["manipulation_blocked"] = _random.Next(0, 3),
            ["throughput"] = _random.Next(50, 200)
        };
    }
}