using ByzantineSwarmDotnet.Examples.Models;
using ByzantineSwarmDotnet.Examples.Interfaces;
using ByzantineSwarmDotnet.Examples.SampleData;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace ByzantineSwarmDotnet.Examples.Workflows;

/// <summary>
/// Demonstrates a complete Byzantine-tolerant code review workflow using the agent swarm
/// Shows step-by-step agent coordination with consensus mechanisms and fault tolerance
/// </summary>
public class ByzantineCodeReviewWorkflow
{
    private readonly IByzantineTolerantSwarmOrchestrator _orchestrator;
    private readonly ILogger<ByzantineCodeReviewWorkflow> _logger;

    public ByzantineCodeReviewWorkflow(IByzantineTolerantSwarmOrchestrator orchestrator, ILogger<ByzantineCodeReviewWorkflow> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    /// <summary>
    /// Demonstrates the complete Byzantine-tolerant PR analysis workflow with detailed agent coordination
    /// </summary>
    public async Task RunCompleteWorkflowAsync()
    {
        _logger.LogInformation("🚀 Starting Complete Byzantine Code Review Workflow");
        _logger.LogInformation("=======================================================");

        try
        {
            // Step 1: Initialize the Byzantine workflow
            await InitializeByzantineWorkflowAsync();

            // Step 2: Run the Byzantine bug fix analysis workflow
            await RunByzantineBugFixWorkflowAsync();

            // Step 3: Run the Byzantine feature development workflow
            await RunByzantineFeatureWorkflowAsync();

            // Step 4: Run the maximum security review workflow
            await RunMaximumSecurityWorkflowAsync();

            // Step 5: Run Byzantine attack simulation
            await RunByzantineAttackSimulationAsync();

            _logger.LogInformation("✅ Complete Byzantine workflow finished successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Byzantine workflow execution failed");
            throw;
        }
    }

    private async Task InitializeByzantineWorkflowAsync()
    {
        _logger.LogInformation("\n🔧 Step 1: Initializing Byzantine Workflow");
        _logger.LogInformation("- Setting up Byzantine-tolerant agent swarm");
        _logger.LogInformation("- Configuring consensus mechanisms");
        _logger.LogInformation("- Establishing cryptographic verification");
        _logger.LogInformation("- Initializing reputation system");
        _logger.LogInformation("- Setting up fault detection systems");
        
        await Task.Delay(2000); // Simulate Byzantine initialization
        _logger.LogInformation("✅ Byzantine workflow initialized with fault tolerance");
    }

    /// <summary>
    /// Demonstrates Byzantine bug fix review workflow with consensus
    /// </summary>
    private async Task RunByzantineBugFixWorkflowAsync()
    {
        _logger.LogInformation("\n🐛 Step 2: Byzantine Bug Fix Review Workflow");
        _logger.LogInformation("===============================================");

        var bugFixTask = SamplePRData.CreateByzantineBugFixPR();
        
        _logger.LogInformation($"📋 Analyzing PR: {bugFixTask.Description}");
        _logger.LogInformation($"🎯 Required Agents: {string.Join(", ", bugFixTask.RequiredRoles)}");
        _logger.LogInformation($"🛡️ Fault Tolerance: {bugFixTask.FaultToleranceLevel}");
        _logger.LogInformation($"🔐 Security Level: {bugFixTask.SecurityLevel}");
        _logger.LogInformation($"⚖️ Consensus Required: {bugFixTask.RequiredConsensus}");

        // Execute the task with detailed Byzantine logging
        var result = await ExecuteByzantineTaskWithDetailedLoggingAsync(bugFixTask, "Byzantine Bug Fix Analysis");

        // Show Byzantine workflow-specific insights
        _logger.LogInformation("\n📊 Byzantine Bug Fix Workflow Results:");
        ShowByzantineWorkflowResults(result);
    }

    /// <summary>
    /// Demonstrates Byzantine new feature review workflow with enhanced consensus
    /// </summary>
    private async Task RunByzantineFeatureWorkflowAsync()
    {
        _logger.LogInformation("\n⭐ Step 3: Byzantine Feature Review Workflow");
        _logger.LogInformation("=============================================");

        var featureTask = SamplePRData.CreateByzantineFeaturePR();
        
        _logger.LogInformation($"📋 Analyzing PR: {featureTask.Description}");
        _logger.LogInformation($"🎯 Required Agents: {string.Join(", ", featureTask.RequiredRoles)}");
        _logger.LogInformation($"🛡️ Fault Tolerance: {featureTask.FaultToleranceLevel}");
        _logger.LogInformation($"🔐 Security Level: {featureTask.SecurityLevel}");

        // Execute the task with Byzantine coordination details
        var result = await ExecuteByzantineTaskWithDetailedLoggingAsync(featureTask, "Byzantine Feature Development Analysis");

        // Show Byzantine feature-specific insights
        _logger.LogInformation("\n📊 Byzantine Feature Workflow Results:");
        ShowByzantineWorkflowResults(result);
        
        // Show Byzantine agent coordination patterns
        ShowByzantineAgentCoordinationInsights(result);
    }

    /// <summary>
    /// Demonstrates maximum security review workflow with full Byzantine protection
    /// </summary>
    private async Task RunMaximumSecurityWorkflowAsync()
    {
        _logger.LogInformation("\n🔒 Step 4: Maximum Security Review Workflow");
        _logger.LogInformation("===========================================");

        var securityTask = SamplePRData.CreateByzantineSecurityPR();
        
        _logger.LogInformation($"📋 Analyzing PR: {securityTask.Description}");
        _logger.LogInformation($"🎯 Required Agents: {string.Join(", ", securityTask.RequiredRoles)}");
        _logger.LogInformation($"🛡️ Fault Tolerance: {securityTask.FaultToleranceLevel}");
        _logger.LogInformation($"🔐 Security Level: {securityTask.SecurityLevel}");

        // Execute maximum security analysis
        var result = await ExecuteByzantineTaskWithDetailedLoggingAsync(securityTask, "Maximum Security Review Analysis");

        // Show security-specific insights
        _logger.LogInformation("\n📊 Maximum Security Workflow Results:");
        ShowByzantineWorkflowResults(result);
        ShowByzantineSecurityInsights(result);
    }

    /// <summary>
    /// Demonstrates Byzantine attack simulation and system resilience
    /// </summary>
    private async Task RunByzantineAttackSimulationAsync()
    {
        _logger.LogInformation("\n🛡️ Step 5: Byzantine Attack Simulation");
        _logger.LogInformation("======================================");

        var attackTask = SamplePRData.CreateByzantineAttackSimulation();
        
        _logger.LogInformation($"📋 Simulating Attack: {attackTask.Description}");
        _logger.LogInformation($"🎯 Required Agents: {string.Join(", ", attackTask.RequiredRoles)}");
        _logger.LogInformation($"⚠️ Attack Type: Coordinated Byzantine");
        _logger.LogInformation($"📊 Malicious Agent Percentage: 30%");

        try
        {
            // Execute attack simulation
            var result = await ExecuteByzantineTaskWithDetailedLoggingAsync(attackTask, "Byzantine Attack Simulation");

            // Show attack simulation results
            _logger.LogInformation("\n📊 Byzantine Attack Simulation Results:");
            ShowByzantineAttackResults(result);
        }
        catch (ByzantineAttackDetectedException ex)
        {
            _logger.LogWarning("🔴 Byzantine attack was detected and handled:");
            _logger.LogWarning($"   Malicious Agents: {string.Join(", ", ex.MaliciousAgents)}");
            _logger.LogWarning($"   Attack Details: {ex.Message}");
            _logger.LogInformation("✅ System successfully defended against Byzantine attack");
        }
    }

    /// <summary>
    /// Executes a Byzantine task with detailed logging of agent coordination and consensus
    /// </summary>
    private async Task<SwarmResult> ExecuteByzantineTaskWithDetailedLoggingAsync(SwarmTask task, string workflowName)
    {
        _logger.LogInformation($"\n🔄 Executing {workflowName}...");
        
        var startTime = DateTime.UtcNow;
        
        // Simulate Byzantine agent initialization
        _logger.LogInformation("🔧 Initializing Byzantine agents...");
        await Task.Delay(1000);
        
        // Simulate consensus preparation
        _logger.LogInformation("⚖️ Preparing consensus mechanisms...");
        await Task.Delay(500);
        
        // Simulate cryptographic setup
        _logger.LogInformation("🔐 Setting up cryptographic verification...");
        await Task.Delay(500);
        
        // Simulate step-by-step agent execution with Byzantine considerations
        foreach (var role in task.RequiredRoles)
        {
            _logger.LogInformation($"   🤖 Activating {role} agent with Byzantine validation...");
            await Task.Delay(800); // Longer processing time for Byzantine validation
        }

        // Simulate consensus phase
        _logger.LogInformation("⚖️ Initiating consensus protocol...");
        await Task.Delay(1500);
        
        // Simulate reputation validation
        _logger.LogInformation("⭐ Validating agent reputations...");
        await Task.Delay(500);

        // Execute the actual task
        var result = await _orchestrator.ExecuteTaskAsync(task);
        
        var duration = DateTime.UtcNow - startTime;
        
        _logger.LogInformation($"✅ {workflowName} completed in {duration.TotalSeconds:F1}s");
        _logger.LogInformation($"   🎯 Success: {result.Success}");
        _logger.LogInformation($"   ⚖️ Consensus Reached: {result.ConsensusReached}");
        _logger.LogInformation($"   🛡️ Trust Score: {result.TrustScore:F2}");
        _logger.LogInformation($"   🚨 Byzantine Faults: {result.ByzantineFaultsDetected}");
        
        return result;
    }

    /// <summary>
    /// Shows detailed Byzantine workflow results
    /// </summary>
    private void ShowByzantineWorkflowResults(SwarmResult result)
    {
        _logger.LogInformation($"   ⏱️  Total Execution Time: {result.TotalExecutionTime.TotalSeconds:F1}s");
        _logger.LogInformation($"   🤖 Participating Agents: {result.ParticipatingAgents}");
        _logger.LogInformation($"   ⚖️ Consensus Reached: {result.ConsensusReached}");
        _logger.LogInformation($"   📊 Consensus Confidence: {result.ConsensusConfidence:P}");
        _logger.LogInformation($"   🛡️ Trust Score: {result.TrustScore:F2}");
        _logger.LogInformation($"   ⭐ Average Reputation: {result.AverageReputationScore:F2}");
        _logger.LogInformation($"   🚨 Byzantine Faults Detected: {result.ByzantineFaultsDetected}");
        
        if (result.Warnings.Any())
        {
            _logger.LogWarning("⚠️  Warnings:");
            foreach (var warning in result.Warnings)
            {
                _logger.LogWarning($"   - {warning}");
            }
        }
        
        if (result.Errors.Any())
        {
            _logger.LogError("❌ Errors:");
            foreach (var error in result.Errors)
            {
                _logger.LogError($"   - {error}");
            }
        }
        
        _logger.LogInformation($"   📝 Final Result: {TruncateString(result.Result, 200)}");
    }

    /// <summary>
    /// Shows Byzantine agent coordination insights
    /// </summary>
    private void ShowByzantineAgentCoordinationInsights(SwarmResult result)
    {
        _logger.LogInformation("\n🔗 Byzantine Agent Coordination Insights:");
        
        // Show consensus participation
        _logger.LogInformation($"   ⚖️ Consensus Participation: {result.ConsensusConfidence:P}");
        _logger.LogInformation($"   🎯 Agreement Level: {result.TrustScore:F2}");
        
        // Show reputation distribution
        _logger.LogInformation($"   ⭐ Average Reputation Score: {result.AverageReputationScore:F2}");
        
        // Show fault tolerance metrics
        if (result.ByzantineFaultsDetected > 0)
        {
            _logger.LogInformation($"   🛡️ Fault Tolerance: {result.ByzantineFaultsDetected} faults detected and handled");
        }
        else
        {
            _logger.LogInformation($"   ✅ Fault Tolerance: No Byzantine faults detected");
        }
        
        // Show cryptographic verification
        var signaturesValid = (bool)(result.Metadata.GetValueOrDefault("signatures_valid", false));
        _logger.LogInformation($"   🔐 Cryptographic Verification: {(signaturesValid ? "✅ Valid" : "❌ Invalid")}");
        
        // Show recovery actions
        var recoveryActions = result.Metadata.GetValueOrDefault("recovery_actions", 0);
        if ((int)recoveryActions > 0)
        {
            _logger.LogInformation($"   🔄 Recovery Actions: {recoveryActions} automatic recoveries performed");
        }
    }

    /// <summary>
    /// Shows Byzantine security-specific insights
    /// </summary>
    private void ShowByzantineSecurityInsights(SwarmResult result)
    {
        _logger.LogInformation("\n🔐 Byzantine Security Analysis Insights:");
        
        // Show security validation results
        var securityValidation = (bool)(result.Metadata.GetValueOrDefault("security_validation", false));
        _logger.LogInformation($"   🛡️ Security Validation: {(securityValidation ? "✅ Passed" : "❌ Failed")}");
        
        // Show cryptographic integrity
        var cryptoIntegrity = (bool)(result.Metadata.GetValueOrDefault("cryptographic_integrity", false));
        _logger.LogInformation($"   🔐 Cryptographic Integrity: {(cryptoIntegrity ? "✅ Maintained" : "❌ Compromised")}");
        
        // Show attack detection
        var attacksDetected = result.Metadata.GetValueOrDefault("attacks_detected", 0);
        _logger.LogInformation($"   🚨 Attacks Detected: {attacksDetected}");
        
        // Show quarantine actions
        var quarantineActions = result.Metadata.GetValueOrDefault("quarantine_actions", 0);
        if ((int)quarantineActions > 0)
        {
            _logger.LogInformation($"   🚫 Quarantine Actions: {quarantineActions} malicious agents quarantined");
        }
        
        // Show security score
        var securityScore = result.Metadata.GetValueOrDefault("security_score", 0.0);
        _logger.LogInformation($"   📊 Security Score: {securityScore:F2}");
    }

    /// <summary>
    /// Shows Byzantine attack simulation results
    /// </summary>
    private void ShowByzantineAttackResults(SwarmResult result)
    {
        _logger.LogInformation($"   🛡️ System Survived Attack: {result.Success}");
        _logger.LogInformation($"   ⚖️ Consensus Reached Despite Attack: {result.ConsensusReached}");
        _logger.LogInformation($"   🎯 Final Trust Score: {result.TrustScore:F2}");
        _logger.LogInformation($"   🚨 Byzantine Faults Detected: {result.ByzantineFaultsDetected}");
        _logger.LogInformation($"   👥 Participating Agents: {result.ParticipatingAgents}");
        
        // Show attack-specific metrics
        var attacksBlocked = result.Metadata.GetValueOrDefault("attacks_blocked", 0);
        _logger.LogInformation($"   🛡️ Attacks Blocked: {attacksBlocked}");
        
        var maliciousAgentsDetected = result.Metadata.GetValueOrDefault("malicious_agents_detected", 0);
        _logger.LogInformation($"   🔍 Malicious Agents Detected: {maliciousAgentsDetected}");
        
        var recoveryTimeMs = result.Metadata.GetValueOrDefault("recovery_time_ms", 0);
        _logger.LogInformation($"   🔄 Recovery Time: {recoveryTimeMs}ms");
        
        var consensusImpact = result.Metadata.GetValueOrDefault("consensus_impact", 0.0);
        _logger.LogInformation($"   📊 Consensus Impact: {consensusImpact:P}");
        
        // Show resilience metrics
        var resilienceScore = result.Metadata.GetValueOrDefault("resilience_score", 0.0);
        _logger.LogInformation($"   💪 Resilience Score: {resilienceScore:F2}");
        
        _logger.LogInformation($"   ⏱️ Total Simulation Time: {result.TotalExecutionTime.TotalSeconds:F1}s");
        
        if (result.Success)
        {
            _logger.LogInformation("✅ System successfully defended against Byzantine attack");
        }
        else
        {
            _logger.LogWarning("⚠️ System experienced issues during attack simulation");
        }
    }

    /// <summary>
    /// Truncates string for display purposes
    /// </summary>
    private string TruncateString(string input, int maxLength)
    {
        if (string.IsNullOrEmpty(input) || input.Length <= maxLength)
            return input;
            
        return input.Substring(0, maxLength) + "...";
    }
}

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