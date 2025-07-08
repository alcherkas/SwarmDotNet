namespace ByzantineSwarmDotnet.Configuration;

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
