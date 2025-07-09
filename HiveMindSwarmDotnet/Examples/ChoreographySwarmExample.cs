using HiveMindSwarmDotnet.Console.Choreography;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;

namespace HiveMindSwarmDotnet.Examples;

/// <summary>
/// Example demonstrating how to use the AI Agent Swarm with choreography pattern for code review analysis
/// This example shows decentralized coordination where agents autonomously collaborate without central orchestration
/// </summary>
public class ChoreographySwarmExample
{
    private readonly IChoreographyCoordinator _coordinator;
    private readonly IEventBus _eventBus;
    private readonly ILogger<ChoreographySwarmExample> _logger;

    public ChoreographySwarmExample(
        IChoreographyCoordinator coordinator, 
        IEventBus eventBus,
        ILogger<ChoreographySwarmExample> logger)
    {
        _coordinator = coordinator;
        _eventBus = eventBus;
        _logger = logger;
    }

    /// <summary>
    /// Main example method demonstrating choreographed PR analysis workflow
    /// </summary>
    public async Task RunChoreographedPullRequestAnalysisAsync()
    {
        _logger.LogInformation("🕺 Starting Choreographed Pull Request Analysis Example");

        try
        {
            // Example 1: Basic PR Analysis with Choreography
            await RunChoreographedBasicPRAnalysisAsync();

            // Example 2: Complex Feature Analysis with Agent Collaboration
            await RunChoreographedComplexFeatureAnalysisAsync();

            // Example 3: Self-Organizing Risk Assessment
            await RunSelfOrganizingRiskAssessmentAsync();

            // Example 4: Peer-to-Peer Communication Demo
            await RunPeerToPeerCommunicationDemoAsync();

            _logger.LogInformation("✅ All choreography examples completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error running choreography examples");
            throw;
        }
    }

    /// <summary>
    /// Example 1: Basic PR analysis using choreography pattern
    /// Agents autonomously decide to participate based on task events
    /// </summary>
    private async Task RunChoreographedBasicPRAnalysisAsync()
    {
        _logger.LogInformation("\n🎭 Example 1: Choreographed Basic PR Analysis - Bug Fix");

        var swarmTask = new SwarmTask
        {
            Description = "[Choreography] Analyze pull request: Fix null pointer exception in user authentication",
            RequiredRoles = new[]
            {
                AgentRole.PRExtractor,
                AgentRole.CodeAnalyzer,
                AgentRole.RiskAssessment,
                AgentRole.TestCoverage,
                AgentRole.SummaryGenerator
            },
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
                ["choreography_mode"] = true
            },
            Priority = 2
        };

        var result = await _coordinator.ExecuteTaskAsync(swarmTask);
        
