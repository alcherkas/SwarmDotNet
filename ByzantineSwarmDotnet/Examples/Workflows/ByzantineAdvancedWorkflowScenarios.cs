using ByzantineSwarmDotnet.Examples.Interfaces;
using ByzantineSwarmDotnet.Examples.Models;
using ByzantineSwarmDotnet.Examples.SampleData;
using Microsoft.Extensions.Logging;

namespace ByzantineSwarmDotnet.Examples.Workflows;

/// <summary>
/// Demonstrates advanced Byzantine workflow scenarios
/// </summary>
public class ByzantineAdvancedWorkflowScenarios
{
    private readonly IByzantineTolerantSwarmOrchestrator _orchestrator;
    private readonly ILogger<ByzantineAdvancedWorkflowScenarios> _logger;

    public ByzantineAdvancedWorkflowScenarios(IByzantineTolerantSwarmOrchestrator orchestrator, ILogger<ByzantineAdvancedWorkflowScenarios> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    /// <summary>
    /// Demonstrates parallel Byzantine workflow execution with fault tolerance
    /// </summary>
    public async Task RunParallelWorkflowAsync()
    {
        _logger.LogInformation("🚀 Running Parallel Byzantine Workflow Example");
        
        var tasks = new[]
        {
            SamplePRData.CreateByzantineBugFixPR(),
            SamplePRData.CreateByzantineFeaturePR(),
            SamplePRData.CreateByzantineSecurityPR()
        };

        _logger.LogInformation($"📋 Processing {tasks.Length} PRs in parallel with Byzantine tolerance...");

        var startTime = DateTime.UtcNow;
        
        // Execute all tasks in parallel with Byzantine fault tolerance
        var results = await Task.WhenAll(
            tasks.Select(task => _orchestrator.ExecuteTaskAsync(task))
        );

        var totalTime = DateTime.UtcNow - startTime;
        
        _logger.LogInformation($"✅ Parallel Byzantine execution completed in {totalTime.TotalSeconds:F1}s");
        _logger.LogInformation($"📊 Results: {results.Count(r => r.Success)}/{results.Length} successful");
        _logger.LogInformation($"⚖️ Consensus: {results.Count(r => r.ConsensusReached)}/{results.Length} reached consensus");
        _logger.LogInformation($"🛡️ Average Trust Score: {results.Average(r => r.TrustScore):F2}");
        _logger.LogInformation($"🚨 Total Byzantine Faults: {results.Sum(r => r.ByzantineFaultsDetected)}");
        
        for (int i = 0; i < results.Length; i++)
        {
            _logger.LogInformation($"   Task {i + 1}: Trust={results[i].TrustScore:F2}, Consensus={results[i].ConsensusReached}, Faults={results[i].ByzantineFaultsDetected}");
        }
    }

    /// <summary>
    /// Demonstrates adaptive Byzantine workflow based on threat assessment
    /// </summary>
    public async Task RunAdaptiveWorkflowAsync()
    {
        _logger.LogInformation("🧠 Running Adaptive Byzantine Workflow Example");
        
        // Start with initial threat assessment
        var initialTask = new SwarmTask
        {
            Description = "Initial threat assessment for adaptive Byzantine workflow",
            RequiredRoles = new[] { "PRExtractor", "CodeAnalyzer", "RiskAssessment" },
            RequiredConsensus = true,
            FaultToleranceLevel = FaultToleranceLevel.Byzantine,
            SecurityLevel = SecurityLevel.Medium,
            Parameters = new Dictionary<string, object>
            {
                ["pr_title"] = "Update authentication system",
                ["files_changed"] = new[] { "src/auth/JwtValidator.cs", "src/auth/UserManager.cs" },
                ["threat_assessment"] = true
            }
        };

        var initialResult = await _orchestrator.ExecuteTaskAsync(initialTask);
        
        _logger.LogInformation($"📊 Initial threat assessment: Trust={initialResult.TrustScore:F2}, Consensus={initialResult.ConsensusReached}");

        // Adapt workflow based on threat level and consensus
        var nextRoles = new List<string> { "Validator" };
        var securityLevel = SecurityLevel.Medium;
        var faultToleranceLevel = FaultToleranceLevel.Byzantine;
        
        if (initialResult.TrustScore < 0.7)
        {
            _logger.LogInformation("🔍 Low trust score detected - escalating security measures");
            nextRoles.AddRange(new[] { "Security", "ReputationManager" });
            securityLevel = SecurityLevel.High;
        }

        if (initialResult.ByzantineFaultsDetected > 0)
        {
            _logger.LogInformation("🚨 Byzantine faults detected - maximum security protocol");
            nextRoles.AddRange(new[] { "Security", "Monitoring" });
            securityLevel = SecurityLevel.Critical;
        }

        // Check for security-sensitive changes
        if (initialResult.Result.Contains("auth", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogInformation("🔒 Security-sensitive code detected - adding enhanced validation");
            nextRoles.AddRange(new[] { "RiskAssessment", "IntegrationAnalyzer" });
            securityLevel = SecurityLevel.Critical;
        }

        // Always add summary generator
        nextRoles.Add("SummaryGenerator");

        if (nextRoles.Any())
        {
            var adaptiveTask = new SwarmTask
            {
                Description = "Adaptive Byzantine follow-up analysis based on threat assessment",
                RequiredRoles = nextRoles.ToArray(),
                RequiredConsensus = true,
                FaultToleranceLevel = faultToleranceLevel,
                SecurityLevel = securityLevel,
                Parameters = new Dictionary<string, object>(initialTask.Parameters)
                {
                    ["adaptive_escalation"] = true,
                    ["initial_trust_score"] = initialResult.TrustScore,
                    ["threat_level"] = securityLevel.ToString()
                }
            };

            var finalResult = await _orchestrator.ExecuteTaskAsync(adaptiveTask);
            
            _logger.LogInformation($"✅ Adaptive Byzantine workflow completed:");
            _logger.LogInformation($"   🎯 Final Trust Score: {finalResult.TrustScore:F2}");
            _logger.LogInformation($"   ⚖️ Consensus: {finalResult.ConsensusReached}");
            _logger.LogInformation($"   🛡️ Security Level: {securityLevel}");
            _logger.LogInformation($"   🚨 Total Faults: {initialResult.ByzantineFaultsDetected + finalResult.ByzantineFaultsDetected}");
            _logger.LogInformation($"   👥 Total Agents: {initialResult.ParticipatingAgents + finalResult.ParticipatingAgents}");
        }
    }

    /// <summary>
    /// Demonstrates continuous learning workflow with reputation tracking
    /// </summary>
    public async Task RunLearningWorkflowAsync()
    {
        _logger.LogInformation("📚 Running Byzantine Learning Workflow Example");
        
        var learningTask = new SwarmTask
        {
            Description = "Analyze agent performance and reputation patterns for continuous improvement",
            RequiredRoles = new[] { "Learning", "ReputationManager", "Validator", "SummaryGenerator" },
            RequiredConsensus = true,
            FaultToleranceLevel = FaultToleranceLevel.Byzantine,
            SecurityLevel = SecurityLevel.Medium,
            Parameters = new Dictionary<string, object>
            {
                ["historical_data"] = new
                {
                    previous_reviews = 250,
                    accuracy_rate = 0.91,
                    byzantine_attacks_detected = 15,
                    false_positives = 8,
                    consensus_success_rate = 0.94,
                    average_trust_score = 0.82,
                    reputation_improvements = 45
                },
                ["recent_performance"] = new
                {
                    consensus_failures = 2,
                    byzantine_faults_detected = 5,
                    reputation_updates = 12,
                    trust_score_improvements = 8,
                    security_incidents_prevented = 3
                },
                ["improvement_focus"] = new
                {
                    consensus_optimization = true,
                    reputation_accuracy = true,
                    byzantine_detection = true,
                    recovery_speed = true
                }
            }
        };

        var result = await _orchestrator.ExecuteTaskAsync(learningTask);
        
        _logger.LogInformation("📈 Byzantine Learning Analysis Results:");
        _logger.LogInformation($"   🎯 Current system accuracy: 91%");
        _logger.LogInformation($"   ⚖️ Consensus success rate: 94%");
        _logger.LogInformation($"   🛡️ Average trust score: 0.82");
        _logger.LogInformation($"   🚨 Byzantine attacks detected: 15");
        _logger.LogInformation($"   📊 Learning confidence: {result.TrustScore:F2}");
        _logger.LogInformation($"   🔧 Improvement recommendations: {result.Result}");
    }

    /// <summary>
    /// Demonstrates Byzantine fault tolerance under high load
    /// </summary>
    public async Task RunByzantineFaultToleranceExampleAsync()
    {
        _logger.LogInformation("🛡️ Running Byzantine Fault Tolerance Example");
        
        var faultToleranceTask = new SwarmTask
        {
            Description = "Test Byzantine fault tolerance under simulated high load and adversarial conditions",
            RequiredRoles = new[] { "Orchestrator", "Validator", "Security", "Monitoring", "SummaryGenerator" },
            RequiredConsensus = true,
            FaultToleranceLevel = FaultToleranceLevel.Byzantine,
            SecurityLevel = SecurityLevel.Critical,
            Parameters = new Dictionary<string, object>
            {
                ["load_simulation"] = new
                {
                    concurrent_requests = 100,
                    malicious_agents = 25,
                    attack_patterns = new[] { "consensus_disruption", "reputation_manipulation", "timing_attacks" },
                    duration_minutes = 5,
                    fault_injection_rate = 0.2
                },
                ["tolerance_testing"] = new
                {
                    max_faults_tolerated = 7,
                    consensus_threshold = 0.67,
                    recovery_time_limit_seconds = 30,
                    reputation_decay_simulation = true
                }
            }
        };

        var result = await _orchestrator.ExecuteTaskAsync(faultToleranceTask);
        
        _logger.LogInformation("🛡️ Byzantine Fault Tolerance Results:");
        _logger.LogInformation($"   ✅ System Stability: {result.Success}");
        _logger.LogInformation($"   ⚖️ Consensus Maintained: {result.ConsensusReached}");
        _logger.LogInformation($"   🎯 Final Trust Score: {result.TrustScore:F2}");
        _logger.LogInformation($"   🚨 Faults Handled: {result.ByzantineFaultsDetected}");
        _logger.LogInformation($"   ⏱️ Response Time: {result.TotalExecutionTime.TotalSeconds:F1}s");
        _logger.LogInformation($"   📊 Throughput: {result.Metadata.GetValueOrDefault("throughput", 0)} requests/second");
    }

    /// <summary>
    /// Demonstrates consensus algorithm effectiveness
    /// </summary>
    public async Task RunConsensusAlgorithmExampleAsync()
    {
        _logger.LogInformation("⚖️ Running Consensus Algorithm Example");
        
        var consensusTask = new SwarmTask
        {
            Description = "Demonstrate PBFT consensus algorithm effectiveness under various conditions",
            RequiredRoles = new[] { "Orchestrator", "Validator", "PRExtractor", "CodeAnalyzer", "SummaryGenerator" },
            RequiredConsensus = true,
            FaultToleranceLevel = FaultToleranceLevel.Byzantine,
            SecurityLevel = SecurityLevel.High,
            Parameters = new Dictionary<string, object>
            {
                ["consensus_testing"] = new
                {
                    algorithm = "PBFT",
                    participant_count = 16,
                    byzantine_nodes = 5,
                    network_partitions = 2,
                    message_delays = true,
                    timeout_scenarios = true
                },
                ["validation_scenarios"] = new
                {
                    normal_operation = true,
                    under_attack = true,
                    network_issues = true,
                    node_failures = true,
                    recovery_testing = true
                }
            }
        };

        var result = await _orchestrator.ExecuteTaskAsync(consensusTask);
        
        _logger.LogInformation("⚖️ Consensus Algorithm Results:");
        _logger.LogInformation($"   🎯 Consensus Success: {result.ConsensusReached}");
        _logger.LogInformation($"   📊 Confidence Level: {result.ConsensusConfidence:P}");
        _logger.LogInformation($"   🛡️ Byzantine Tolerance: {result.ByzantineFaultsDetected} faults handled");
        _logger.LogInformation($"   ⏱️ Consensus Time: {result.TotalExecutionTime.TotalSeconds:F1}s");
        _logger.LogInformation($"   👥 Participating Agents: {result.ParticipatingAgents}");
        _logger.LogInformation($"   🔄 Recovery Actions: {result.Metadata.GetValueOrDefault("recovery_actions", 0)}");
    }

    /// <summary>
    /// Demonstrates reputation system under Byzantine attacks
    /// </summary>
    public async Task RunReputationSystemExampleAsync()
    {
        _logger.LogInformation("⭐ Running Reputation System Example");
        
        var reputationTask = new SwarmTask
        {
            Description = "Test reputation system resilience under Byzantine attacks and manipulation attempts",
            RequiredRoles = new[] { "ReputationManager", "Validator", "Security", "SummaryGenerator" },
            RequiredConsensus = true,
            FaultToleranceLevel = FaultToleranceLevel.Byzantine,
            SecurityLevel = SecurityLevel.High,
            Parameters = new Dictionary<string, object>
            {
                ["reputation_testing"] = new
                {
                    initial_reputation_distribution = new { high = 5, medium = 8, low = 3 },
                    manipulation_attempts = 12,
                    collusion_scenarios = 3,
                    false_positive_injection = 5,
                    recovery_validation = true
                },
                ["attack_scenarios"] = new
                {
                    reputation_bombing = true,
                    collusive_promotion = true,
                    sybil_attacks = true,
                    reputation_laundering = true,
                    whitewashing_attempts = true
                }
            }
        };

        var result = await _orchestrator.ExecuteTaskAsync(reputationTask);
        
        _logger.LogInformation("⭐ Reputation System Results:");
        _logger.LogInformation($"   🛡️ System Integrity: {result.Success}");
        _logger.LogInformation($"   📊 Average Reputation: {result.AverageReputationScore:F2}");
        _logger.LogInformation($"   🎯 Trust Score: {result.TrustScore:F2}");
        _logger.LogInformation($"   🚨 Attacks Detected: {result.ByzantineFaultsDetected}");
        _logger.LogInformation($"   🔄 Reputation Updates: {result.Metadata.GetValueOrDefault("reputation_updates", 0)}");
        _logger.LogInformation($"   ⚖️ Consensus on Updates: {result.ConsensusReached}");
        _logger.LogInformation($"   🛡️ Manipulation Attempts Blocked: {result.Metadata.GetValueOrDefault("manipulation_blocked", 0)}");
    }
}