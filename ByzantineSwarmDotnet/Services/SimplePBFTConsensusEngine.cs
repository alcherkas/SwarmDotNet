using ByzantineSwarmDotnet.Interfaces;
using ByzantineSwarmDotnet.Models;
using Microsoft.Extensions.Logging;

namespace ByzantineSwarmDotnet.Services;

public class SimplePBFTConsensusEngine : IPBFTConsensusEngine
{
    private readonly ILogger<SimplePBFTConsensusEngine> _logger;
    private readonly IHierarchicalAgentPool _agentPool;

    public SimplePBFTConsensusEngine(ILogger<SimplePBFTConsensusEngine> logger, IHierarchicalAgentPool agentPool)
    {
        _logger = logger;
        _agentPool = agentPool;
    }

    public async Task<ConsensusResult> ReachConsensusAsync(object proposal)
    {
        try
        {
            _logger.LogInformation("Starting consensus for proposal");
            
            var agents = await _agentPool.GetActiveAgentsAsync();
            var requiredVotes = (agents.Count * 2 / 3) + 1; // 2/3 majority
            
            // Simulate consensus process
            await Task.Delay(100); // Simulate network delay
            
            var acceptVotes = (int)(agents.Count * 0.8); // 80% acceptance rate
            var totalVotes = agents.Count;
            
            var consensusReached = acceptVotes >= requiredVotes;
            
            return new ConsensusResult
            {
                ProposalId = Guid.NewGuid().ToString(),
                IsAccepted = consensusReached,
                ConsensusReached = consensusReached,
                SafetyMaintained = true,
                TotalVotes = totalVotes,
                AcceptVotes = acceptVotes,
                RejectVotes = totalVotes - acceptVotes,
                HonestParticipants = totalVotes,
                ParticipantIds = agents.Select(a => a.Id).ToList(),
                CompletedAt = DateTime.UtcNow,
                Duration = TimeSpan.FromMilliseconds(100)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in consensus process");
            return ConsensusResult.Failed();
        }
    }

    public async Task<ConsensusVote> VoteOnProposalAsync(string agentId, Proposal proposal)
    {
        return await Task.FromResult(new ConsensusVote
        {
            VoterId = agentId,
            ProposalId = proposal.Id,
            Accept = true,
            Justification = "Proposal accepted",
            VotedAt = DateTime.UtcNow
        });
    }

    public async Task<bool> ValidateProposalAsync(Proposal proposal)
    {
        return await Task.FromResult(!string.IsNullOrEmpty(proposal.Id));
    }

    public async Task<ConsensusResult> ExecuteThreePhaseProtocolAsync(Proposal proposal)
    {
        return await ReachConsensusAsync(proposal);
    }
}