using ByzantineSwarmDotnet.Models;

namespace ByzantineSwarmDotnet.Interfaces;

public interface ILoadBalancer
{
    Task<string> SelectOptimalAgentAsync(List<string> candidateAgents, AgentTask task);
    Task<Dictionary<string, int>> GetCurrentLoadDistributionAsync();
    Task RebalanceLoadAsync();
    Task<bool> IsAgentOverloadedAsync(string agentId);
}