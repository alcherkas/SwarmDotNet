namespace ByzantineSwarmDotnet.Interfaces;

public interface INetworkPartitionDetector
{
    Task<List<string>> DetectPartitionedAgentsAsync();
    Task<bool> IsAgentReachableAsync(string agentId);
    Task<Dictionary<string, List<string>>> GetNetworkTopologyAsync();
    Task NotifyPartitionDetectedAsync(List<string> partitionedAgents);
}