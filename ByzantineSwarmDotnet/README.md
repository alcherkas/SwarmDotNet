# Byzantine Swarm .NET - AI Agents Swarm Solution

This project implements a Byzantine fault-tolerant AI agents swarm system based on the design document `AI-Agents-Swarm-Solution-Design-v2.md`. The system provides distributed artificial intelligence capabilities with robust fault tolerance, security, and consensus mechanisms.

## Features

### Core Byzantine Fault Tolerance
- **PBFT Consensus**: Practical Byzantine Fault Tolerance consensus protocol
- **3f+1 Agent Requirement**: Automatically maintains minimum agents for fault tolerance
- **Reputation System**: Trust-based agent evaluation and management
- **Byzantine Attack Detection**: Real-time detection and mitigation of malicious behavior

### Hierarchical Agent Architecture
- **Multi-Level Organization**: Executive, Supervisor, and Worker levels
- **Specialized Roles**: Supervisor, Validator, Security, Monitoring, etc.
- **Dynamic Scaling**: Automatic agent provisioning based on workload
- **Load Balancing**: Intelligent task distribution across agents

### Security & Reliability
- **Digital Signatures**: ECDSA-P256 cryptographic message signing
- **Circuit Breakers**: Cascade failure prevention with Polly integration
- **Fault Detection**: Advanced anomaly and Byzantine behavior detection
- **Recovery Mechanisms**: Automatic system healing and agent replacement

### AI Integration
- **Semantic Kernel**: Microsoft Semantic Kernel for AI capabilities
- **Ollama Support**: Local LLM integration with connection pooling
- **Task Distribution**: Intelligent decomposition and redundant execution

## Architecture Components

```
┌─────────────────────────────────────────────────────────────┐
│                ByzantineTolerantSwarmOrchestrator           │
├─────────────────────────────────────────────────────────────┤
│  HierarchicalAgentPool  │  ByzantineTaskDistributor       │
│  ReputationSystem       │  PBFTConsensusEngine            │
│  FaultDetector         │  CommunicationHub               │
│  CircuitBreakers       │  CryptographicSigner            │
└─────────────────────────────────────────────────────────────┘
                           │
         ┌─────────────────┼─────────────────┐
         │                 │                 │
   ┌─────▼──────┐   ┌─────▼──────┐   ┌─────▼──────┐
   │ Supervisor │   │ Validator  │   │   Worker   │
   │   Agent    │   │   Agent    │   │   Agent    │
   └────────────┘   └────────────┘   └────────────┘
```

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- Ollama (running locally on port 11434)
- A compatible LLM model (default: gemma3:4b-it-q8_0)

### Installation

1. Clone the repository:
```bash
git clone <repository-url>
cd ByzantineSwarmDotnet
```

2. Install dependencies:
```bash
dotnet restore
```

3. Ensure Ollama is running with the required model:
```bash
ollama serve
ollama pull gemma3:4b-it-q8_0
```

4. Configure the system (optional - edit `appsettings.json`):
```json
{
  "EnhancedSwarmConfiguration": {
    "MaxAgents": 16,
    "RequiredAgents": 16,
    "DefaultModel": "gemma3:4b-it-q8_0"
  }
}
```

### Running the Demo

```bash
dotnet run
```

The demo will automatically:
1. Initialize a swarm with 16 agents
2. Run various test scenarios including Byzantine fault tolerance tests
3. Display real-time metrics and system status

## Demo Scenarios

### Scenario 1: Simple Distributed Task
- Basic task distribution across multiple agents
- No consensus requirement
- Demonstrates basic swarm coordination

### Scenario 2: Consensus-Required Task
- High-security task requiring PBFT consensus
- Multiple validator participation
- Demonstrates Byzantine fault tolerance

### Scenario 3: Byzantine Fault Tolerance Test
- Simulates malicious agents with low reputation
- Tests system resilience under attack
- Demonstrates automatic fault detection and isolation

