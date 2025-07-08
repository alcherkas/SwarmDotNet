using ByzantineSwarmDotnet.Models;

namespace ByzantineSwarmDotnet.Interfaces;

public interface IReputationSystem
{
    Task<double> GetReputationAsync(string agentId);
    Task<bool> UpdateReputationAsync(string agentId, ReputationUpdate update);
    Task<TrustScore> CalculateTrustScoreAsync(string agentId);
    Task<List<string>> GetTrustedAgentsAsync(double minThreshold);
    Task<List<string>> GetSuspiciousAgentsAsync();
    Task<bool> ValidateReputationUpdateAsync(ReputationUpdate update);
}