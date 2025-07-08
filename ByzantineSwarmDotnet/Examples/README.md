# ByzantineSwarmDotnet Examples

This directory contains comprehensive examples demonstrating the Byzantine-tolerant AI Agent Swarm for code review analysis and other distributed computing scenarios.

## Overview

The Byzantine Swarm examples showcase how to use the fault-tolerant agent swarm system with:

- **Byzantine Fault Tolerance**: Resilient to up to 1/3 malicious or faulty agents
- **Consensus Mechanisms**: PBFT (Practical Byzantine Fault Tolerance) for critical decisions
- **Reputation System**: Track and maintain agent trustworthiness
- **Cryptographic Security**: Digital signatures and message authentication
- **Hierarchical Architecture**: Multi-level agent coordination
- **Real-time Monitoring**: Continuous health and performance tracking

## Examples Structure

### 1. Basic Usage Examples (`ByzantineSwarmUsageExample.cs`)

**Pull Request Analysis Examples:**
- Basic PR analysis with Byzantine tolerance
- Complex feature analysis with consensus validation
- Security-focused analysis with enhanced validation
- Byzantine attack simulation and recovery
- Custom workflow with fault tolerance
- Reputation system tracking

**Key Features:**
- Consensus-based decision making
- Cryptographic verification
- Automatic fault detection and recovery
- Reputation-weighted voting
- Real-time Byzantine attack detection

### 2. Detailed Workflows (`Workflows/ByzantineCodeReviewWorkflow.cs`)

**Complete Workflow Examples:**
- Byzantine bug fix review workflow
- Byzantine feature development workflow
- Maximum security review workflow
- Byzantine attack simulation workflow

**Advanced Scenarios:**
- Parallel Byzantine workflow execution
- Adaptive workflow based on threat assessment
- Continuous learning with reputation tracking
- High-load fault tolerance testing
- Consensus algorithm effectiveness demonstration
- Reputation system resilience testing

### 3. Sample Data (`SampleData/SamplePRData.cs`)

**Enhanced Sample Data:**
- Byzantine-validated pull request data
- Security-critical change examples
- Attack simulation scenarios
- Jira integration with Byzantine context
- Configuration samples for different security levels

## Running Examples

### Interactive Mode

```bash
cd Examples
dotnet run
```

This will show a menu with available examples:

1. **Basic Pull Request Analysis Examples** - Fundamental Byzantine swarm usage
2. **Detailed Code Review Workflow** - Complete workflow demonstrations
3. **Advanced Workflow Scenarios** - Complex scenarios and optimizations
4. **Byzantine Fault Tolerance Examples** - Attack simulations and resilience testing
5. **Run All Examples** - Execute all examples in sequence

### Non-Interactive Mode

```bash
# Run specific example sets
dotnet run basic      # Basic examples
dotnet run workflow   # Workflow examples
dotnet run advanced   # Advanced scenarios
dotnet run byzantine  # Byzantine fault tolerance
dotnet run all        # All examples

# Or use numbers
dotnet run 1  # Basic examples
dotnet run 2  # Workflow examples
dotnet run 3  # Advanced scenarios
dotnet run 4  # Byzantine fault tolerance
dotnet run 5  # All examples
```

## Configuration

The examples use layered configuration:

- **`appsettings.json`** - Production-ready configuration
- **`appsettings.Development.json`** - Development settings with lower thresholds
- **`appsettings.Example.json`** - Extended configuration for advanced scenarios

### Key Configuration Sections

#### Byzantine Fault Tolerance
```json
{
  "ByzantineFaultTolerance": {
    "EnableBFT": true,
    "ConsensusAlgorithm": "PBFT",
    "MinConsensusParticipants": 10,
    "MaxConsensusTime": 15000,
    "ConsensusThreshold": 0.67,
    "MaxMaliciousAgents": 5
  }
}
```

#### Reputation System
```json
{
  "ReputationSystem": {
    "EnableReputationTracking": true,
    "InitialReputation": 0.5,
    "MinTrustThreshold": 0.3,
    "ReputationUpdateConsensus": true,
    "MaxReputationChange": 0.2
  }
}
```

#### Security Settings
```json
{
  "Security": {
    "EnableMessageSigning": true,
    "EnableAgentAuthentication": true,
    "CryptographicAlgorithm": "ECDSA-P256",
    "SecurityLevel": "High"
  }
}
```

## Example Scenarios

### 1. Basic Byzantine PR Analysis

Demonstrates fundamental Byzantine fault tolerance:

```csharp
var swarmTask = new SwarmTask
{
    Description = "Byzantine-tolerant analysis: Fix authentication bug",
    RequiredConsensus = true,
    FaultToleranceLevel = FaultToleranceLevel.Byzantine,
    SecurityLevel = SecurityLevel.High,
    RequiredRoles = new[] { "PRExtractor", "CodeAnalyzer", "Validator" }
};

var result = await orchestrator.ExecuteTaskAsync(swarmTask);
```

