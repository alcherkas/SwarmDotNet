using HiveMindSwarmDotnet.Console.Configuration;
using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using HiveMindSwarmDotnet.Console.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace HiveMindSwarmDotnet.Examples;

/// <summary>
/// Example demonstrating how to use the AI Agent Swarm for code review analysis
/// This example simulates a pull request review workflow using the swarm
/// </summary>
public class SwarmUsageExample
{
    private readonly ISwarmOrchestrator _orchestrator;
    private readonly ILogger<SwarmUsageExample> _logger;

    public SwarmUsageExample(ISwarmOrchestrator orchestrator, ILogger<SwarmUsageExample> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    /// <summary>
    /// Main example method demonstrating a complete PR analysis workflow
    /// </summary>
    public async Task RunPullRequestAnalysisExampleAsync()
    {
        _logger.LogInformation("🚀 Starting Pull Request Analysis Example");

        try
        {
            // Example 1: Basic PR Analysis
            await RunBasicPRAnalysisAsync();

            // Example 2: Complex Feature Analysis
            await RunComplexFeatureAnalysisAsync();

            // Example 3: Risk Assessment Focus
            await RunRiskAssessmentAnalysisAsync();

            _logger.LogInformation("✅ All examples completed successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Error running examples");
            throw;
        }
    }

    /// <summary>
    /// Example 1: Basic PR analysis for a simple bug fix
    /// </summary>
    private async Task RunBasicPRAnalysisAsync()
    {
        _logger.LogInformation("\n📝 Example 1: Basic PR Analysis - Bug Fix");

        var swarmTask = new SwarmTask
        {
            Description = "Analyze pull request: Fix null pointer exception in user authentication",
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
                ["lines_changed"] = "+15 -8"
            },
            Priority = 2
        };

        var result = await _orchestrator.ExecuteTaskAsync(swarmTask);
        
        _logger.LogInformation("📊 Basic PR Analysis Results:");
        _logger.LogInformation($"   Success: {result.IsSuccessful}");
        _logger.LogInformation($"   Confidence: {result.OverallConfidence:P}");
        _logger.LogInformation($"   Execution Time: {result.ExecutionTime.TotalSeconds:F1}s");
        _logger.LogInformation($"   Final Answer: {result.FinalAnswer}");
    }

    /// <summary>
    /// Example 2: Complex feature analysis involving multiple systems
    /// </summary>
    private async Task RunComplexFeatureAnalysisAsync()
    {
        _logger.LogInformation("\n🔧 Example 2: Complex Feature Analysis - New Payment Integration");

        var swarmTask = new SwarmTask
        {
            Description = "Analyze major feature PR: Integration with new payment provider",
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
                ["external_dependencies"] = new[] { "Stripe.net v40.0.0" }
            },
            Priority = 1
        };

        var result = await _orchestrator.ExecuteTaskAsync(swarmTask);
        
        _logger.LogInformation("📊 Complex Feature Analysis Results:");
        _logger.LogInformation($"   Success: {result.IsSuccessful}");
        _logger.LogInformation($"   Confidence: {result.OverallConfidence:P}");
        _logger.LogInformation($"   Execution Time: {result.ExecutionTime.TotalSeconds:F1}s");
        _logger.LogInformation($"   Agents Involved: {result.AgentResponses.Count}");
        
        foreach (var agentResponse in result.AgentResponses)
        {
            _logger.LogInformation($"   - {agentResponse.Role}: {agentResponse.Confidence:P} confidence");
        }
        
        _logger.LogInformation($"   Final Summary: {result.FinalAnswer}");
    }

    /// <summary>
    /// Example 3: Security-focused analysis with risk assessment
    /// </summary>
    private async Task RunRiskAssessmentAnalysisAsync()
    {
        _logger.LogInformation("\n🔒 Example 3: Security Risk Assessment - Authentication Changes");

        var swarmTask = new SwarmTask
        {
            Description = "Security review: Changes to JWT token validation and user permissions",
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
                ["affects_endpoints"] = new[] { "/api/admin/*", "/api/users/permissions" }
            },
            Priority = 1
        };

        var result = await _orchestrator.ExecuteTaskAsync(swarmTask);
        
        _logger.LogInformation("📊 Security Risk Assessment Results:");
        _logger.LogInformation($"   Success: {result.IsSuccessful}");
        _logger.LogInformation($"   Confidence: {result.OverallConfidence:P}");
        _logger.LogInformation($"   Execution Time: {result.ExecutionTime.TotalSeconds:F1}s");
        
        // Extract risk assessment specific insights
        var riskAssessment = result.AgentResponses
            .FirstOrDefault(r => r.Role == AgentRole.RiskAssessment);
        
        if (riskAssessment != null)
        {
            _logger.LogInformation($"   🔍 Risk Assessment: {riskAssessment.Confidence:P} confidence");
            _logger.LogInformation($"       {riskAssessment.Content}");
        }
        
        _logger.LogInformation($"   📋 Security Summary: {result.FinalAnswer}");
    }

    /// <summary>
    /// Demonstrates how to configure and run a custom analysis workflow
    /// </summary>
    public async Task RunCustomWorkflowExampleAsync()
    {
        _logger.LogInformation("\n⚙️ Custom Workflow Example: API Documentation Review");

        // Create a custom task focusing on documentation and integration analysis
        var customTask = new SwarmTask
        {
            Description = "Review API documentation changes and validate integration impacts",
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
                ["api_version"] = "v2.0"
            }
        };

        var result = await _orchestrator.ExecuteTaskAsync(customTask);
        
        _logger.LogInformation("📊 Custom Workflow Results:");
        _logger.LogInformation($"   Analysis completed in {result.ExecutionTime.TotalSeconds:F1}s");
        _logger.LogInformation($"   Documentation review: {result.FinalAnswer}");
    }
}

