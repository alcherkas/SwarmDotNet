namespace ByzantineSwarmDotnet.Configuration;

public class EnhancedSwarmConfiguration
{
    public int MaxAgents { get; set; } = 16;
    public int MinAgents { get; set; } = 10;
    public int MaxByzantineFaults { get; set; } = 5;
    public int RequiredAgents { get; set; } = 16;
    public string DefaultModel { get; set; } = "gemma3:4b-it-q8_0";
    public string OptimizationStrategy { get; set; } = "HybridLamarckian";
    public int CommunicationTimeout { get; set; } = 30000;
    public int TaskTimeout { get; set; } = 300000;
    public int ConsensusTimeout { get; set; } = 15000;
    public int HeartbeatInterval { get; set; } = 5000;
    public int FaultDetectionThreshold { get; set; } = 3;
    public double ReputationDecayRate { get; set; } = 0.95;
    public int HierarchyLevels { get; set; } = 3;
    public double SupervisorRatio { get; set; } = 0.25;
}

public class ByzantineFaultToleranceConfiguration
{
    public bool EnableBFT { get; set; } = true;
    public string ConsensusAlgorithm { get; set; } = "PBFT";
    public int MinConsensusParticipants { get; set; } = 10;
    public int MaxConsensusTime { get; set; } = 15000;
    public bool MessageVerificationEnabled { get; set; } = true;
    public int RequiredSignatures { get; set; } = 7;
    public string SignatureAlgorithm { get; set; } = "ECDSA-P256";
    public bool EnableGossipProtocol { get; set; } = true;
    public int GossipFanout { get; set; } = 3;
    public int GossipInterval { get; set; } = 2000;
}

public class ReputationSystemConfiguration
{
    public bool EnableReputationTracking { get; set; } = true;
    public double InitialReputation { get; set; } = 0.5;
    public double MinTrustThreshold { get; set; } = 0.3;
    public double ReputationDecayRate { get; set; } = 0.99;
    public double MaliciousDetectionThreshold { get; set; } = 0.2;
    public string ReputationAggregationMethod { get; set; } = "WeightedAverage";
}

public class FaultToleranceConfiguration
{
    public int CircuitBreakerThreshold { get; set; } = 5;
    public int CircuitBreakerTimeout { get; set; } = 60000;
    public int MaxRetryAttempts { get; set; } = 3;
    public double RetryBackoffMultiplier { get; set; } = 2.0;
    public int HealthCheckInterval { get; set; } = 10000;
    public bool EnableBulkheadIsolation { get; set; } = true;
    public int MaxConcurrentOperations { get; set; } = 100;
}

public class SecurityConfiguration
{
    public bool EnableMessageSigning { get; set; } = true;
    public bool EnableAgentAuthentication { get; set; } = true;
    public bool CertificateValidation { get; set; } = true;
    public bool EnableSecureEnclaves { get; set; } = false;
    public bool AuditLogEnabled { get; set; } = true;
    public int SessionTimeout { get; set; } = 3600000;
}

public class MonitoringConfiguration
{
    public bool EnableDistributedTracing { get; set; } = true;
    public int MetricsCollectionInterval { get; set; } = 5000;
    public bool EnableAnomalyDetection { get; set; } = true;
    public AlertThresholdsConfiguration AlertThresholds { get; set; } = new();
}

public class AlertThresholdsConfiguration
{
    public int HighLatency { get; set; } = 5000;
    public double HighErrorRate { get; set; } = 0.05;
    public double LowConsensusParticipation { get; set; } = 0.7;
}

public class OllamaConfiguration
{
    public string Endpoint { get; set; } = "http://localhost:11434";
    public List<string> Models { get; set; } = new();
    public ConnectionPoolConfiguration ConnectionPool { get; set; } = new();
    public LoadBalancingConfiguration LoadBalancing { get; set; } = new();
}

public class ConnectionPoolConfiguration
{
    public int MaxConnections { get; set; } = 20;
    public int ConnectionTimeout { get; set; } = 10000;
    public int RetryAttempts { get; set; } = 3;
    public int HealthCheckInterval { get; set; } = 30000;
}

public class LoadBalancingConfiguration
{
    public string Strategy { get; set; } = "RoundRobin";
    public bool EnableFailover { get; set; } = true;
    public string HealthCheckEndpoint { get; set; } = "/api/health";
}

public class SwarmConfiguration
{
    public EnhancedSwarmConfiguration EnhancedSwarmConfiguration { get; set; } = new();
    public ByzantineFaultToleranceConfiguration ByzantineFaultTolerance { get; set; } = new();
    public ReputationSystemConfiguration ReputationSystem { get; set; } = new();
    public FaultToleranceConfiguration FaultTolerance { get; set; } = new();
    public SecurityConfiguration Security { get; set; } = new();
    public MonitoringConfiguration Monitoring { get; set; } = new();
    public OllamaConfiguration Ollama { get; set; } = new();
}
