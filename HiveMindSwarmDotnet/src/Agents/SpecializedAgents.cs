using System.Text.Json;
using System.Text.RegularExpressions;
using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace HiveMindSwarmDotnet.Console.Agents;

public class OrchestratorAgent : SwarmAgent
{
    public OrchestratorAgent(
        Kernel kernel,
        AgentRoleConfiguration configuration,
        ILogger<SwarmAgent> logger)
        : base(AgentRole.Orchestrator, kernel, configuration, logger)
    {
    }

    protected override async Task<bool> ValidateRoleSpecificResponseAsync(AgentResponse response, CancellationToken cancellationToken)
    {
        var content = response.Content.ToLower();
        
        var hasCoordinationWords = content.Contains("orchestrate") || content.Contains("coordinate") ||
                                   content.Contains("manage") || content.Contains("sequence") ||
                                   content.Contains("workflow") || content.Contains("distribute");
        
        if (!hasCoordinationWords)
        {
            response.ValidationErrors.Add("Orchestrator response should contain coordination and workflow management language");
            return false;
        }

        return await Task.FromResult(true);
    }
}

public class PRExtractorAgent : SwarmAgent
{
    public PRExtractorAgent(
        Kernel kernel,
        AgentRoleConfiguration configuration,
        ILogger<SwarmAgent> logger)
        : base(AgentRole.PRExtractor, kernel, configuration, logger)
    {
    }

    protected override async Task<bool> ValidateRoleSpecificResponseAsync(AgentResponse response, CancellationToken cancellationToken)
    {
        var content = response.Content;
        
        var hasPRElements = content.Contains("PR") || content.Contains("pull request") ||
                           content.Contains("diff") || content.Contains("commit") ||
                           content.Contains("file") || content.Contains("change");
        
        if (!hasPRElements)
        {
            response.ValidationErrors.Add("PR Extractor response should contain pull request related information");
            return false;
        }

        var hasStructuredData = content.Contains("{") || content.Contains("title:") ||
                               content.Contains("files:") || content.Contains("jira");
        
        if (hasStructuredData)
        {
            response.Confidence = Math.Min(1.0, response.Confidence + 0.2);
        }

        return await Task.FromResult(true);
    }
}

public class JiraContextAgent : SwarmAgent
{
    public JiraContextAgent(
        Kernel kernel,
        AgentRoleConfiguration configuration,
        ILogger<SwarmAgent> logger)
        : base(AgentRole.JiraContext, kernel, configuration, logger)
    {
    }

    protected override async Task<bool> ValidateRoleSpecificResponseAsync(AgentResponse response, CancellationToken cancellationToken)
    {
        var content = response.Content.ToLower();
        
        var hasJiraElements = content.Contains("ticket") || content.Contains("epic") ||
                             content.Contains("story") || content.Contains("acceptance") ||
                             content.Contains("requirement") || content.Contains("jira");
        
        if (!hasJiraElements)
        {
            response.ValidationErrors.Add("Jira Context response should contain business requirement information");
            return false;
        }

        var hasBusinessContext = content.Contains("business") || content.Contains("user") ||
                                content.Contains("criteria") || content.Contains("value");
        
        if (hasBusinessContext)
        {
            response.Confidence = Math.Min(1.0, response.Confidence + 0.15);
        }

        return await Task.FromResult(true);
    }
}

public class CodeAnalyzerAgent : SwarmAgent
{
    public CodeAnalyzerAgent(
        Kernel kernel,
        AgentRoleConfiguration configuration,
        ILogger<SwarmAgent> logger)
        : base(AgentRole.CodeAnalyzer, kernel, configuration, logger)
    {
    }

