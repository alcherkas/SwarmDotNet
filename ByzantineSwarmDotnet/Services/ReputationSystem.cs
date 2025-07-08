using Microsoft.Extensions.Logging;
using ByzantineSwarmDotnet.Models;
using ByzantineSwarmDotnet.Interfaces;

namespace ByzantineSwarmDotnet.Services;

public class ReputationSystem : IReputationSystem
{
    private readonly ILogger<ReputationSystem> _logger;
    private readonly IDistributedReputationStore _store;
    private readonly Dictionary<string, double> _reputationCache;
    private readonly Dictionary<string, List<ReputationUpdate>> _reputationHistory;
    private readonly object _lockObject = new();
    private readonly double _minTrustThreshold;
    private readonly double _maliciousThreshold;
    private readonly double _decayRate;

    public ReputationSystem(
        ILogger<ReputationSystem> logger,
        IDistributedReputationStore store,
        double minTrustThreshold = 0.3,
        double maliciousThreshold = 0.2,
        double decayRate = 0.99)
    {
        _logger = logger;
        _store = store;
        _reputationCache = new Dictionary<string, double>();
        _reputationHistory = new Dictionary<string, List<ReputationUpdate>>();
        _minTrustThreshold = minTrustThreshold;
        _maliciousThreshold = maliciousThreshold;
        _decayRate = decayRate;
        
        // Start reputation decay timer
        _ = Task.Run(async () => await StartReputationDecayLoopAsync());
    }

    public async Task<double> GetReputationAsync(string agentId)
    {
        lock (_lockObject)
        {
            if (_reputationCache.TryGetValue(agentId, out var cachedReputation))
            {
                return cachedReputation;
            }
        }

        try
        {
            var reputation = await _store.GetReputationAsync(agentId);
            
            lock (_lockObject)
            {
                _reputationCache[agentId] = reputation;
            }
            
            return reputation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reputation for agent {AgentId}", agentId);
            return 0.5; // Default neutral reputation
        }
    }

    public async Task<bool> UpdateReputationAsync(string agentId, ReputationUpdate update)
    {
        try
        {
            // Validate the update
            if (!await ValidateReputationUpdateAsync(update))
            {
                _logger.LogWarning("Invalid reputation update for agent {AgentId}", agentId);
                return false;
            }

            var currentReputation = await GetReputationAsync(agentId);
            var newReputation = CalculateNewReputation(currentReputation, update);
            
            // Apply bounds (0 to 1)
            newReputation = Math.Max(0, Math.Min(1, newReputation));

            // Update cache and store
            lock (_lockObject)
            {
                _reputationCache[agentId] = newReputation;
                
                if (!_reputationHistory.ContainsKey(agentId))
                {
                    _reputationHistory[agentId] = new List<ReputationUpdate>();
                }
                _reputationHistory[agentId].Add(update);
                
                // Keep only recent history (last 100 updates)
                if (_reputationHistory[agentId].Count > 100)
                {
                    _reputationHistory[agentId].RemoveAt(0);
                }
            }

            await _store.UpdateReputationAsync(update);
            
            _logger.LogInformation("Updated reputation for agent {AgentId}: {OldScore:F3} -> {NewScore:F3} (Change: {Change:F3})",
                agentId, currentReputation, newReputation, update.ScoreChange);
            
            // Check if agent became malicious
            if (newReputation <= _maliciousThreshold)
            {
                _logger.LogWarning("Agent {AgentId} reputation dropped to malicious level: {Reputation:F3}", 
                    agentId, newReputation);
                await NotifyMaliciousAgentAsync(agentId, newReputation);
            }
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating reputation for agent {AgentId}", agentId);
            return false;
        }
    }