        _logger.LogInformation("📊 Choreographed Basic PR Analysis Results:");
        _logger.LogInformation($"   Success: {result.IsSuccessful}");
        _logger.LogInformation($"   Confidence: {result.OverallConfidence:P}");
        _logger.LogInformation($"   Execution Time: {result.ExecutionTime.TotalSeconds:F1}s");
        _logger.LogInformation($"   Agents Self-Organized: {result.AgentResponses.Count}");
        _logger.LogInformation($"   Final Answer: {result.FinalAnswer}");
    }

    /// <summary>
    /// Example 2: Complex feature analysis with autonomous agent collaboration
    /// </summary>
    private async Task RunChoreographedComplexFeatureAnalysisAsync()
    {
        _logger.LogInformation("\n🤝 Example 2: Choreographed Complex Feature Analysis - Payment Integration");

        var swarmTask = new SwarmTask
        {
            Description = "[Choreography] Analyze major feature PR: Integration with new payment provider",
            RequiredRoles = new[]
            {
                AgentRole.PRExtractor,
                AgentRole.JiraContext,
                AgentRole.CodeAnalyzer,
                AgentRole.RequirementMapper,
                AgentRole.TestCoverage,
                AgentRole.RiskAssessment,
                AgentRole.IntegrationAnalyzer,
                AgentRole.SummaryGenerator
            },
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
                ["choreography_mode"] = true,
                ["enable_auto_collaboration"] = true
            },
            Priority = 1
        };

        var result = await _coordinator.ExecuteTaskAsync(swarmTask);
        
        _logger.LogInformation("📊 Choreographed Complex Feature Analysis Results:");
        _logger.LogInformation($"   Success: {result.IsSuccessful}");
        _logger.LogInformation($"   Confidence: {result.OverallConfidence:P}");
        _logger.LogInformation($"   Execution Time: {result.ExecutionTime.TotalSeconds:F1}s");
        _logger.LogInformation($"   Autonomous Agents: {result.AgentResponses.Count}");
        
        foreach (var agentResponse in result.AgentResponses)
        {
            _logger.LogInformation($"   - {agentResponse.Role}: {agentResponse.Confidence:P} confidence (autonomous)");
        }
        
        _logger.LogInformation($"   Choreographed Summary: {result.FinalAnswer}");
    }

    /// <summary>
    /// Example 3: Self-organizing risk assessment with peer collaboration
    /// </summary>
    private async Task RunSelfOrganizingRiskAssessmentAsync()
    {
        _logger.LogInformation("\n🔒 Example 3: Self-Organizing Security Risk Assessment");

        var swarmTask = new SwarmTask
        {
            Description = "[Choreography] Security review: Changes to JWT token validation and user permissions",
            RequiredRoles = new[]
            {
                AgentRole.PRExtractor,
                AgentRole.CodeAnalyzer,
                AgentRole.RiskAssessment,
                AgentRole.IntegrationAnalyzer,
                AgentRole.SummaryGenerator
            },
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
                ["choreography_mode"] = true,
                ["enable_self_organization"] = true
            },
            Priority = 1
        };

        var result = await _coordinator.ExecuteTaskAsync(swarmTask);
        
        _logger.LogInformation("📊 Self-Organizing Security Risk Assessment Results:");
        _logger.LogInformation($"   Success: {result.IsSuccessful}");
        _logger.LogInformation($"   Confidence: {result.OverallConfidence:P}");
        _logger.LogInformation($"   Execution Time: {result.ExecutionTime.TotalSeconds:F1}s");
        
        // Extract risk assessment specific insights
        var riskAssessment = result.AgentResponses
            .FirstOrDefault(r => r.Role == AgentRole.RiskAssessment);
        
        if (riskAssessment != null)
        {
            _logger.LogInformation($"   🔍 Autonomous Risk Assessment: {riskAssessment.Confidence:P} confidence");
            _logger.LogInformation($"       {riskAssessment.Content}");
        }
        
        _logger.LogInformation($"   📋 Self-Organized Security Summary: {result.FinalAnswer}");
    }

    /// <summary>
    /// Example 4: Demonstrates peer-to-peer communication between agents
    /// </summary>
    private async Task RunPeerToPeerCommunicationDemoAsync()
    {
        _logger.LogInformation("\n💬 Example 4: Peer-to-Peer Communication Demo");

        // Subscribe to communication events to observe peer-to-peer interactions
        await _eventBus.SubscribeAsync<PeerCommunicationEvent>(async (peerEvent, ct) =>
        {
            _logger.LogInformation($"   📡 P2P Communication: {peerEvent.FromAgentId} → {peerEvent.ToAgentId} ({peerEvent.MessageType})");
        });

        await _eventBus.SubscribeAsync<CollaborationRequestEvent>(async (collabEvent, ct) =>
        {
            _logger.LogInformation($"   🤝 Collaboration Request: {collabEvent.RequestingAgentId} needs {string.Join(", ", collabEvent.RequiredRoles)}");
        });

        await _eventBus.SubscribeAsync<AgentCapabilityEvent>(async (capabilityEvent, ct) =>
        {
            _logger.LogInformation($"   🎯 Agent Capability: {capabilityEvent.Role} reported skills: {string.Join(", ", capabilityEvent.Capabilities)}");
        });

        var swarmTask = new SwarmTask
        {
            Description = "[Choreography] API Documentation Review with Peer Communication",
            RequiredRoles = new[]
            {
                AgentRole.PRExtractor,
                AgentRole.CodeAnalyzer,
                AgentRole.IntegrationAnalyzer,
                AgentRole.SummaryGenerator
            },
            MaxExecutionTime = TimeSpan.FromMinutes(5),
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
                ["choreography_mode"] = true,
                ["enable_peer_communication"] = true
            }
        };

        var result = await _coordinator.ExecuteTaskAsync(swarmTask);
        
        _logger.LogInformation("📊 Peer-to-Peer Communication Demo Results:");
        _logger.LogInformation($"   Analysis completed in {result.ExecutionTime.TotalSeconds:F1}s");
        _logger.LogInformation($"   Peer-coordinated review: {result.FinalAnswer}");
    }

    /// <summary>
    /// Demonstrates event-driven task monitoring and real-time updates
    /// </summary>
    public async Task RunEventDrivenMonitoringExampleAsync()
    {
        _logger.LogInformation("\n📊 Event-Driven Monitoring Example");

        // Subscribe to task events for real-time monitoring
        await _eventBus.SubscribeAsync<TaskAvailableEvent>(async (taskEvent, ct) =>
        {
            _logger.LogInformation($"   📋 Task Available: {taskEvent.RequiredRole} - {taskEvent.TaskDescription}");
        });

        await _eventBus.SubscribeAsync<TaskCompletedEvent>(async (completedEvent, ct) =>
        {
            _logger.LogInformation($"   ✅ Task Completed: {completedEvent.CompletedByRole} - Confidence: {completedEvent.Confidence:P}");
        });

        await _eventBus.SubscribeAsync<TopologyUpdateEvent>(async (topologyEvent, ct) =>
        {
            _logger.LogInformation($"   🌐 Topology Update: {topologyEvent.ActivePeers.Count} active peers, {topologyEvent.ConnectionCount} connections");
        });

        var monitoringTask = new SwarmTask
        {
            Description = "[Choreography] Monitored Analysis: Database schema changes review",
            RequiredRoles = new[]
            {
                AgentRole.CodeAnalyzer,
                AgentRole.RiskAssessment,
                AgentRole.IntegrationAnalyzer,
                AgentRole.SummaryGenerator
            },
            MaxExecutionTime = TimeSpan.FromMinutes(3),
            Parameters = new Dictionary<string, object>
            {
                ["pr_title"] = "Database schema migration for user preferences",
                ["files_changed"] = new[]
                {
                    "database/migrations/001_add_user_preferences.sql",
                    "src/models/UserPreferences.cs",
                    "src/repositories/UserPreferencesRepository.cs"
                },
                ["database_changes"] = true,
                ["choreography_mode"] = true,
                ["enable_monitoring"] = true
            }
        };

        var result = await _coordinator.ExecuteTaskAsync(monitoringTask);
        
        _logger.LogInformation("📊 Event-Driven Monitoring Results:");
        _logger.LogInformation($"   Monitored analysis completed: {result.IsSuccessful}");
        _logger.LogInformation($"   Real-time coordination summary: {result.FinalAnswer}");
    }

    /// <summary>
    /// Shows how to get choreography metrics and performance data
    /// </summary>
    public async Task ShowChoreographyMetricsAsync()
    {
        _logger.LogInformation("\n📈 Choreography Metrics and Performance");

        var metrics = await _coordinator.GetMetricsAsync();
        
        _logger.LogInformation("📊 Choreography Performance Metrics:");
        _logger.LogInformation($"   Total Tasks: {metrics.TotalTasks}");
        _logger.LogInformation($"   Completed Tasks: {metrics.CompletedTasks}");
        _logger.LogInformation($"   Failed Tasks: {metrics.FailedTasks}");
        _logger.LogInformation($"   Average Execution Time: {metrics.AverageExecutionTime:F2}ms");
        _logger.LogInformation($"   Average Confidence: {metrics.AverageConfidence:P}");
        _logger.LogInformation($"   Total Communications: {metrics.TotalCommunications}");
        
        _logger.LogInformation("🎭 Agent Utilization (Choreography):");
        foreach (var utilization in metrics.AgentUtilization)
        {
            _logger.LogInformation($"   - {utilization.Key}: {utilization.Value} autonomous actions");
        }
    }
}