    protected override async Task<bool> ValidateRoleSpecificResponseAsync(AgentResponse response, CancellationToken cancellationToken)
    {
        var content = response.Content.ToLower();
        
        var hasCodeElements = content.Contains("code") || content.Contains("function") ||
                             content.Contains("class") || content.Contains("method") ||
                             content.Contains("api") || content.Contains("pattern");
        
        if (!hasCodeElements)
        {
            response.ValidationErrors.Add("Code Analyzer response should contain technical implementation details");
            return false;
        }

        var hasTechnicalDepth = content.Contains("complexity") || content.Contains("architecture") ||
                               content.Contains("performance") || content.Contains("quality");
        
        if (hasTechnicalDepth)
        {
            response.Confidence = Math.Min(1.0, response.Confidence + 0.2);
        }

        return await Task.FromResult(true);
    }
}

public class RequirementMapperAgent : SwarmAgent
{
    public RequirementMapperAgent(
        Kernel kernel,
        AgentRoleConfiguration configuration,
        ILogger<SwarmAgent> logger)
        : base(AgentRole.RequirementMapper, kernel, configuration, logger)
    {
    }

    protected override async Task<bool> ValidateRoleSpecificResponseAsync(AgentResponse response, CancellationToken cancellationToken)
    {
        var content = response.Content.ToLower();
        
        var hasMappingElements = content.Contains("map") || content.Contains("trace") ||
                                content.Contains("align") || content.Contains("match") ||
                                content.Contains("requirement") || content.Contains("criteria");
        
        if (!hasMappingElements)
        {
            response.ValidationErrors.Add("Requirement Mapper response should contain mapping and traceability information");
            return false;
        }

        var hasGapAnalysis = content.Contains("gap") || content.Contains("missing") ||
                            content.Contains("unimplemented") || content.Contains("coverage");
        
        if (hasGapAnalysis)
        {
            response.Confidence = Math.Min(1.0, response.Confidence + 0.15);
        }

        return await Task.FromResult(true);
    }
}

public class TestCoverageAgent : SwarmAgent
{
    public TestCoverageAgent(
        Kernel kernel,
        AgentRoleConfiguration configuration,
        ILogger<SwarmAgent> logger)
        : base(AgentRole.TestCoverage, kernel, configuration, logger)
    {
    }

    protected override async Task<bool> ValidateRoleSpecificResponseAsync(AgentResponse response, CancellationToken cancellationToken)
    {
        var content = response.Content.ToLower();
        
        var hasTestElements = content.Contains("test") || content.Contains("coverage") ||
                             content.Contains("scenario") || content.Contains("assertion") ||
                             content.Contains("quality") || content.Contains("validation");
        
        if (!hasTestElements)
        {
            response.ValidationErrors.Add("Test Coverage response should contain testing and quality assurance information");
            return false;
        }

        var hasMetrics = Regex.IsMatch(content, @"\d+%") || content.Contains("percentage") ||
                        content.Contains("metric") || content.Contains("untested");
        
        if (hasMetrics)
        {
            response.Confidence = Math.Min(1.0, response.Confidence + 0.2);
        }

        return await Task.FromResult(true);
    }
}

public class RiskAssessmentAgent : SwarmAgent
{
    public RiskAssessmentAgent(
        Kernel kernel,
        AgentRoleConfiguration configuration,
        ILogger<SwarmAgent> logger)
        : base(AgentRole.RiskAssessment, kernel, configuration, logger)
    {
    }

    protected override async Task<bool> ValidateRoleSpecificResponseAsync(AgentResponse response, CancellationToken cancellationToken)
    {
        var content = response.Content.ToLower();
        
        var hasRiskElements = content.Contains("risk") || content.Contains("breaking") ||
                             content.Contains("impact") || content.Contains("vulnerability") ||
                             content.Contains("security") || content.Contains("performance");
        
        if (!hasRiskElements)
        {
            response.ValidationErrors.Add("Risk Assessment response should contain risk and impact analysis");
            return false;
        }

        var hasSeverityIndicators = content.Contains("high") || content.Contains("medium") ||
                                   content.Contains("low") || content.Contains("critical") ||
                                   content.Contains("severe") || content.Contains("moderate");
        
        if (hasSeverityIndicators)
        {
            response.Confidence = Math.Min(1.0, response.Confidence + 0.2);
        }

        return await Task.FromResult(true);
    }
}

