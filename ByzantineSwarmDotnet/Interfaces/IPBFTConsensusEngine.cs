using ByzantineSwarmDotnet.Models;

namespace ByzantineSwarmDotnet.Interfaces;

public interface IPBFTConsensusEngine
{
    Task<ConsensusResult> ReachConsensusAsync(object proposal);
    Task<ConsensusVote> VoteOnProposalAsync(string agentId, Proposal proposal);
    Task<bool> ValidateProposalAsync(Proposal proposal);
    Task<ConsensusResult> ExecuteThreePhaseProtocolAsync(Proposal proposal);
}