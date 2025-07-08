using Microsoft.Extensions.Logging;
using ByzantineSwarmDotnet.Models;
using ByzantineSwarmDotnet.Interfaces;

namespace ByzantineSwarmDotnet.Services;

public class ByzantineDetector : IByzantineDetector
{
    private readonly ILogger<ByzantineDetector> _logger;
    private readonly IReputationSystem _reputationSystem;
    private readonly Dictionary<string, List<AgentMessage>> _messageHistory;
    private readonly Dictionary<string, DateTime> _lastActivityTime;
    private readonly Dictionary<string, double> _behaviorPatterns;
    private readonly object _lockObject = new();

    public ByzantineDetector(
        ILogger<ByzantineDetector> logger,
        IReputationSystem reputationSystem)
    {
        _logger = logger;
        _reputationSystem = reputationSystem;
        _messageHistory = new Dictionary<string, List<AgentMessage>>();
        _lastActivityTime = new Dictionary<string, DateTime>();
        _behaviorPatterns = new Dictionary<string, double>();
    }

    public async Task<bool> DetectByzantineBehaviorAsync(AgentMessage message)
    {
        try
        {
            var agentId = message.SenderId;
            var suspiciousIndicators = new List<string>();
            
            // Record message for pattern analysis
            RecordMessage(message);
            
            // Check for rapid-fire messaging (potential flooding attack)
            if (await DetectFloodingAttackAsync(agentId))
            {
                suspiciousIndicators.Add("Flooding attack");
            }
            
            // Check for contradictory messages
            if (await DetectContradictoryBehaviorAsync(agentId))
            {
                suspiciousIndicators.Add("Contradictory behavior");
            }
            
            // Check for timing anomalies
            if (await DetectTimingAnomaliesAsync(message))
            {
                suspiciousIndicators.Add("Timing anomalies");
            }
            
            // Check for content anomalies
            if (await DetectContentAnomaliesAsync(message))
            {
                suspiciousIndicators.Add("Content anomalies");
            }
            
            // Check for reputation manipulation attempts
            if (await DetectReputationManipulationAsync(message))
            {
                suspiciousIndicators.Add("Reputation manipulation");
            }
            
            // Determine if behavior is Byzantine
            var isByzantine = suspiciousIndicators.Count >= 2; // Multiple indicators suggest Byzantine behavior
            
            if (isByzantine)
            {
                _logger.LogWarning("Byzantine behavior detected for agent {AgentId}. Indicators: {Indicators}", 
                    agentId, string.Join(", ", suspiciousIndicators));
                
                // Update behavior pattern score
                await UpdateBehaviorPatternAsync(agentId, 0.2); // Increase suspicion
                
                // Notify reputation system
                var reputationUpdate = new ReputationUpdate
                {
                    AgentId = agentId,
                    UpdatedBy = "ByzantineDetector",
                    ScoreChange = -0.15, // Significant reputation penalty
                    Reason = $"Byzantine behavior detected: {string.Join(", ", suspiciousIndicators)}",
                    Evidence = new Dictionary<string, object>
                    {
                        ["DetectionTime"] = DateTime.UtcNow,
                        ["SuspiciousIndicators"] = suspiciousIndicators,
                        ["MessageId"] = message.Id
                    }
                };
                
                await _reputationSystem.UpdateReputationAsync(agentId, reputationUpdate);
            }
            else if (suspiciousIndicators.Any())
            {
                _logger.LogInformation("Minor suspicious behavior detected for agent {AgentId}: {Indicators}", 
                    agentId, string.Join(", ", suspiciousIndicators));
                
                // Minor reputation impact
                await UpdateBehaviorPatternAsync(agentId, 0.05);
            }
            else
            {
                // Good behavior - slightly improve pattern score
                await UpdateBehaviorPatternAsync(agentId, -0.01);
            }
            
            return isByzantine;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error detecting Byzantine behavior for message from {SenderId}", message.SenderId);
            return false; // Fail-safe: don't flag as Byzantine on detection errors
        }
    }