    public async Task<TrustScore> CalculateTrustScoreAsync(string agentId)
    {
        try
        {
            var reputation = await GetReputationAsync(agentId);
            var trustFactors = new List<string>();
            
            // Calculate base trust from reputation
            var baseTrust = reputation;
            
            // Adjust for reputation volatility
            var volatilityAdjustment = await CalculateVolatilityAdjustmentAsync(agentId);
            var adjustedTrust = baseTrust * (1 - volatilityAdjustment);
            
            trustFactors.Add($"Base reputation: {baseTrust:F3}");
            
            if (volatilityAdjustment > 0)
            {
                trustFactors.Add($"Volatility adjustment: -{volatilityAdjustment:F3}");
            }
            
            // Consider recent behavior trends
            var trendAdjustment = await CalculateTrendAdjustmentAsync(agentId);
            adjustedTrust += trendAdjustment;
            
            if (Math.Abs(trendAdjustment) > 0.01)
            {
                trustFactors.Add($"Trend adjustment: {trendAdjustment:+F3;-F3}");
            }
            
            // Final bounds
            adjustedTrust = Math.Max(0, Math.Min(1, adjustedTrust));
            
            var isTrusted = adjustedTrust >= _minTrustThreshold;
            
            return new TrustScore
            {
                AgentId = agentId,
                Score = adjustedTrust,
                IsTrusted = isTrusted,
                LastUpdated = DateTime.UtcNow,
                TrustFactors = trustFactors
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating trust score for agent {AgentId}", agentId);
            return new TrustScore
            {
                AgentId = agentId,
                Score = 0,
                IsTrusted = false,
                LastUpdated = DateTime.UtcNow,
                TrustFactors = new List<string> { "Error in calculation" }
            };
        }
    }

    public async Task<List<string>> GetTrustedAgentsAsync(double minThreshold)
    {
        try
        {
            var allReputations = await _store.GetAllReputationsAsync();
            
            return allReputations
                .Where(kvp => kvp.Value >= minThreshold)
                .Select(kvp => kvp.Key)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting trusted agents");
            return new List<string>();
        }
    }

    public async Task<List<string>> GetSuspiciousAgentsAsync()
    {
        try
        {
            var allReputations = await _store.GetAllReputationsAsync();
            
            return allReputations
                .Where(kvp => kvp.Value <= _maliciousThreshold)
                .Select(kvp => kvp.Key)
                .ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting suspicious agents");
            return new List<string>();
        }
    }

    public async Task<bool> ValidateReputationUpdateAsync(ReputationUpdate update)
    {
        try
        {
            // Basic validation
            if (string.IsNullOrEmpty(update.AgentId) || string.IsNullOrEmpty(update.UpdatedBy))
            {
                return false;
            }

            // Validate score change bounds
            if (Math.Abs(update.ScoreChange) > 0.5)
            {
                _logger.LogWarning("Reputation change too large: {Change}", update.ScoreChange);
                return false;
            }

            // Validate timestamp
            if (update.UpdatedAt > DateTime.UtcNow.AddMinutes(1) || 
                update.UpdatedAt < DateTime.UtcNow.AddDays(-1))
            {
                return false;
            }

            // Check if updater is trusted (prevent malicious reputation manipulation)
            var updaterTrust = await CalculateTrustScoreAsync(update.UpdatedBy);
            if (!updaterTrust.IsTrusted)
            {
                _logger.LogWarning("Reputation update from untrusted agent {UpdatedBy}", update.UpdatedBy);
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating reputation update");
            return false;
        }
    }

    private double CalculateNewReputation(double currentReputation, ReputationUpdate update)
    {
        // Apply weighted update based on evidence quality
        var evidenceWeight = CalculateEvidenceWeight(update);
        var weightedChange = update.ScoreChange * evidenceWeight;
        
        // Use exponential moving average for smoother updates
        var alpha = 0.1; // Learning rate
        return currentReputation + (alpha * weightedChange);
    }

    private double CalculateEvidenceWeight(ReputationUpdate update)
    {
        var weight = 1.0;
        
        // More evidence increases weight
        if (update.Evidence.Count > 3)
        {
            weight *= 1.2;
        }
        
        // Recent updates have higher weight
        var ageInHours = (DateTime.UtcNow - update.UpdatedAt).TotalHours;
        if (ageInHours < 1)
        {
            weight *= 1.1;
        }
        
        return Math.Min(1.5, weight); // Cap at 1.5x
    }

    private async Task<double> CalculateVolatilityAdjustmentAsync(string agentId)
    {
        lock (_lockObject)
        {
            if (!_reputationHistory.TryGetValue(agentId, out var history) || history.Count < 5)
            {
                return 0; // Not enough history
            }
            
            // Calculate variance of recent reputation changes
            var recentChanges = history.TakeLast(10).Select(h => h.ScoreChange).ToList();
            var mean = recentChanges.Average();
            var variance = recentChanges.Select(x => Math.Pow(x - mean, 2)).Average();
            var standardDeviation = Math.Sqrt(variance);
            
            // High volatility reduces trust
            return Math.Min(0.3, standardDeviation * 2);
        }
    }

    private async Task<double> CalculateTrendAdjustmentAsync(string agentId)
    {
        lock (_lockObject)
        {
            if (!_reputationHistory.TryGetValue(agentId, out var history) || history.Count < 3)
            {
                return 0;
            }
            
            // Calculate trend from recent updates
            var recentUpdates = history.TakeLast(5).ToList();
            var totalChange = recentUpdates.Sum(u => u.ScoreChange);
            
            // Positive trend slightly increases trust, negative trend decreases it
            return Math.Max(-0.1, Math.Min(0.1, totalChange * 0.1));
        }
    }

    private async Task NotifyMaliciousAgentAsync(string agentId, double reputation)
    {
        // This would trigger security protocols in a real implementation
        _logger.LogCritical("SECURITY ALERT: Agent {AgentId} marked as potentially malicious with reputation {Reputation:F3}", 
            agentId, reputation);
    }

    private async Task StartReputationDecayLoopAsync()
    {
        while (true)
        {
            try
            {
                await Task.Delay(TimeSpan.FromHours(1)); // Decay every hour
                await ApplyReputationDecayAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in reputation decay loop");
            }
        }
    }

    private async Task ApplyReputationDecayAsync()
    {
        try
        {
            var allReputations = await _store.GetAllReputationsAsync();
            
            foreach (var kvp in allReputations)
            {
                var agentId = kvp.Key;
                var currentReputation = kvp.Value;
                
                // Apply decay towards neutral (0.5)
                var neutralTendency = 0.5;
                var decayedReputation = currentReputation * _decayRate + neutralTendency * (1 - _decayRate);
                
                if (Math.Abs(decayedReputation - currentReputation) > 0.001)
                {
                    var decayUpdate = new ReputationUpdate
                    {
                        AgentId = agentId,
                        UpdatedBy = "ReputationSystem",
                        ScoreChange = decayedReputation - currentReputation,
                        Reason = "Automatic reputation decay",
                        UpdatedAt = DateTime.UtcNow
                    };
                    
                    await UpdateReputationAsync(agentId, decayUpdate);
                }
            }
            
            _logger.LogDebug("Applied reputation decay to {Count} agents", allReputations.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error applying reputation decay");
        }
    }
}
