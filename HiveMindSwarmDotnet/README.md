# HiveMind Swarm .NET

A sophisticated AI Agents Swarm implementation based on the research paper "Collective Intelligence in Multi-Agent Systems" (arXiv:2503.05473v2). This system demonstrates collective intelligence through coordinated problem-solving using specialized AI agents.

## 🚀 Features

- **Multi-Agent Architecture**: Specialized agents for different roles (Orchestrator, PRExtractor, JiraContext, CodeAnalyzer, RequirementMapper, TestCoverage, RiskAssessment, IntegrationAnalyzer, SummaryGenerator, Learning)
- **Dynamic Communication**: Graph-based communication topology with probabilistic links
- **Optimization Algorithms**: Darwinian and Lamarckian optimization strategies
- **Task Distribution**: Intelligent task distribution with dependency management
- **Real-time Monitoring**: Comprehensive logging and metrics collection
- **Ollama Integration**: Seamless integration with Ollama for local LLM execution
- **SemanticKernel**: Built on Microsoft's SemanticKernel framework

## 🏗️ Architecture

The system implements the Society of HiveMind (SOHM) framework with:

- **SwarmOrchestrator**: Central coordination engine
- **AgentPool**: Manages specialized AI agents
- **CommunicationHub**: Facilitates inter-agent messaging
- **TaskDistributor**: Handles task allocation and dependencies
- **OptimizationEngine**: Applies performance optimization strategies

## 🛠️ Prerequisites

- .NET 9.0 or later
- Ollama installed and running locally
- Available LLM models (gemma3:4b-it-q8_0, llama3.1:8b, qwen2.5:7b)

## 🚀 Quick Start

1. **Start Ollama**:
   ```bash
   ollama serve
   ```

2. **Pull required models**:
   ```bash
   ollama pull gemma3:4b-it-q8_0
   ollama pull llama3.1:8b
   ollama pull qwen2.5:7b
   ```

3. **Run the application**:
   ```bash
   dotnet run
   ```

## 📋 Example Usage

The system includes three demonstration scenarios:

### 1. Code Analysis
```csharp
var codeTask = new SwarmTask
{
    Description = "Analyze this code change: Added new authentication middleware to handle JWT tokens in the API gateway.",
    RequiredRoles = new[] { AgentRole.CodeAnalyzer, AgentRole.RiskAssessment },
    MaxExecutionTime = TimeSpan.FromMinutes(3)
};
```

### 2. PR Analysis
```csharp
var prTask = new SwarmTask
{
    Description = "Analyze this pull request: Feature/user-authentication - implements OAuth2 integration with Google and GitHub providers.",
    RequiredRoles = new[] { AgentRole.PRExtractor, AgentRole.JiraContext, AgentRole.RiskAssessment },
    MaxExecutionTime = TimeSpan.FromMinutes(4)
};
```

### 3. Comprehensive Analysis
```csharp
var analysisTask = new SwarmTask
{
    Description = "Analyze the impact of microservices migration on the current monolithic architecture.",
    RequiredRoles = new[] { AgentRole.CodeAnalyzer, AgentRole.IntegrationAnalyzer, AgentRole.SummaryGenerator },
    MaxExecutionTime = TimeSpan.FromMinutes(5)
};
```

## 🤖 Agent Roles

- **Orchestrator** 🎼: Coordinates and manages overall workflow
- **PRExtractor** 📋: Extracts and analyzes pull request data
- **JiraContext** 🎯: Gathers business context and requirements
- **CodeAnalyzer** 🔍: Analyzes code changes and technical implementation
- **RequirementMapper** 🗺️: Maps code changes to business requirements
- **TestCoverage** 🧪: Analyzes test coverage and quality
- **RiskAssessment** ⚠️: Assesses risks and potential issues
- **IntegrationAnalyzer** 🔗: Analyzes integration and system dependencies
- **SummaryGenerator** 📝: Generates comprehensive summaries and insights
- **Learning** 🧠: Learns and adapts from feedback

## 📊 Performance Metrics

The system tracks:
- Task completion rates
- Average execution times
- Agent utilization statistics
- Communication efficiency
- Overall confidence scores

## 🔧 Configuration

Edit `appsettings.json` to customize:

```json
{
  "SwarmConfiguration": {
    "MaxAgents": 10,
    "DefaultModel": "gemma3:4b-it-q8_0",
    "OptimizationStrategy": "Lamarckian",
    "CommunicationTimeout": 30000,
    "TaskTimeout": 300000
  },
  "Ollama": {
    "Endpoint": "http://localhost:11434",
    "Models": ["gemma3:4b-it-q8_0", "llama3.1:8b", "qwen2.5:7b"]
  }
}
```

## 🧪 Testing

Run the built-in examples to test the system:

```bash
dotnet run
```

The application will execute three different scenarios demonstrating various aspects of the swarm intelligence system.

## 📈 Monitoring

The system provides comprehensive logging with:
- Task execution tracking
- Agent performance metrics
- Communication patterns
- Error reporting
- Real-time status updates

## 🔬 Research Foundation

This implementation is based on the research paper:
- **Title**: "Collective Intelligence in Multi-Agent Systems"
- **arXiv**: 2503.05473v2
- **Focus**: Society of HiveMind (SOHM) framework for AI agent coordination

## 🤝 Contributing

Contributions are welcome! Please feel free to submit issues and pull requests.

## 📄 License

This project is open source and available under the MIT License.

## 🙏 Acknowledgments

- Research paper authors for the theoretical foundation
- Microsoft SemanticKernel team
- Ollama project for local LLM execution
- .NET community for the excellent tooling