    public async Task<double> CalculateByzantineRiskScoreAsync(string agentId)
    {
        try
        {
            var factors = new List<double>();
            
            // Reputation factor
            var reputation = await _reputationSystem.GetReputationAsync(agentId);
            factors.Add(1.0 - reputation); // Lower reputation = higher risk
            
            // Behavior pattern factor
            lock (_lockObject)
            {
                if (_behaviorPatterns.TryGetValue(agentId, out var behaviorScore))
                {
                    factors.Add(behaviorScore);
                }
                else
                {
                    factors.Add(0.0); // No pattern data
                }
            }
            
            // Message frequency factor
            var messageFrequency = await CalculateMessageFrequencyRiskAsync(agentId);
            factors.Add(messageFrequency);
            
            // Response consistency factor
            var consistencyRisk = await CalculateConsistencyRiskAsync(agentId);
            factors.Add(consistencyRisk);
            
            // Weighted average with emphasis on reputation and behavior patterns
            var weights = new[] { 0.4, 0.3, 0.15, 0.15 }; // Must sum to 1.0
            var weightedScore = factors.Zip(weights, (f, w) => f * w).Sum();
            
            return Math.Max(0, Math.Min(1, weightedScore));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating Byzantine risk score for agent {AgentId}", agentId);
            return 0.5; // Neutral risk on error
        }
    }

    public async Task<List<string>> IdentifyMaliciousAgentsAsync()
    {
        try
        {
            var allAgents = new List<string>();
            
            lock (_lockObject)
            {
                allAgents.AddRange(_behaviorPatterns.Keys);
                allAgents.AddRange(_messageHistory.Keys);
            }
            
            var maliciousAgents = new List<string>();
            
            foreach (var agentId in allAgents.Distinct())
            {
                var riskScore = await CalculateByzantineRiskScoreAsync(agentId);
                if (riskScore > 0.7) // High risk threshold
                {
                    maliciousAgents.Add(agentId);
                }
            }
            
            return maliciousAgents;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error identifying malicious agents");
            return new List<string>();
        }
    }

