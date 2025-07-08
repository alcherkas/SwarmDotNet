using System.Collections.Concurrent;
using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Models;
using Microsoft.Extensions.Logging;

namespace HiveMindSwarmDotnet.Console.Optimization;

public class OptimizationEngine : IOptimizationEngine
{
    private readonly ILogger<OptimizationEngine> _logger;
    private readonly ConcurrentDictionary<string, Dictionary<string, object>> _agentPerformanceHistory = new();
    private readonly ConcurrentDictionary<string, double> _agentFitnessScores = new();
    private readonly Random _random = new();

    public OptimizationEngine(ILogger<OptimizationEngine> logger)
    {
        _logger = logger;
    }

    public async Task<List<CommunicationLink>> OptimizeCommunicationLinksAsync(
        List<CommunicationLink> links, 
        SwarmMetrics metrics, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Optimizing {LinkCount} communication links", links.Count);

        var optimizedLinks = new List<CommunicationLink>();
        
        foreach (var link in links)
        {
            var optimizedLink = await OptimizeLinkAsync(link, metrics, cancellationToken);
            optimizedLinks.Add(optimizedLink);
        }

        await PruneWeakLinksAsync(optimizedLinks, cancellationToken);
        await CreateBeneficialLinksAsync(optimizedLinks, metrics, cancellationToken);

        _logger.LogInformation("Communication optimization completed. {OptimizedCount} links optimized", 
            optimizedLinks.Count);

        return optimizedLinks;
    }

    public async Task<Dictionary<AgentRole, double>> CalculateAgentPerformanceAsync(
        List<AgentResponse> responses, 
        CancellationToken cancellationToken = default)
    {
        var rolePerformance = new Dictionary<AgentRole, double>();
        
        foreach (var role in Enum.GetValues<AgentRole>())
        {
            var roleResponses = responses.Where(r => r.Role == role).ToList();
            if (!roleResponses.Any())
            {
                rolePerformance[role] = 0.0;
                continue;
            }

            var avgConfidence = roleResponses.Average(r => r.Confidence);
            var validResponseRatio = roleResponses.Count(r => r.IsValid) / (double)roleResponses.Count;
            var avgResponseTime = roleResponses.Average(r => 
                r.Metadata.TryGetValue("processing_time", out var time) ? Convert.ToDouble(time) : 1000.0);

            var timeScore = Math.Max(0, 1.0 - (avgResponseTime / 10000.0));
            var performance = (avgConfidence * 0.5) + (validResponseRatio * 0.3) + (timeScore * 0.2);
            
            rolePerformance[role] = Math.Max(0.0, Math.Min(1.0, performance));
        }

        await UpdatePerformanceHistoryAsync(rolePerformance, cancellationToken);
        
        return rolePerformance;
    }

    public async Task AdaptAgentBehaviorAsync(
        string agentId, 
        Dictionary<string, object> performanceData, 
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Adapting behavior for agent {AgentId}", agentId);

        var previousPerformance = _agentPerformanceHistory.GetOrAdd(agentId, _ => new Dictionary<string, object>());
        
        foreach (var (key, value) in performanceData)
        {
            previousPerformance[key] = value;
        }

        var currentFitness = CalculateAgentFitness(agentId, performanceData);
        var previousFitness = _agentFitnessScores.GetValueOrDefault(agentId, 0.5);

        if (currentFitness > previousFitness)
        {
            await ApplyPositiveReinforcementAsync(agentId, performanceData, cancellationToken);
        }
        else
        {
            await ApplyAdaptiveChangesAsync(agentId, performanceData, cancellationToken);
        }

        _agentFitnessScores[agentId] = currentFitness;
        
        await Task.CompletedTask;
    }

    private async Task<CommunicationLink> OptimizeLinkAsync(
        CommunicationLink link, 
        SwarmMetrics metrics, 
        CancellationToken cancellationToken)
    {
        var optimizedLink = new CommunicationLink
        {
            FromAgentId = link.FromAgentId,
            ToAgentId = link.ToAgentId,
            Weight = link.Weight,
            Probability = link.Probability,
            MessageCount = link.MessageCount,
            LastUsed = link.LastUsed
        };

        var timeSinceLastUse = DateTime.UtcNow - link.LastUsed;
        var usageFrequency = link.MessageCount / Math.Max(1, timeSinceLastUse.TotalHours);
        
        if (usageFrequency > 5.0)
        {
            optimizedLink.Weight = Math.Min(10.0, link.Weight * 1.1);
            optimizedLink.Probability = Math.Min(1.0, link.Probability * 1.05);
        }
        else if (usageFrequency < 0.5)
        {
            optimizedLink.Weight = Math.Max(0.1, link.Weight * 0.9);
            optimizedLink.Probability = Math.Max(0.1, link.Probability * 0.95);
        }

        if (link.MessageCount > 50)
        {
            optimizedLink.Weight = Math.Min(10.0, link.Weight * 1.2);
        }

        return await Task.FromResult(optimizedLink);
    }

    private async Task PruneWeakLinksAsync(List<CommunicationLink> links, CancellationToken cancellationToken)
    {
        var weakLinks = links.Where(l => l.Weight < 0.3 && l.MessageCount < 5).ToList();
        
        foreach (var weakLink in weakLinks)
        {
            links.Remove(weakLink);
        }

        if (weakLinks.Any())
        {
            _logger.LogDebug("Pruned {WeakLinkCount} weak communication links", weakLinks.Count);
        }

        await Task.CompletedTask;
    }

