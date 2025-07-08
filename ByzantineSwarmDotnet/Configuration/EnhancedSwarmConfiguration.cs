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