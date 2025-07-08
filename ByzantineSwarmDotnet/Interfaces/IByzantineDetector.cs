using ByzantineSwarmDotnet.Models;

namespace ByzantineSwarmDotnet.Interfaces;

public interface IByzantineDetector
{
    Task<bool> DetectByzantineBehaviorAsync(AgentMessage message);
    Task<double> CalculateByzantineRiskScoreAsync(string agentId);
    Task<List<string>> IdentifyMaliciousAgentsAsync();
    Task NotifyByzantineAttackAsync(List<string> maliciousAgents);
}