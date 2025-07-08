using ByzantineSwarmDotnet.Models;

namespace ByzantineSwarmDotnet.Interfaces;

public interface IAdvancedFaultDetector
{
    Task<bool> DetectByzantineFaultAsync(string agentId);
    Task<List<string>> GetFaultyAgentsAsync();
    Task<bool> ValidateAgentBehaviorAsync(string agentId, List<AgentMessage> recentMessages);
    Task<double> CalculateSuspiciousActivityScoreAsync(string agentId);
    Task NotifyFaultDetectedAsync(string agentId, string faultType);
}