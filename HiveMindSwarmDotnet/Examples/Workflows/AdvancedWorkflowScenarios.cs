using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using HiveMindSwarmDotnet.Examples.SampleData;
using Microsoft.Extensions.Logging;

namespace HiveMindSwarmDotnet.Examples.Workflows;

/// <summary>
/// Demonstrates advanced workflow scenarios
/// </summary>
public class AdvancedWorkflowScenarios
{
    private readonly ISwarmOrchestrator _orchestrator;
    private readonly ILogger<AdvancedWorkflowScenarios> _logger;

    public AdvancedWorkflowScenarios(ISwarmOrchestrator orchestrator, ILogger<AdvancedWorkflowScenarios> logger)
    {
        _orchestrator = orchestrator;
        _logger = logger;
    }

    /// <summary>
    /// Demonstrates parallel workflow execution
    /// </summary>
    public async Task RunParallelWorkflowAsync()
    {
        _logger.LogInformation("🚀 Running Parallel Workflow Example");
        
        var tasks = new[]
        {
            SamplePRData.CreateBugFixPR(),
            SamplePRData.CreateNewFeaturePR(),
            SamplePRData.CreateSecurityPR()
        };

        _logger.LogInformation($"📋 Processing {tasks.Length} PRs in parallel...");

        var startTime = DateTime.UtcNow;
        
        // Execute all tasks in parallel
        var results = await Task.WhenAll(
            tasks.Select(task => _orchestrator.ExecuteTaskAsync(task))
        );

        var totalTime = DateTime.UtcNow - startTime;
        
        _logger.LogInformation($"✅ Parallel execution completed in {totalTime.TotalSeconds:F1}s");
        _logger.LogInformation($"📊 Results: {results.Count(r => r.IsSuccessful)}/{results.Length} successful");
        
        for (int i = 0; i < results.Length; i++)
        {
            _logger.LogInformation($"   Task {i + 1}: {results[i].OverallConfidence:P} confidence");
        }
    }

    /// <summary>
    /// Demonstrates adaptive workflow based on initial analysis
    /// </summary>
    public async Task RunAdaptiveWorkflowAsync()
    {
        _logger.LogInformation("🧠 Running Adaptive Workflow Example");
        
        // Start with basic analysis
        var initialTask = new SwarmTask
        {
            Description = "Initial PR analysis to determine workflow path",
            RequiredRoles = new[] { AgentRole.PRExtractor, AgentRole.CodeAnalyzer },
            Parameters = new Dictionary<string, object>
            {
                ["pr_title"] = "Update authentication system",
                ["files_changed"] = new[] { "src/auth/JwtValidator.cs", "src/auth/UserManager.cs" }
            }
        };

        var initialResult = await _orchestrator.ExecuteTaskAsync(initialTask);
        
        _logger.LogInformation($"📊 Initial analysis confidence: {initialResult.OverallConfidence:P}");

        // Adapt workflow based on results
        var nextRoles = new List<AgentRole>();
        
        if (initialResult.OverallConfidence < 0.7)
        {
            _logger.LogInformation("🔍 Low confidence detected - adding more analysis agents");
            nextRoles.AddRange(new[] { AgentRole.JiraContext, AgentRole.RequirementMapper });
        }

        // Check if security-sensitive
        var codeAnalysis = initialResult.AgentResponses
            .FirstOrDefault(r => r.Role == AgentRole.CodeAnalyzer);
            
        if (codeAnalysis?.Content.Contains("auth", StringComparison.OrdinalIgnoreCase) == true)
        {
            _logger.LogInformation("🔒 Security-sensitive code detected - adding security analysis");
            nextRoles.AddRange(new[] { AgentRole.RiskAssessment, AgentRole.IntegrationAnalyzer });
        }

        // Always add summary generator
        nextRoles.Add(AgentRole.SummaryGenerator);

        if (nextRoles.Any())
        {
            var adaptiveTask = new SwarmTask
            {
                Description = "Adaptive follow-up analysis based on initial results",
                RequiredRoles = nextRoles.ToArray(),
                Parameters = initialTask.Parameters
            };

            var finalResult = await _orchestrator.ExecuteTaskAsync(adaptiveTask);
            
            _logger.LogInformation($"✅ Adaptive workflow completed with {finalResult.OverallConfidence:P} confidence");
            _logger.LogInformation($"🎯 Total agents used: {initialResult.AgentResponses.Count + finalResult.AgentResponses.Count}");
        }
    }

    /// <summary>
    /// Demonstrates continuous learning workflow
    /// </summary>
    public async Task RunLearningWorkflowAsync()
    {
        _logger.LogInformation("📚 Running Learning Workflow Example");
        
        var learningTask = new SwarmTask
        {
            Description = "Analyze previous PR review outcomes to improve future analysis",
            RequiredRoles = new[] { AgentRole.Learning, AgentRole.SummaryGenerator },
            Parameters = new Dictionary<string, object>
            {
                ["historical_data"] = new
                {
                    previous_reviews = 150,
                    accuracy_rate = 0.87,
                    common_issues = new[] { "insufficient_test_coverage", "security_concerns", "performance_impact" },
                    improvement_areas = new[] { "requirement_mapping", "integration_analysis" }
                },
                ["recent_feedback"] = new
                {
                    false_positives = 5,
                    missed_issues = 3,
                    reviewer_satisfaction = 4.2
                }
            }
        };

        var result = await _orchestrator.ExecuteTaskAsync(learningTask);
        
        _logger.LogInformation("📈 Learning Analysis Results:");
        _logger.LogInformation($"   🎯 Current accuracy rate: 87%");
        _logger.LogInformation($"   📊 Improvement potential: {result.OverallConfidence:P}");
        _logger.LogInformation($"   🔧 Recommended optimizations: {result.FinalAnswer}");
    }
}