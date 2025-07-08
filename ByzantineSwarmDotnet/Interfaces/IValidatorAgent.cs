using ByzantineSwarmDotnet.Models;

namespace ByzantineSwarmDotnet.Interfaces;

public interface IValidatorAgent : IEnhancedSwarmAgent
{
    Task<bool> ValidateAgentResponseAsync(AgentResponse response);
    Task<bool> ValidateConsensusProposalAsync(Proposal proposal);
    Task<double> CalculateResponseConfidenceAsync(AgentResponse response);
}