### 2. Security-Critical Analysis

Enhanced validation for security-sensitive changes:

```csharp
var securityTask = new SwarmTask
{
    Description = "Maximum security validation: JWT token updates",
    RequiredConsensus = true,
    FaultToleranceLevel = FaultToleranceLevel.Byzantine,
    SecurityLevel = SecurityLevel.Critical,
    RequiredRoles = new[] { "Security", "Validator", "RiskAssessment" }
};
```

### 3. Byzantine Attack Simulation

Test system resilience under adversarial conditions:

```csharp
var attackTask = new SwarmTask
{
    Description = "Byzantine attack simulation: 30% malicious agents",
    Parameters = new Dictionary<string, object>
    {
        ["simulate_byzantine_attack"] = true,
        ["malicious_agent_percentage"] = 0.30,
        ["attack_type"] = "coordinated_byzantine"
    }
};
```

### 4. Adaptive Workflow

Dynamic security escalation based on threat assessment:

```csharp
// Initial assessment
var initialResult = await orchestrator.ExecuteTaskAsync(threatAssessment);

// Adapt based on results
if (initialResult.TrustScore < 0.7)
{
    // Escalate to higher security protocol
    securityLevel = SecurityLevel.Critical;
    additionalRoles.Add("Security");
}
```

## Monitoring and Metrics

The examples provide comprehensive monitoring:

### Byzantine Fault Metrics
- **Consensus Success Rate**: Percentage of successful consensus decisions
- **Trust Score**: Overall system trustworthiness
- **Byzantine Faults Detected**: Number of malicious/faulty agents identified
- **Recovery Time**: Time to recover from Byzantine attacks
- **Quarantine Actions**: Malicious agents isolated

### Performance Metrics
- **Execution Time**: Task completion time with Byzantine overhead
- **Consensus Time**: Time to reach consensus decisions
- **Agent Participation**: Number of agents involved in decisions
- **Reputation Updates**: Changes to agent reputation scores

### Security Metrics
- **Attack Detection Rate**: Percentage of attacks successfully identified
- **False Positive Rate**: Incorrect Byzantine fault detections
- **Cryptographic Verification**: Message signature validation success
- **Security Level Compliance**: Adherence to security requirements

## Advanced Features

### Hierarchical Agent Architecture
- **Executive Level**: High-level decision making
- **Coordinator Level**: Workflow orchestration
- **Supervisor Level**: Task management and delegation
- **Worker Level**: Specialized task execution

### Reputation-Weighted Consensus
- Agents with higher reputation have more influence
- Automatic reputation adjustment based on performance
- Consensus validation for reputation updates
- Protection against reputation manipulation

### Cryptographic Security
- ECDSA-P256 digital signatures for all messages
- Automatic key rotation
- Message integrity verification
- Secure communication channels

### Automatic Recovery
- Real-time Byzantine fault detection
- Automatic quarantine of malicious agents
- Dynamic agent pool adjustment
- Consensus repair mechanisms

## Performance Considerations

### Scalability
- **Agent Count**: Supports up to 20 agents with Byzantine tolerance
- **Consensus Overhead**: ~15-25% performance impact for fault tolerance
- **Memory Usage**: Efficient agent pooling and recycling
- **Network Efficiency**: Message batching and compression

### Optimization Strategies
- **Parallel Execution**: Independent tasks run concurrently
- **Adaptive Thresholds**: Dynamic adjustment based on conditions
- **Circuit Breakers**: Prevent cascade failures
- **Load Balancing**: Distribute work across healthy agents

## Troubleshooting

### Common Issues

1. **Consensus Failures**
   - Check agent count meets minimum requirements (3F+1)
   - Verify network connectivity between agents
   - Ensure sufficient reputation scores

2. **Byzantine Detection False Positives**
   - Adjust detection thresholds in configuration
   - Review reputation system parameters
   - Check for network timing issues

3. **Performance Degradation**
   - Monitor agent pool health
   - Check for excessive Byzantine faults
   - Verify consensus timeout settings

### Debugging

Enable detailed logging for troubleshooting:

```json
{
  "Logging": {
    "LogLevel": {
      "ByzantineSwarmDotnet": "Trace"
    }
  }
}
```

## Contributing

When adding new examples:

1. Follow the existing pattern structure
2. Include Byzantine fault tolerance considerations
3. Add comprehensive logging and monitoring
4. Document security implications
5. Include performance metrics
6. Test with various attack scenarios

## Security Considerations

- All examples use cryptographic signatures
- Consensus is required for critical decisions
- Agent reputation is continuously monitored
- Automatic quarantine for malicious behavior
- Secure communication channels
- Regular security audits and penetration testing

## License

This project is licensed under the MIT License - see the LICENSE file for details.