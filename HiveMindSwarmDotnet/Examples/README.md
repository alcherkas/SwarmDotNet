# HiveMind Swarm Examples

This directory contains comprehensive examples demonstrating how to use the AI Agent Swarm for code review and analysis workflows.

## 📁 Directory Structure

```
Examples/
├── README.md                          # This file
├── SwarmUsageExample.cs               # Main usage examples
├── SampleData/
│   └── SamplePRData.cs               # Sample PR and Jira data
├── Workflows/
│   └── CodeReviewWorkflow.cs         # Detailed workflow demonstrations
└── appsettings.Example.json          # Example configuration
```

## 🚀 Getting Started

### 1. Prerequisites

- .NET 9.0 or later
- Ollama running locally with required models
- Sample models: `gemma3:4b-it-q8_0`, `llama3.1:8b`, `qwen2.5:7b`

### 2. Configuration

Copy the example configuration and update with your settings:

```bash
cp Examples/appsettings.Example.json Examples/appsettings.json
```

Update the configuration with your Ollama endpoint and available models.

### 3. Running Examples

```csharp
// Basic usage
var example = serviceProvider.GetRequiredService<SwarmUsageExample>();
await example.RunPullRequestAnalysisExampleAsync();

// Detailed workflow
var workflow = serviceProvider.GetRequiredService<CodeReviewWorkflow>();
await workflow.RunCompleteWorkflowAsync();
```

## 📋 Example Scenarios

### 1. Basic PR Analysis (`SwarmUsageExample.cs`)

Demonstrates simple pull request analysis workflows:

- **Bug Fix Analysis**: Analyzes a simple bug fix PR
- **Complex Feature Analysis**: Reviews a major feature implementation
- **Security Risk Assessment**: Focuses on security-related changes
- **Custom Workflow**: Shows how to create targeted analysis workflows

### 2. Detailed Workflows (`CodeReviewWorkflow.cs`)

Shows step-by-step agent coordination:

- **Complete Workflow**: Full end-to-end analysis process
- **Agent Coordination**: How agents communicate and collaborate
- **Parallel Processing**: Multiple PRs analyzed simultaneously
- **Adaptive Workflows**: Dynamic workflow adjustment based on initial findings
- **Learning Workflows**: Continuous improvement based on feedback

### 3. Sample Data (`SampleData/SamplePRData.cs`)

Provides realistic test data:

- **Bug Fix PR**: Memory leak fix with test cases
- **New Feature PR**: OAuth2 integration implementation
- **Security PR**: JWT validation and rate limiting enhancements
- **Jira Context**: Business requirements and acceptance criteria

## 🤖 Agent Roles in Examples

| Agent | Role | Example Usage |
|-------|------|---------------|
| 🎭 **Orchestrator** | Workflow coordination | Manages task distribution and sequencing |
| 📥 **PRExtractor** | Data extraction | Parses PR metadata, file changes, and commits |
| 📋 **JiraContext** | Business context | Retrieves requirements and acceptance criteria |
| 🔍 **CodeAnalyzer** | Technical analysis | Analyzes code quality, patterns, and complexity |
| 🗺️ **RequirementMapper** | Traceability | Maps code changes to business requirements |
| 🧪 **TestCoverage** | Quality assurance | Evaluates test completeness and quality |
| ⚠️ **RiskAssessment** | Risk identification | Identifies security and operational risks |
| 🔌 **IntegrationAnalyzer** | System integration | Analyzes API and service dependencies |
| 📄 **SummaryGenerator** | Result synthesis | Creates comprehensive analysis summaries |
| 🧠 **Learning** | Continuous improvement | Learns from feedback to improve accuracy |

## 🔧 Usage Patterns

### Basic Analysis Pattern

```csharp
var swarmTask = new SwarmTask
{
    Description = "Analyze pull request changes",
    RequiredRoles = new[]
    {
        AgentRole.PRExtractor,
        AgentRole.CodeAnalyzer,
        AgentRole.SummaryGenerator
    },
    Parameters = new Dictionary<string, object>
    {
        ["pr_url"] = "https://github.com/org/repo/pull/123",
        ["files_changed"] = new[] { "src/file1.cs", "src/file2.cs" }
    }
};

var result = await orchestrator.ExecuteTaskAsync(swarmTask);
```

### Security-Focused Analysis

```csharp
var securityTask = new SwarmTask
{
    Description = "Security review of authentication changes",
    RequiredRoles = new[]
    {
        AgentRole.PRExtractor,
        AgentRole.CodeAnalyzer,
        AgentRole.RiskAssessment,
        AgentRole.IntegrationAnalyzer,
        AgentRole.SummaryGenerator
    },
    Parameters = new Dictionary<string, object>
    {
        ["security_sensitive"] = true,
        ["focus"] = "authentication_and_authorization"
    }
};
```

### Comprehensive Feature Review

```csharp
var featureTask = new SwarmTask
{
    Description = "Complete feature analysis with business context",
    RequiredRoles = new[]
    {
        AgentRole.PRExtractor,
        AgentRole.JiraContext,
        AgentRole.CodeAnalyzer,
        AgentRole.RequirementMapper,
        AgentRole.TestCoverage,
        AgentRole.IntegrationAnalyzer,
        AgentRole.SummaryGenerator
    },
    Parameters = new Dictionary<string, object>
    {
        ["jira_tickets"] = new[] { "FEAT-123", "FEAT-124" },
        ["epic"] = "EPIC-456"
    }
};
```

