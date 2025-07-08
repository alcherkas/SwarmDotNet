using ByzantineSwarmDotnet.Examples.Models;
using ByzantineSwarmDotnet.Examples.Interfaces;
using Microsoft.Extensions.Logging;

namespace ByzantineSwarmDotnet.Examples;

/// <summary>
/// Example demonstrating how to use the Byzantine-tolerant AI Agent Swarm for code review analysis
/// This example simulates a pull request review workflow using the Byzantine swarm with fault tolerance
/// </summary>
public class ByzantineSwarmUsageExample
{
    private readonly IByzantineTolerantSwarmOrchestrator _orchestrator;
    private readonly ILogger<ByzantineSwarmUsageExample> _logger;

    public ByzantineSwarmUsageExample(IByzantineTolerantSwarmOrchestrator orchestrator, ILogger<ByzantineSwarmUsageExample> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    /// <summary>
    /// Main example method demonstrating a complete Byzantine-tolerant PR analysis workflow
    /// </summary>
    public async Task RunPullRequestAnalysisExampleAsync()
    {
        _logger.LogInformation("🚀 Starting Byzantine-Tolerant Pull Request Analysis Example");

        try
        {
            // Example 1: Basic PR Analysis with Byzantine Tolerance
            await RunBasicPRAnalysisAsync();

            // Example 2: Complex Feature Analysis with Consensus
            await RunComplexFeatureAnalysisAsync();

            // Example 3: Security-focused Analysis with Enhanced Validation
            await RunSecurityAnalysisAsync();

            // Example 4: Byzantine Attack Simulation
            await RunByzantineAttackSimulationAsync();

            _logger.LogInformation("✅ All Byzantine swarm examples completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error running Byzantine swarm examples");
            throw;
        }
    }

    /// <summary>
    /// Example 1: Basic PR analysis with Byzantine fault tolerance
    /// </summary>
    private async Task RunBasicPRAnalysisAsync()
    {
        _logger.LogInformation("\n📝 Example 1: Basic PR Analysis with Byzantine Tolerance");

        var swarmTask = new SwarmTask
        {
            Description = "Byzantine-tolerant analysis: Fix null pointer exception in user authentication",
            RequiredRoles = new[]
            {
                "PRExtractor",
                "CodeAnalyzer", 
                "RiskAssessment",
                "TestCoverage",
                "SummaryGenerator"
            },
            RequiredConsensus = true,
            FaultToleranceLevel = FaultToleranceLevel.Byzantine,
            SecurityLevel = SecurityLevel.High,
            MaxExecutionTime = TimeSpan.FromMinutes(5),
            Parameters = new Dictionary<string, object>
            {
                ["pr_url"] = "https://github.com/company/project/pull/123",
                ["pr_title"] = "Fix null pointer exception in user authentication",
                ["pr_description"] = "Fixes NPE that occurs when user tries to login with empty credentials",
                ["jira_tickets"] = new[] { "BUG-456" },
                ["files_changed"] = new[]
                {
                    "src/auth/UserAuthService.cs",
                    "tests/auth/UserAuthServiceTests.cs"
                },
                ["lines_changed"] = "+15 -8",
                ["byzantine_tolerance"] = true,
                ["required_consensus"] = true
            }
        };

        var result = await _orchestrator.ExecuteTaskAsync(swarmTask);
        
        _logger.LogInformation("📊 Byzantine PR Analysis Results:");
        _logger.LogInformation($"   Success: {result.Success}");
        _logger.LogInformation($"   Consensus Reached: {result.ConsensusReached}");
        _logger.LogInformation($"   Trust Score: {result.TrustScore:F2}");
        _logger.LogInformation($"   Participating Agents: {result.ParticipatingAgents}");
        _logger.LogInformation($"   Byzantine Faults Detected: {result.ByzantineFaultsDetected}");
        _logger.LogInformation($"   Execution Time: {result.TotalExecutionTime.TotalSeconds:F1}s");
        _logger.LogInformation($"   Final Result: {result.Result}");
    }

    /// <summary>
    /// Example 2: Complex feature analysis with consensus validation
    /// </summary>
    private async Task RunComplexFeatureAnalysisAsync()
    {
        _logger.LogInformation("\n🔧 Example 2: Complex Feature Analysis with Consensus");

        var swarmTask = new SwarmTask
        {
            Description = "Byzantine-tolerant analysis: Integration with new payment provider",
            RequiredRoles = new[]
            {
                "Orchestrator",
                "PRExtractor",
                "JiraContext",
                "CodeAnalyzer",
                "RequirementMapper",
                "TestCoverage",
                "RiskAssessment",
                "IntegrationAnalyzer",
                "SummaryGenerator"
            },
            RequiredConsensus = true,
            FaultToleranceLevel = FaultToleranceLevel.Byzantine,
            SecurityLevel = SecurityLevel.Critical,
            MaxExecutionTime = TimeSpan.FromMinutes(10),
            Parameters = new Dictionary<string, object>
            {
                ["pr_url"] = "https://github.com/company/project/pull/789",
                ["pr_title"] = "Add Stripe payment integration for subscription billing",
                ["pr_description"] = "Implements Stripe payment gateway integration for monthly subscriptions. Includes webhook handling, payment processing, and subscription management.",
                ["jira_tickets"] = new[] { "FEAT-123", "FEAT-124", "TECH-456" },
                ["epic"] = "PAY-001",
                ["files_changed"] = new[]
                {
                    "src/payments/StripePaymentService.cs",
                    "src/payments/PaymentController.cs",
                    "src/subscriptions/SubscriptionManager.cs",
                    "src/webhooks/StripeWebhookHandler.cs",
                    "tests/payments/StripePaymentServiceTests.cs",
                    "tests/integration/PaymentIntegrationTests.cs",
                    "config/appsettings.json"
                },
                ["lines_changed"] = "+450 -25",
                ["api_changes"] = true,
                ["database_changes"] = new[] { "add_stripe_customer_id", "add_subscription_status" },
                ["external_dependencies"] = new[] { "Stripe.net v40.0.0" },
                ["consensus_required"] = true,
                ["min_consensus_threshold"] = 0.7
            }
        };

        var result = await _orchestrator.ExecuteTaskAsync(swarmTask);
        
        _logger.LogInformation("📊 Complex Feature Analysis Results:");
        _logger.LogInformation($"   Success: {result.Success}");
        _logger.LogInformation($"   Consensus Reached: {result.ConsensusReached}");
        _logger.LogInformation($"   Consensus Confidence: {result.ConsensusConfidence:P}");
        _logger.LogInformation($"   Trust Score: {result.TrustScore:F2}");
        _logger.LogInformation($"   Average Reputation: {result.AverageReputationScore:F2}");
        _logger.LogInformation($"   Participating Agents: {result.ParticipatingAgents}");
        _logger.LogInformation($"   Byzantine Faults Detected: {result.ByzantineFaultsDetected}");
        _logger.LogInformation($"   Execution Time: {result.TotalExecutionTime.TotalSeconds:F1}s");
        
        if (result.Warnings.Any())
        {
            _logger.LogWarning("⚠️  Warnings:");
            foreach (var warning in result.Warnings)
            {
                _logger.LogWarning($"   - {warning}");
            }
        }
        
        _logger.LogInformation($"   Final Summary: {result.Result}");
    }

    /// <summary>
    /// Example 3: Security-focused analysis with enhanced validation
    /// </summary>
    private async Task RunSecurityAnalysisAsync()
    {
        _logger.LogInformation("\n🔒 Example 3: Security Analysis with Enhanced Validation");

        var swarmTask = new SwarmTask
        {
            Description = "Byzantine-tolerant security review: JWT token validation and user permissions",
            RequiredRoles = new[]
            {
                "PRExtractor",
                "CodeAnalyzer",
                "RiskAssessment",
                "IntegrationAnalyzer",
                "Validator",
                "SummaryGenerator"
            },
            RequiredConsensus = true,
            FaultToleranceLevel = FaultToleranceLevel.Byzantine,
            SecurityLevel = SecurityLevel.Critical,
            MaxExecutionTime = TimeSpan.FromMinutes(7),
            Parameters = new Dictionary<string, object>
            {
                ["pr_url"] = "https://github.com/company/project/pull/456",
                ["pr_title"] = "Update JWT validation and add role-based access control",
                ["pr_description"] = "Updates JWT token validation logic and implements role-based access control for admin endpoints",
                ["jira_tickets"] = new[] { "SEC-789", "SEC-790" },
                ["files_changed"] = new[]
                {
                    "src/auth/JwtValidator.cs",
                    "src/auth/RoleManager.cs",
                    "src/middleware/AuthorizationMiddleware.cs",
                    "src/controllers/AdminController.cs"
                },
                ["lines_changed"] = "+120 -45",
                ["security_sensitive"] = true,
                ["breaking_changes"] = false,
                ["affects_endpoints"] = new[] { "/api/admin/*", "/api/users/permissions" },
                ["enhanced_validation"] = true,
                ["cryptographic_signing"] = true
            }
        };

        var result = await _orchestrator.ExecuteTaskAsync(swarmTask);
        
        _logger.LogInformation("📊 Security Analysis Results:");
        _logger.LogInformation($"   Success: {result.Success}");
        _logger.LogInformation($"   Consensus Reached: {result.ConsensusReached}");
        _logger.LogInformation($"   Trust Score: {result.TrustScore:F2}");
        _logger.LogInformation($"   Security Validation Passed: {result.Metadata.GetValueOrDefault("security_validation", false)}");
        _logger.LogInformation($"   Cryptographic Signatures Valid: {result.Metadata.GetValueOrDefault("signatures_valid", false)}");
        _logger.LogInformation($"   Byzantine Faults Detected: {result.ByzantineFaultsDetected}");
        _logger.LogInformation($"   Execution Time: {result.TotalExecutionTime.TotalSeconds:F1}s");
        _logger.LogInformation($"   📋 Security Summary: {result.Result}");
    }

    /// <summary>
    /// Example 4: Byzantine attack simulation and recovery
    /// </summary>
    private async Task RunByzantineAttackSimulationAsync()
    {
        _logger.LogInformation("\n🛡️ Example 4: Byzantine Attack Simulation");

        var swarmTask = new SwarmTask
        {
            Description = "Simulate Byzantine attack during PR analysis to test fault tolerance",
            RequiredRoles = new[]
            {
                "Orchestrator",
                "PRExtractor",
                "CodeAnalyzer",
                "Validator",
                "RiskAssessment",
                "SummaryGenerator"
            },
            RequiredConsensus = true,
            FaultToleranceLevel = FaultToleranceLevel.Byzantine,
            SecurityLevel = SecurityLevel.High,
            MaxExecutionTime = TimeSpan.FromMinutes(8),
            Parameters = new Dictionary<string, object>
            {
                ["pr_title"] = "Test PR for Byzantine attack simulation",
                ["pr_description"] = "This PR is used to test the system's resilience to Byzantine attacks",
                ["simulate_byzantine_attack"] = true,
                ["malicious_agent_percentage"] = 0.25, // 25% of agents will act maliciously
                ["attack_type"] = "contradictory_responses",
                ["recovery_enabled"] = true,
                ["consensus_threshold"] = 0.67 // Require 67% consensus
            }
        };

        try
        {
            var result = await _orchestrator.ExecuteTaskAsync(swarmTask);
            
            _logger.LogInformation("📊 Byzantine Attack Simulation Results:");
            _logger.LogInformation($"   System Survived Attack: {result.Success}");
            _logger.LogInformation($"   Consensus Reached Despite Attack: {result.ConsensusReached}");
            _logger.LogInformation($"   Trust Score: {result.TrustScore:F2}");
            _logger.LogInformation($"   Byzantine Faults Detected: {result.ByzantineFaultsDetected}");
            _logger.LogInformation($"   Participating Agents: {result.ParticipatingAgents}");
            _logger.LogInformation($"   Recovery Actions Taken: {result.Metadata.GetValueOrDefault("recovery_actions", 0)}");
            _logger.LogInformation($"   Execution Time: {result.TotalExecutionTime.TotalSeconds:F1}s");
            
            if (result.Errors.Any())
            {
                _logger.LogError("🚨 Attack-related errors detected:");
                foreach (var error in result.Errors)
                {
                    _logger.LogError($"   - {error}");
                }
            }
        }
        catch (ByzantineAttackDetectedException ex)
        {
            _logger.LogWarning("🔴 Byzantine attack was detected and blocked:");
            _logger.LogWarning($"   Malicious Agents: {string.Join(", ", ex.MaliciousAgents)}");
            _logger.LogWarning($"   Attack Details: {ex.Message}");
            _logger.LogInformation("✅ System successfully defended against Byzantine attack");
        }
    }

    /// <summary>
    /// Demonstrates custom workflow with Byzantine fault tolerance
    /// </summary>
    public async Task RunCustomWorkflowExampleAsync()
    {
        _logger.LogInformation("\n⚙️ Custom Byzantine-Tolerant Workflow: API Documentation Review");

        var customTask = new SwarmTask
        {
            Description = "Byzantine-tolerant review of API documentation changes with consensus validation",
            RequiredRoles = new[]
            {
                "Orchestrator",
                "PRExtractor",
                "CodeAnalyzer",
                "IntegrationAnalyzer",
                "Validator",
                "SummaryGenerator"
            },
            RequiredConsensus = true,
            FaultToleranceLevel = FaultToleranceLevel.Byzantine,
            SecurityLevel = SecurityLevel.Medium,
            MaxExecutionTime = TimeSpan.FromMinutes(6),
            Parameters = new Dictionary<string, object>
            {
                ["focus"] = "documentation_and_integration",
                ["pr_title"] = "Update API documentation for v2.0 endpoints",
                ["files_changed"] = new[]
                {
                    "docs/api/v2/users.md",
                    "docs/api/v2/payments.md",
                    "src/controllers/UsersV2Controller.cs",
                    "openapi.yaml"
                },
                ["documentation_changes"] = true,
                ["api_version"] = "v2.0",
                ["consensus_validation"] = true,
                ["cryptographic_verification"] = true,
                ["reputation_tracking"] = true
            }
        };

        var result = await _orchestrator.ExecuteTaskAsync(customTask);
        
        _logger.LogInformation("📊 Custom Byzantine Workflow Results:");
        _logger.LogInformation($"   Analysis completed in {result.TotalExecutionTime.TotalSeconds:F1}s");
        _logger.LogInformation($"   Consensus reached: {result.ConsensusReached}");
        _logger.LogInformation($"   Trust score: {result.TrustScore:F2}");
        _logger.LogInformation($"   Byzantine faults: {result.ByzantineFaultsDetected}");
        _logger.LogInformation($"   Documentation review: {result.Result}");
    }

    /// <summary>
    /// Demonstrates reputation system tracking and impact
    /// </summary>
    public async Task RunReputationTrackingExampleAsync()
    {
        _logger.LogInformation("\n⭐ Reputation System Tracking Example");

        var reputationTask = new SwarmTask
        {
            Description = "Demonstrate reputation tracking and its impact on consensus decisions",
            RequiredRoles = new[]
            {
                "ReputationManager",
                "Validator",
                "CodeAnalyzer",
                "SummaryGenerator"
            },
            RequiredConsensus = true,
            FaultToleranceLevel = FaultToleranceLevel.Byzantine,
            SecurityLevel = SecurityLevel.Medium,
            Parameters = new Dictionary<string, object>
            {
                ["track_reputation"] = true,
                ["reputation_impact"] = true,
                ["trust_threshold"] = 0.6,
                ["reputation_decay"] = 0.95,
                ["pr_title"] = "Reputation tracking demonstration"
            }
        };

        var result = await _orchestrator.ExecuteTaskAsync(reputationTask);
        
        _logger.LogInformation("📊 Reputation Tracking Results:");
        _logger.LogInformation($"   Average reputation score: {result.AverageReputationScore:F2}");
        _logger.LogInformation($"   Trust score: {result.TrustScore:F2}");
        _logger.LogInformation($"   Reputation updates: {result.Metadata.GetValueOrDefault("reputation_updates", 0)}");
        _logger.LogInformation($"   High-reputation agents: {result.Metadata.GetValueOrDefault("high_reputation_agents", 0)}");
    }
}