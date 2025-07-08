using ByzantineSwarmDotnet.Models;

namespace ByzantineSwarmDotnet.Interfaces;

public interface IMessageStore
{
    Task StoreMessageAsync(SignedMessage message);
    Task<List<SignedMessage>> GetMessagesAsync(string agentId);
    Task<List<SignedMessage>> GetMessagesByTypeAsync(string messageType);
    Task<SignedMessage?> GetMessageAsync(string messageId);
}