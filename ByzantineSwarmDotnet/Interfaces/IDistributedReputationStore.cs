using ByzantineSwarmDotnet.Models;

namespace ByzantineSwarmDotnet.Interfaces;

public interface IDistributedReputationStore
{
    Task<double> GetReputationAsync(string agentId);
    Task UpdateReputationAsync(ReputationUpdate update);
    Task<Dictionary<string, double>> GetAllReputationsAsync();
    Task<List<ReputationUpdate>> GetReputationHistoryAsync(string agentId);
}