    public async Task NotifyByzantineAttackAsync(List<string> maliciousAgents)
    {
        try
        {
            _logger.LogCritical("BYZANTINE ATTACK DETECTED! Malicious agents: {MaliciousAgents}", 
                string.Join(", ", maliciousAgents));
            
            // In a real implementation, this would trigger:
            // 1. Immediate quarantine of malicious agents
            // 2. Security alert to system administrators
            // 3. Forensic analysis initiation
            // 4. Network partition isolation if necessary
            // 5. Consensus protocol adjustments
            
            foreach (var agentId in maliciousAgents)
            {
                var reputationUpdate = new ReputationUpdate
                {
                    AgentId = agentId,
                    UpdatedBy = "ByzantineDetector",
                    ScoreChange = -0.5, // Severe reputation penalty
                    Reason = "Identified as malicious agent in Byzantine attack",
                    Evidence = new Dictionary<string, object>
                    {
                        ["AttackDetectionTime"] = DateTime.UtcNow,
                        ["AttackId"] = Guid.NewGuid().ToString(),
                        ["MaliciousAgentCount"] = maliciousAgents.Count
                    }
                };
                
                await _reputationSystem.UpdateReputationAsync(agentId, reputationUpdate);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error notifying Byzantine attack");
        }
    }

    private void RecordMessage(AgentMessage message)
    {
        lock (_lockObject)
        {
            var agentId = message.SenderId;
            
            if (!_messageHistory.ContainsKey(agentId))
            {
                _messageHistory[agentId] = new List<AgentMessage>();
            }
            
            _messageHistory[agentId].Add(message);
            _lastActivityTime[agentId] = DateTime.UtcNow;
            
            // Keep only recent messages (last 100 per agent)
            if (_messageHistory[agentId].Count > 100)
            {
                _messageHistory[agentId].RemoveAt(0);
            }
        }
    }

    private async Task<bool> DetectFloodingAttackAsync(string agentId)
    {
        lock (_lockObject)
        {
            if (!_messageHistory.TryGetValue(agentId, out var messages))
                return false;
            
            // Check for too many messages in a short time period
            var recentMessages = messages.Where(m => 
                DateTime.UtcNow - m.Timestamp < TimeSpan.FromMinutes(1)).Count();
            
            return recentMessages > 20; // More than 20 messages per minute
        }
    }

    private async Task<bool> DetectContradictoryBehaviorAsync(string agentId)
    {
        lock (_lockObject)
        {
            if (!_messageHistory.TryGetValue(agentId, out var messages) || messages.Count < 3)
                return false;
            
            // Simplified contradiction detection
            // In a real implementation, this would use NLP to analyze content semantics
            var recentMessages = messages.TakeLast(5).ToList();
            
            // Check for conflicting votes or responses
            var consensusVotes = recentMessages
                .Where(m => m.MessageType == "ConsensusVote")
                .ToList();
            
            if (consensusVotes.Count >= 2)
            {
                // Check if agent voted differently on similar proposals
                // This is a simplified check - real implementation would be more sophisticated
                var voteTimes = consensusVotes.Select(m => m.Timestamp).ToList();
                var timeSpan = voteTimes.Max() - voteTimes.Min();
                
                if (timeSpan < TimeSpan.FromMinutes(5))
                {
                    // Multiple votes in short time could indicate manipulation
                    return true;
                }
            }
            
            return false;
        }
    }

    private async Task<bool> DetectTimingAnomaliesAsync(AgentMessage message)
    {
        // Check for impossible response times
        if (message.Response != null && message.Response.ExecutionTime < TimeSpan.FromMilliseconds(10))
        {
            return true; // Impossibly fast response
        }
        
        // Check for clock synchronization issues
        var timeDrift = Math.Abs((DateTime.UtcNow - message.Timestamp).TotalMinutes);
        if (timeDrift > 10) // Message timestamp is off by more than 10 minutes
        {
            return true;
        }
        
        lock (_lockObject)
        {
            if (_lastActivityTime.TryGetValue(message.SenderId, out var lastActivity))
            {
                var timeSinceLastActivity = message.Timestamp - lastActivity;
                
                // Detect impossible rapid-fire messages
                if (timeSinceLastActivity < TimeSpan.FromMilliseconds(100))
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    private async Task<bool> DetectContentAnomaliesAsync(AgentMessage message)
    {
        // Check for obviously malformed content
        if (string.IsNullOrWhiteSpace(message.Content) && message.Task == null && message.Response == null)
        {
            return true; // Empty message
        }
        
        // Check for suspiciously long content (potential attack)
        if (message.Content?.Length > 100000) // 100KB limit
        {
            return true;
        }
        
        // Check for repeated identical messages (spam detection)
        lock (_lockObject)
        {
            if (_messageHistory.TryGetValue(message.SenderId, out var messages))
            {
                var identicalMessages = messages
                    .Where(m => m.Content == message.Content && 
                               DateTime.UtcNow - m.Timestamp < TimeSpan.FromMinutes(5))
                    .Count();
                
                if (identicalMessages > 3) // More than 3 identical messages in 5 minutes
                {
                    return true;
                }
            }
        }
        
        return false;
    }

    private async Task<bool> DetectReputationManipulationAsync(AgentMessage message)
    {
        // Check if message contains reputation-related content that might indicate manipulation
        if (message.MessageType == "ReputationUpdate" || 
            message.Content?.ToLowerInvariant().Contains("reputation") == true)
        {
            // Analyze if agent is trying to manipulate reputation unfairly
            // This is a simplified check
            var agentReputation = await _reputationSystem.GetReputationAsync(message.SenderId);
            
            if (agentReputation < 0.3) // Low reputation agents shouldn't influence others' reputation significantly
            {
                return true;
            }
        }
        
        return false;
    }

    private async Task UpdateBehaviorPatternAsync(string agentId, double change)
    {
        lock (_lockObject)
        {
            if (!_behaviorPatterns.ContainsKey(agentId))
            {
                _behaviorPatterns[agentId] = 0.0;
            }
            
            _behaviorPatterns[agentId] = Math.Max(0, Math.Min(1, _behaviorPatterns[agentId] + change));
        }
    }

    private async Task<double> CalculateMessageFrequencyRiskAsync(string agentId)
    {
        lock (_lockObject)
        {
            if (!_messageHistory.TryGetValue(agentId, out var messages))
                return 0;
            
            var recentMessages = messages.Where(m => 
                DateTime.UtcNow - m.Timestamp < TimeSpan.FromHours(1)).Count();
            
            // Risk increases with message frequency
            // Normal: 0-10 messages/hour = low risk
            // Moderate: 11-30 messages/hour = medium risk  
            // High: 31+ messages/hour = high risk
            
            if (recentMessages <= 10) return 0.0;
            if (recentMessages <= 30) return 0.3;
            return 0.8;
        }
    }

    private async Task<double> CalculateConsistencyRiskAsync(string agentId)
    {
        lock (_lockObject)
        {
            if (!_messageHistory.TryGetValue(agentId, out var messages) || messages.Count < 5)
                return 0;
            
            // Analyze response time consistency
            var responseTimes = messages
                .Where(m => m.Response != null)
                .Select(m => m.Response!.ExecutionTime.TotalSeconds)
                .ToList();
            
            if (responseTimes.Count < 3)
                return 0;
            
            // Calculate variance in response times
            var mean = responseTimes.Average();
            var variance = responseTimes.Select(x => Math.Pow(x - mean, 2)).Average();
            var standardDeviation = Math.Sqrt(variance);
            
            // High variance in response times might indicate suspicious behavior
            var normalizedVariance = Math.Min(1.0, standardDeviation / 10.0);
            
            return normalizedVariance;
        }
    }
}
