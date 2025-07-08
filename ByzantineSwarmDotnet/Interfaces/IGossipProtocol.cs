using ByzantineSwarmDotnet.Models;

namespace ByzantineSwarmDotnet.Interfaces;

public interface IGossipProtocol
{
    Task DisseminateAsync(SignedMessage message, List<string> targets);
    Task<bool> ReceiveGossipAsync(SignedMessage message, string fromAgent);
    Task StartGossipRoundAsync();
    Task<List<SignedMessage>> GetUndeliveredMessagesAsync();
}