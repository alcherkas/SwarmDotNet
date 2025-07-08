using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using HiveMindSwarmDotnet.Examples.SampleData;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace HiveMindSwarmDotnet.Examples.Workflows;

/// <summary>
/// Demonstrates a complete code review workflow using the agent swarm
/// Shows step-by-step agent coordination and communication
/// </summary>
public class CodeReviewWorkflow
{
    private readonly ISwarmOrchestrator _orchestrator;
    private readonly ILogger<CodeReviewWorkflow> _logger;

    public CodeReviewWorkflow(ISwarmOrchestrator orchestrator, ILogger<CodeReviewWorkflow> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    /// <summary>
    /// Demonstrates the complete PR analysis workflow with detailed agent coordination
    /// </summary>
    public async Task RunCompleteWorkflowAsync()
    {
        _logger.LogInformation("🚀 Starting Complete Code Review Workflow");
        _logger.LogInformation("================================================");

        try
        {
            // Step 1: Initialize the workflow
            await InitializeWorkflowAsync();

            // Step 2: Run the bug fix analysis workflow
            await RunBugFixWorkflowAsync();

            // Step 3: Run the feature development workflow
            await RunFeatureWorkflowAsync();

            // Step 4: Run the security review workflow
            await RunSecurityWorkflowAsync();

            _logger.LogInformation("✅ Complete workflow finished successfully!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Workflow execution failed");
            throw;
        }
    }

    private async Task InitializeWorkflowAsync()
    {
        _logger.LogInformation("\n🔧 Step 1: Initializing Workflow");
        _logger.LogInformation("- Setting up agent swarm");
        _logger.LogInformation("- Preparing sample data");
        _logger.LogInformation("- Configuring communication channels");
        
        await Task.Delay(1000); // Simulate initialization
        _logger.LogInformation("✅ Workflow initialized");
    }

    /// <summary>
    /// Demonstrates bug fix review workflow
    /// </summary>
    private async Task RunBugFixWorkflowAsync()
    {
        _logger.LogInformation("\n🐛 Step 2: Bug Fix Review Workflow");
        _logger.LogInformation("================================================");

        var bugFixTask = SamplePRData.CreateBugFixPR();
        
        _logger.LogInformation($"📋 Analyzing PR: {bugFixTask.Description}");
        _logger.LogInformation($"🎯 Required Agents: {string.Join(", ", bugFixTask.RequiredRoles)}");

        // Execute the task and show detailed progress
        var result = await ExecuteTaskWithDetailedLoggingAsync(bugFixTask, "Bug Fix Analysis");

        // Show workflow-specific insights
        _logger.LogInformation("\n📊 Bug Fix Workflow Results:");
        ShowWorkflowResults(result);
    }

    /// <summary>
    /// Demonstrates new feature review workflow
    /// </summary>
    private async Task RunFeatureWorkflowAsync()
    {
        _logger.LogInformation("\n⭐ Step 3: New Feature Review Workflow");
        _logger.LogInformation("================================================");

        var featureTask = SamplePRData.CreateNewFeaturePR();
        
        _logger.LogInformation($"📋 Analyzing PR: {featureTask.Description}");
        _logger.LogInformation($"🎯 Required Agents: {string.Join(", ", featureTask.RequiredRoles)}");

        // Execute the task with coordination details
        var result = await ExecuteTaskWithDetailedLoggingAsync(featureTask, "Feature Development Analysis");

        // Show feature-specific insights
        _logger.LogInformation("\n📊 Feature Workflow Results:");
        ShowWorkflowResults(result);
        
        // Show agent coordination patterns
        ShowAgentCoordinationInsights(result);
    }

    /// <summary>
    /// Demonstrates security review workflow
    /// </summary>
    private async Task RunSecurityWorkflowAsync()
    {
        _logger.LogInformation("\n🔒 Step 4: Security Review Workflow");
        _logger.LogInformation("================================================");

        var securityTask = SamplePRData.CreateSecurityPR();
        
        _logger.LogInformation($"📋 Analyzing PR: {securityTask.Description}");
        _logger.LogInformation($"🎯 Required Agents: {string.Join(", ", securityTask.RequiredRoles)}");

        // Execute security-focused analysis
        var result = await ExecuteTaskWithDetailedLoggingAsync(securityTask, "Security Review Analysis");

        // Show security-specific insights
        _logger.LogInformation("\n📊 Security Workflow Results:");
        ShowWorkflowResults(result);
        ShowSecurityInsights(result);
    }

    /// <summary>
    /// Executes a task with detailed logging of agent coordination
    /// </summary>
    private async Task<SwarmResult> ExecuteTaskWithDetailedLoggingAsync(SwarmTask task, string workflowName)
    {
        _logger.LogInformation($"\n🔄 Executing {workflowName}...");
        
        var startTime = DateTime.UtcNow;
        
        // Simulate step-by-step agent execution
        foreach (var role in task.RequiredRoles)
        {
            _logger.LogInformation($"   🤖 Activating {role} agent...");
            await Task.Delay(500); // Simulate processing time
        }

        // Execute the actual task
        var result = await _orchestrator.ExecuteTaskAsync(task);
        
        var duration = DateTime.UtcNow - startTime;
        
        _logger.LogInformation($"✅ {workflowName} completed in {duration.TotalSeconds:F1}s");
        _logger.LogInformation($"   📈 Overall Confidence: {result.OverallConfidence:P}");
        _logger.LogInformation($"   🎯 Success: {result.IsSuccessful}");
        
        return result;
    }

    /// <summary>
    /// Shows detailed workflow results
    /// </summary>
    private void ShowWorkflowResults(SwarmResult result)
    {
        _logger.LogInformation($"   ⏱️  Execution Time: {result.ExecutionTime.TotalSeconds:F1}s");
        _logger.LogInformation($"   🤖 Agents Involved: {result.AgentResponses.Count}");
        _logger.LogInformation($"   💬 Communications: {result.Communications.Count}");
        
        // Show individual agent results
        foreach (var response in result.AgentResponses)
        {
            var emoji = GetAgentEmoji(response.Role);
            _logger.LogInformation($"   {emoji} {response.Role}: {response.Confidence:P} confidence");
            
            if (response.ValidationErrors.Any())
            {
                _logger.LogWarning($"      ⚠️  Validation issues: {response.ValidationErrors.Count}");
            }
        }
        
        _logger.LogInformation($"   📝 Summary: {TruncateString(result.FinalAnswer, 150)}");
    }

    /// <summary>
    /// Shows agent coordination insights
    /// </summary>
    private void ShowAgentCoordinationInsights(SwarmResult result)
    {
        _logger.LogInformation("\n🔗 Agent Coordination Insights:");
        
        // Analyze communication patterns
        var communicationsByAgent = result.Communications
            .GroupBy(c => c.FromAgentId)
            .ToDictionary(g => g.Key, g => g.Count());

        foreach (var kvp in communicationsByAgent)
        {
            _logger.LogInformation($"   📤 Agent {kvp.Key}: sent {kvp.Value} messages");
        }

        // Show collaboration effectiveness
        var avgConfidence = result.AgentResponses.Average(r => r.Confidence);
        _logger.LogInformation($"   🎯 Average Agent Confidence: {avgConfidence:P}");
        
        // Identify high-performing agent combinations
        var highConfidenceAgents = result.AgentResponses
            .Where(r => r.Confidence > 0.8)
            .Select(r => r.Role)
            .ToList();
            
        if (highConfidenceAgents.Any())
        {
            _logger.LogInformation($"   ⭐ High-confidence agents: {string.Join(", ", highConfidenceAgents)}");
        }
    }

    /// <summary>
    /// Shows security-specific insights
    /// </summary>
    private void ShowSecurityInsights(SwarmResult result)
    {
        _logger.LogInformation("\n🔐 Security Analysis Insights:");
        
        var riskAssessment = result.AgentResponses
            .FirstOrDefault(r => r.Role == AgentRole.RiskAssessment);
            
        if (riskAssessment != null)
        {
            _logger.LogInformation($"   🛡️  Risk Assessment Confidence: {riskAssessment.Confidence:P}");
            _logger.LogInformation($"   📋 Risk Findings: {TruncateString(riskAssessment.Content, 200)}");
        }

        var integrationAnalysis = result.AgentResponses
            .FirstOrDefault(r => r.Role == AgentRole.IntegrationAnalyzer);
            
        if (integrationAnalysis != null)
        {
            _logger.LogInformation($"   🔌 Integration Impact: {integrationAnalysis.Confidence:P} confidence");
        }

        // Check for security-related metadata
        if (result.Metrics.ContainsKey("security_score"))
        {
            _logger.LogInformation($"   📊 Security Score: {result.Metrics["security_score"]}");
        }
    }

    /// <summary>
    /// Gets emoji representation for agent roles
    /// </summary>
    private string GetAgentEmoji(AgentRole role) => role switch
    {
        AgentRole.Orchestrator => "🎭",
        AgentRole.PRExtractor => "📥",
        AgentRole.JiraContext => "📋",
        AgentRole.CodeAnalyzer => "🔍",
        AgentRole.RequirementMapper => "🗺️",
        AgentRole.TestCoverage => "🧪",
        AgentRole.RiskAssessment => "⚠️",
        AgentRole.IntegrationAnalyzer => "🔌",
        AgentRole.SummaryGenerator => "📄",
        AgentRole.Learning => "🧠",
        _ => "🤖"
    };

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