## 📊 Example Output

### Agent Coordination Log
```
🚀 Starting Complete Code Review Workflow
================================================
🔧 Step 1: Initializing Workflow
✅ Workflow initialized

🐛 Step 2: Bug Fix Review Workflow
================================================
📋 Analyzing PR: Analyze bug fix: Resolve memory leak in background service
🎯 Required Agents: PRExtractor, CodeAnalyzer, RiskAssessment, TestCoverage, SummaryGenerator

🔄 Executing Bug Fix Analysis...
   🤖 Activating PRExtractor agent...
   🤖 Activating CodeAnalyzer agent...
   🤖 Activating RiskAssessment agent...
   🤖 Activating TestCoverage agent...
   🤖 Activating SummaryGenerator agent...
✅ Bug Fix Analysis completed in 3.2s
   📈 Overall Confidence: 87%
   🎯 Success: True

📊 Bug Fix Workflow Results:
   ⏱️  Execution Time: 3.2s
   🤖 Agents Involved: 5
   💬 Communications: 12
   📥 PRExtractor: 92% confidence
   🔍 CodeAnalyzer: 88% confidence
   ⚠️ RiskAssessment: 85% confidence
   🧪 TestCoverage: 82% confidence
   📄 SummaryGenerator: 90% confidence
   📝 Summary: Memory leak successfully identified and fixed with proper HttpClient disposal pattern...
```

### Parallel Analysis Results
```
🚀 Running Parallel Workflow Example
📋 Processing 3 PRs in parallel...
✅ Parallel execution completed in 4.7s
📊 Results: 3/3 successful
   Task 1: 87% confidence
   Task 2: 91% confidence
   Task 3: 89% confidence
```

## 🔍 Customization

### Custom Agent Configuration

```json
{
  "AgentRoles": {
    "CustomAnalyzer": {
      "ModelId": "llama3.1:8b",
      "SystemPrompt": "You are a custom analysis specialist...",
      "MaxTokens": 4000
    }
  }
}
```

### Custom Workflow Creation

```csharp
public async Task CreateCustomWorkflowAsync()
{
    var customTask = new SwarmTask
    {
        Description = "Custom analysis workflow",
        RequiredRoles = new[] { /* your custom roles */ },
        Parameters = new Dictionary<string, object>
        {
            ["custom_parameter"] = "value"
        }
    };
    
    var result = await orchestrator.ExecuteTaskAsync(customTask);
}
```

## 🚨 Error Handling

The examples include comprehensive error handling patterns:

```csharp
try
{
    var result = await orchestrator.ExecuteTaskAsync(task);
    
    if (!result.IsSuccessful)
    {
        logger.LogWarning($"Analysis completed with issues: {result.Errors}");
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "Failed to execute swarm task");
    // Handle gracefully
}
```

## 📈 Performance Considerations

- **Parallel Execution**: Use `Task.WhenAll` for analyzing multiple PRs
- **Agent Pooling**: Agents are automatically pooled and reused
- **Timeouts**: Configure appropriate timeouts for complex analyses
- **Model Selection**: Choose appropriate models based on task complexity

## 🔗 Integration Examples

### GitHub Integration

```csharp
// Webhook handler example
public async Task HandlePRWebhookAsync(GitHubPRWebhook webhook)
{
    var task = new SwarmTask
    {
        Description = $"Analyze PR #{webhook.Number}",
        Parameters = new Dictionary<string, object>
        {
            ["pr_url"] = webhook.PullRequest.Url,
            ["files_changed"] = webhook.PullRequest.ChangedFiles
        }
    };
    
    var result = await orchestrator.ExecuteTaskAsync(task);
    await PostReviewCommentAsync(webhook.Number, result.FinalAnswer);
}
```

### Jira Integration

```csharp
// Jira ticket analysis
public async Task AnalyzeJiraTicketAsync(string ticketId)
{
    var jiraContext = await jiraClient.GetTicketAsync(ticketId);
    
    var task = new SwarmTask
    {
        RequiredRoles = new[] { AgentRole.JiraContext, AgentRole.RequirementMapper },
        Parameters = new Dictionary<string, object>
        {
            ["jira_ticket"] = jiraContext
        }
    };
}
```

## 🛠️ Troubleshooting

### Common Issues

1. **Ollama Connection**: Ensure Ollama is running and models are available
2. **Model Performance**: Use appropriate models for task complexity
3. **Timeout Issues**: Increase timeouts for complex analyses
4. **Memory Usage**: Monitor memory usage with large file sets

### Debug Mode

Enable debug logging to see detailed agent coordination:

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Debug",
      "HiveMindSwarmDotnet": "Trace"
    }
  }
}
```

## 📚 Next Steps

1. **Explore Examples**: Run the provided examples to understand the patterns
2. **Customize Workflows**: Create workflows specific to your use cases
3. **Integrate Systems**: Connect with your existing PR/review tools
4. **Monitor Performance**: Track agent performance and optimize accordingly
5. **Extend Capabilities**: Add custom agents for specialized analysis needs

For more information, see the main project documentation and API reference.