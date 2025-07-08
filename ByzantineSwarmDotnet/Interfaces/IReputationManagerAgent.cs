using ByzantineSwarmDotnet.Models;

namespace ByzantineSwarmDotnet.Interfaces;

public interface IReputationManagerAgent : IEnhancedSwarmAgent
{
    Task ProcessReputationUpdateAsync(ReputationUpdate update);
    Task<bool> ValidateReputationUpdateWithConsensusAsync(ReputationUpdate update);
    Task<Dictionary<string, double>> GetReputationSnapshotAsync();
}