    private async Task CreateBeneficialLinksAsync(
        List<CommunicationLink> links, 
        SwarmMetrics metrics, 
        CancellationToken cancellationToken)
    {
        var agentIds = links.Select(l => l.FromAgentId).Concat(links.Select(l => l.ToAgentId)).Distinct().ToList();
        var newLinksCreated = 0;

        foreach (var fromAgent in agentIds)
        {
            foreach (var toAgent in agentIds)
            {
                if (fromAgent == toAgent)
                    continue;

                var existingLink = links.FirstOrDefault(l => l.FromAgentId == fromAgent && l.ToAgentId == toAgent);
                if (existingLink != null)
                    continue;

                var shouldCreateLink = ShouldCreateLink(fromAgent, toAgent, metrics);
                if (shouldCreateLink)
                {
                    var newLink = new CommunicationLink
                    {
                        FromAgentId = fromAgent,
                        ToAgentId = toAgent,
                        Weight = 0.5,
                        Probability = 0.4,
                        MessageCount = 0,
                        LastUsed = DateTime.UtcNow
                    };

                    links.Add(newLink);
                    newLinksCreated++;
                }
            }
        }

        if (newLinksCreated > 0)
        {
            _logger.LogDebug("Created {NewLinkCount} beneficial communication links", newLinksCreated);
        }

        await Task.CompletedTask;
    }

    private bool ShouldCreateLink(string fromAgent, string toAgent, SwarmMetrics metrics)
    {
        var random = _random.NextDouble();
        
        if (metrics.AverageConfidence < 0.6)
        {
            return random < 0.3;
        }

        if (metrics.TotalCommunications / Math.Max(1, metrics.CompletedTasks) < 2.0)
        {
            return random < 0.4;
        }

        return random < 0.2;
    }

    private double CalculateAgentFitness(string agentId, Dictionary<string, object> performanceData)
    {
        var fitness = 0.5;
        
        if (performanceData.TryGetValue("low_confidence", out var lowConfidenceObj) && 
            lowConfidenceObj is double lowConfidence)
        {
            fitness -= (1.0 - lowConfidence) * 0.3;
        }

        if (performanceData.TryGetValue("validation_errors", out var errorsObj) && 
            errorsObj is int errorCount)
        {
            fitness -= Math.Min(0.3, errorCount * 0.1);
        }

        if (performanceData.TryGetValue("task_complexity", out var complexityObj) && 
            complexityObj is double complexity)
        {
            var complexityScore = Math.Min(1.0, complexity / 1000.0);
            fitness += complexityScore * 0.2;
        }

        return Math.Max(0.0, Math.Min(1.0, fitness));
    }

    private async Task ApplyPositiveReinforcementAsync(
        string agentId, 
        Dictionary<string, object> performanceData, 
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Applying positive reinforcement to agent {AgentId}", agentId);
        
        var reinforcementData = new Dictionary<string, object>
        {
            ["reinforcement_type"] = "positive",
            ["timestamp"] = DateTime.UtcNow,
            ["performance_improvement"] = true
        };

        foreach (var (key, value) in performanceData)
        {
            reinforcementData[$"reinforced_{key}"] = value;
        }

        await Task.CompletedTask;
    }

    private async Task ApplyAdaptiveChangesAsync(
        string agentId, 
        Dictionary<string, object> performanceData, 
        CancellationToken cancellationToken)
    {
        _logger.LogDebug("Applying adaptive changes to agent {AgentId}", agentId);
        
        var adaptationStrategies = new List<string>();

        if (performanceData.TryGetValue("low_confidence", out var lowConfidenceObj) && 
            lowConfidenceObj is double lowConfidence && lowConfidence < 0.5)
        {
            adaptationStrategies.Add("increase_temperature");
            adaptationStrategies.Add("provide_more_context");
        }

        if (performanceData.TryGetValue("validation_errors", out var errorsObj) && 
            errorsObj is int errorCount && errorCount > 2)
        {
            adaptationStrategies.Add("enhance_validation");
            adaptationStrategies.Add("reduce_response_length");
        }

        foreach (var strategy in adaptationStrategies)
        {
            _logger.LogDebug("Applying adaptation strategy '{Strategy}' to agent {AgentId}", strategy, agentId);
        }

        await Task.CompletedTask;
    }

    private async Task UpdatePerformanceHistoryAsync(
        Dictionary<AgentRole, double> rolePerformance, 
        CancellationToken cancellationToken)
    {
        foreach (var (role, performance) in rolePerformance)
        {
            var roleKey = $"role_{role}";
            var history = _agentPerformanceHistory.GetOrAdd(roleKey, _ => new Dictionary<string, object>());
            
            history["last_performance"] = performance;
            history["last_updated"] = DateTime.UtcNow;
            
            if (history.TryGetValue("performance_history", out var historyObj) && 
                historyObj is List<double> performanceHistory)
            {
                performanceHistory.Add(performance);
                if (performanceHistory.Count > 100)
                {
                    performanceHistory.RemoveAt(0);
                }
            }
            else
            {
                history["performance_history"] = new List<double> { performance };
            }
        }

        await Task.CompletedTask;
    }
}