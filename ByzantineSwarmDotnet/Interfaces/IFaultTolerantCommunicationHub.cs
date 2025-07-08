using ByzantineSwarmDotnet.Models;

namespace ByzantineSwarmDotnet.Interfaces;

public interface IFaultTolerantCommunicationHub
{
    // Basic communication
    Task SendMessageAsync(string fromAgent, string toAgent, SignedMessage message);
    Task BroadcastAsync(string fromAgent, SignedMessage message);
    Task<List<SignedMessage>> GetMessagesAsync(string agentId);
    
    // Byzantine fault tolerant communication
    Task<bool> ReliableMulticastAsync(string fromAgent, SignedMessage message, List<string> recipients);
    Task<ConsensusResult> InitiateConsensusAsync(string initiator, Proposal proposal);
    Task<bool> GossipProtocolAsync(string fromAgent, SignedMessage message);
    
    // Security and verification
    Task<bool> VerifyMessageIntegrityAsync(SignedMessage message);
    Task<bool> AuthenticateAgentAsync(string agentId, AuthenticationToken token);
    
    // Fault detection and recovery
    Task<List<string>> DetectPartitionedAgentsAsync();
    Task InitiateRecoveryProtocolAsync(List<string> faultyAgents);
}