public class IntegrationAnalyzerAgent : SwarmAgent
{
    public IntegrationAnalyzerAgent(
        Kernel kernel,
        AgentRoleConfiguration configuration,
        ILogger<SwarmAgent> logger)
        : base(AgentRole.IntegrationAnalyzer, kernel, configuration, logger)
    {
    }

    protected override async Task<bool> ValidateRoleSpecificResponseAsync(AgentResponse response, CancellationToken cancellationToken)
    {
        var content = response.Content.ToLower();
        
        var hasIntegrationElements = content.Contains("integration") || content.Contains("api") ||
                                    content.Contains("service") || content.Contains("dependency") ||
                                    content.Contains("contract") || content.Contains("interface");
        
        if (!hasIntegrationElements)
        {
            response.ValidationErrors.Add("Integration Analyzer response should contain system integration information");
            return false;
        }

        var hasSystemMapping = content.Contains("downstream") || content.Contains("upstream") ||
                              content.Contains("mesh") || content.Contains("endpoint") ||
                              content.Contains("contract");
        
        if (hasSystemMapping)
        {
            response.Confidence = Math.Min(1.0, response.Confidence + 0.15);
        }

        return await Task.FromResult(true);
    }
}

public class SummaryGeneratorAgent : SwarmAgent
{
    public SummaryGeneratorAgent(
        Kernel kernel,
        AgentRoleConfiguration configuration,
        ILogger<SwarmAgent> logger)
        : base(AgentRole.SummaryGenerator, kernel, configuration, logger)
    {
    }

    protected override async Task<bool> ValidateRoleSpecificResponseAsync(AgentResponse response, CancellationToken cancellationToken)
    {
        var content = response.Content.ToLower();
        
        var hasSummaryElements = content.Contains("summary") || content.Contains("overview") ||
                                content.Contains("synthesis") || content.Contains("conclusion") ||
                                content.Contains("recommendation") || content.Contains("findings");
        
        if (!hasSummaryElements)
        {
            response.ValidationErrors.Add("Summary Generator response should contain synthesis and summary language");
            return false;
        }

        if (content.Length < 100)
        {
            response.ValidationErrors.Add("Summary should be comprehensive and detailed");
            return false;
        }

        var hasStructure = content.Contains("1.") || content.Contains("•") || 
                          content.Contains("-") || content.Contains("summary:") ||
                          content.Contains("recommendations:");
        
        if (hasStructure)
        {
            response.Confidence = Math.Min(1.0, response.Confidence + 0.1);
        }

        return await Task.FromResult(true);
    }
}

public class LearningAgent : SwarmAgent
{
    public LearningAgent(
        Kernel kernel,
        AgentRoleConfiguration configuration,
        ILogger<SwarmAgent> logger)
        : base(AgentRole.Learning, kernel, configuration, logger)
    {
    }

    protected override async Task<bool> ValidateRoleSpecificResponseAsync(AgentResponse response, CancellationToken cancellationToken)
    {
        var content = response.Content.ToLower();
        
        var hasLearningElements = content.Contains("learn") || content.Contains("improve") ||
                                 content.Contains("pattern") || content.Contains("feedback") ||
                                 content.Contains("optimize") || content.Contains("adapt");
        
        if (!hasLearningElements)
        {
            response.ValidationErrors.Add("Learning Agent response should contain learning and improvement insights");
            return false;
        }

        var hasMetaAnalysis = content.Contains("accuracy") || content.Contains("effectiveness") ||
                             content.Contains("performance") || content.Contains("trend") ||
                             content.Contains("outcome");
        
        if (hasMetaAnalysis)
        {
            response.Confidence = Math.Min(1.0, response.Confidence + 0.2);
        }

        return await Task.FromResult(true);
    }
}