### Scenario 4: System Monitoring
- Real-time metrics collection
- Health monitoring and alerting
- Recovery mechanism testing

## Configuration

### Key Configuration Sections

**Enhanced Swarm Configuration**:
- `MaxAgents`: Maximum number of agents in the swarm
- `RequiredAgents`: Target number of agents for optimal operation
- `MaxByzantineFaults`: Maximum Byzantine faults to tolerate

**Byzantine Fault Tolerance**:
- `EnableBFT`: Enable/disable Byzantine fault tolerance
- `ConsensusAlgorithm`: Consensus algorithm (PBFT)
- `MinConsensusParticipants`: Minimum agents for consensus

**Security Configuration**:
- `EnableMessageSigning`: Enable cryptographic message signing
- `SignatureAlgorithm`: Digital signature algorithm (ECDSA-P256)

## Project Structure

```
ByzantineSwarmDotnet/
├── Models/
│   └── CoreModels.cs           # Data models and types
├── Configuration/
│   └── SwarmConfiguration.cs   # Configuration classes
├── Interfaces/
│   └── ISwarmInterfaces.cs     # Service interfaces
├── Agents/
│   ├── EnhancedSwarmAgentBase.cs
│   ├── SupervisorAgent.cs
│   └── ValidatorAgent.cs
├── Services/
│   ├── ByzantineTolerantSwarmOrchestrator.cs
│   ├── HierarchicalAgentPool.cs
│   ├── ByzantineTaskDistributor.cs
│   ├── ReputationSystem.cs
│   ├── ByzantineDetector.cs
│   ├── CryptographicSigner.cs
│   ├── CircuitBreakerRegistry.cs
│   └── StubImplementations.cs
├── appsettings.json            # Configuration file
└── Program.cs                  # Main application entry point
```

## Key Features Implemented

✅ **Byzantine Fault Tolerance**: 3f+1 agent requirement with PBFT consensus  
✅ **Hierarchical Architecture**: Multi-level agent organization  
✅ **Reputation System**: Trust-based agent evaluation  
✅ **Cryptographic Security**: Digital signatures and message authentication  
✅ **Circuit Breakers**: Cascade failure prevention  
✅ **Byzantine Detection**: Real-time malicious behavior detection  
✅ **Dynamic Scaling**: Automatic agent provisioning  
✅ **Task Distribution**: Intelligent workload allocation  
✅ **Real-time Monitoring**: System metrics and health monitoring  
✅ **Recovery Mechanisms**: Automatic fault recovery  

## Performance Characteristics

- **Consensus Latency**: Sub-second for small swarms (<20 agents)
- **Fault Tolerance**: Tolerates up to 1/3 malicious agents
- **Scalability**: Logarithmic communication complexity with hierarchical structure
- **Security**: ECDSA-P256 cryptographic signatures
- **Availability**: Automatic recovery from Byzantine attacks

## Limitations and Future Enhancements

### Current Limitations
- Simplified PBFT implementation (production would need full three-phase protocol)
- In-memory storage (production would use distributed storage)
- Basic Byzantine detection (could be enhanced with ML)
- Local Ollama only (could support distributed LLM endpoints)

### Planned Enhancements
- Quantum-resistant cryptography
- Multi-cloud deployment support
- Advanced AI safety integration
- Blockchain-based reputation tracking
- Formal verification of Byzantine properties

## Contributing

This implementation serves as a proof-of-concept for Byzantine fault-tolerant AI agent swarms. Contributions are welcome for:

- Enhanced Byzantine detection algorithms
- Production-grade PBFT implementation
- Advanced cryptographic features
- Performance optimizations
- Additional agent specializations

## License

[Specify license information]

## References

The implementation is based on research from:
- Lamport, L., Shostak, R., & Pease, M. (1982). "The Byzantine Generals Problem"
- Castro, M., & Liskov, B. (2002). "Practical Byzantine Fault Tolerance"
- Microsoft Semantic Kernel documentation
- .NET distributed systems best practices
