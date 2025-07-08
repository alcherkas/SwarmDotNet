using Microsoft.SemanticKernel;
using Microsoft.Extensions.Logging;
using ByzantineSwarmDotnet.Models;
using ByzantineSwarmDotnet.Interfaces;

namespace ByzantineSwarmDotnet.Agents;

public class ValidatorAgent : EnhancedSwarmAgentBase, IValidatorAgent
{
    private readonly Dictionary<string, double> _validationCache;
    private readonly List<string> _suspiciousAgents;

    public ValidatorAgent(
        string id,
        Kernel kernel,
        ILogger<ValidatorAgent> logger,
        ICryptographicSigner signer,
        IByzantineDetector byzantineDetector,
        IReputationSystem reputationSystem,
        IFaultTolerantCommunicationHub communicationHub) 
        : base(id, AgentRole.Validator, AgentHierarchyLevel.Worker, kernel, logger, signer, byzantineDetector, reputationSystem, communicationHub)
    {
        _validationCache = new Dictionary<string, double>();
        _suspiciousAgents = new List<string>();
    }

    public override async Task<AgentResponse> ProcessTaskAsync(AgentTask task)
    {
        try
        {
            _logger.LogInformation("ValidatorAgent {AgentId} processing validation task {TaskId}", Id, task.Id);
            
            var startTime = DateTime.UtcNow;
            
            // Handle different types of validation tasks
            var result = task.Type switch
            {
                TaskType.SecurityValidation => await PerformSecurityValidationAsync(task),
                TaskType.ConsensusParticipation => await ParticipateInConsensusAsync(task),
                _ => await PerformGeneralValidationAsync(task)
            };
            
            var executionTime = DateTime.UtcNow - startTime;
            UpdateMetric("LastExecutionTime", executionTime);
            UpdateMetric("ValidationsPerformed", (int)(GetMetricsAsync().Result.GetValueOrDefault("ValidationsPerformed", 0)) + 1);
            
            return new AgentResponse
            {
                TaskId = task.Id,
                AgentId = Id,
                Success = true,
                Result = result,
                ExecutionTime = executionTime,
                ConfidenceScore = await CalculateValidationConfidenceAsync(task, result),
                Timestamp = DateTime.UtcNow,
                Metadata = new Dictionary<string, object>
                {
                    ["ValidationType"] = task.Type.ToString(),
                    ["ValidationMethod"] = "ByzantineAware"
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing validation task {TaskId}", task.Id);
            return new AgentResponse
            {
                TaskId = task.Id,
                AgentId = Id,
                Success = false,
                ErrorMessage = ex.Message,
                Timestamp = DateTime.UtcNow
            };
        }
    }

    public async Task<bool> ValidateAgentResponseAsync(AgentResponse response)
    {
        try
        {
            // Check if agent is in suspicious list
            if (_suspiciousAgents.Contains(response.AgentId))
            {
                _logger.LogWarning("Validating response from suspicious agent {AgentId}", response.AgentId);
            }

            // Basic response validation
            if (!await ValidateResponseAsync(response))
            {
                await RecordSuspiciousActivityAsync(response.AgentId, "Invalid response format");
                return false;
            }

            // Content validation using AI
            var contentValidation = await ValidateResponseContentAsync(response);
            if (!contentValidation.IsValid)
            {
                await RecordSuspiciousActivityAsync(response.AgentId, "Invalid response content");
                return false;
            }

            // Check for Byzantine patterns
            var byzantineCheck = await CheckForByzantinePatternAsync(response);
            if (byzantineCheck)
            {
                await RecordSuspiciousActivityAsync(response.AgentId, "Byzantine behavior pattern detected");
                return false;
            }

            // Update validation cache
            _validationCache[response.Id] = contentValidation.Confidence;
            
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating agent response {ResponseId}", response.Id);
            return false;
        }
    }

    public async Task<bool> ValidateConsensusProposalAsync(Proposal proposal)
    {
        try
        {
            // Validate proposal structure
            if (string.IsNullOrEmpty(proposal.Id) || proposal.ExpiresAt < DateTime.UtcNow)
            {
                _logger.LogWarning("Invalid proposal structure: {ProposalId}", proposal.Id);
                return false;
            }

            // Check proposer reputation
            var proposerTrust = await CalculateTrustScoreAsync(proposal.ProposerId);
            if (!proposerTrust.IsTrusted)
            {
                _logger.LogWarning("Proposal from untrusted agent {ProposerId}", proposal.ProposerId);
                return false;
            }

            // Validate proposal content
            var contentValidation = await ValidateProposalContentAsync(proposal);
            
            return contentValidation;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating consensus proposal {ProposalId}", proposal.Id);
            return false;
        }
    }

    public async Task<double> CalculateResponseConfidenceAsync(AgentResponse response)
    {
        try
        {
            var factors = new List<double>();

            // Agent reputation factor
            var agentTrust = await CalculateTrustScoreAsync(response.AgentId);
            factors.Add(agentTrust.Score);

            // Response coherence factor
            var coherenceScore = await AnalyzeResponseCoherenceAsync(response.Result);
            factors.Add(coherenceScore);

            // Execution time factor (faster might be less thorough)
            var timeScore = CalculateTimeScore(response.ExecutionTime);
            factors.Add(timeScore);

            // Historical accuracy factor
            var historicalScore = await GetHistoricalAccuracyAsync(response.AgentId);
            factors.Add(historicalScore);

            // Calculate weighted average
            var confidence = factors.Average();
            
            _logger.LogDebug("Calculated confidence {Confidence:F3} for response {ResponseId}", confidence, response.Id);
            
            return Math.Max(0, Math.Min(1, confidence));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating response confidence for {ResponseId}", response.Id);
            return 0.5; // Default neutral confidence
        }
    }

    protected override async Task<bool> DetectByzantineBehaviorAsync(AgentMessage message)
    {
        var baseByzantine = await base.DetectByzantineBehaviorAsync(message);
        
        if (baseByzantine)
            return true;

        // Additional validator-specific Byzantine detection
        try
        {
            // Check for conflicting messages from same agent
            var conflictingMessages = await CheckForConflictingMessagesAsync(message.SenderId);
            if (conflictingMessages)
            {
                _logger.LogWarning("Conflicting messages detected from agent {AgentId}", message.SenderId);
                return true;
            }

            // Check for impossible response times
            var impossibleTiming = await CheckForImpossibleTimingAsync(message);
            if (impossibleTiming)
            {
                _logger.LogWarning("Impossible timing detected from agent {AgentId}", message.SenderId);
                return true;
            }

            // Check for pattern anomalies
            var patternAnomaly = await DetectPatternAnomalyAsync(message);
            if (patternAnomaly)
            {
                _logger.LogWarning("Pattern anomaly detected from agent {AgentId}", message.SenderId);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in enhanced Byzantine detection for agent {AgentId}", message.SenderId);
            return true; // Assume Byzantine on detection error
        }
    }

    private async Task<string> PerformSecurityValidationAsync(AgentTask task)
    {
        var validationPrompt = $@"
You are a security validator agent. Analyze the following for security vulnerabilities:

Task: {task.Description}
Parameters: {string.Join(", ", task.Parameters.Select(p => $"{p.Key}: {p.Value}"))}

Please perform a security analysis and identify:
1. Potential security vulnerabilities
2. Authentication and authorization concerns
3. Data integrity risks
4. Recommended security measures

Provide a structured security assessment.
";

        var result = await _kernel.InvokePromptAsync(validationPrompt);
        return result.ToString();
    }

    private async Task<string> ParticipateInConsensusAsync(AgentTask task)
    {
        // Extract proposal from task parameters
        if (task.Parameters.TryGetValue("proposal", out var proposalObj) && proposalObj is Proposal proposal)
        {
            var isValid = await ValidateConsensusProposalAsync(proposal);
            var vote = await VoteOnProposalAsync(proposal);
            
            return $"Consensus participation: Vote={vote.Accept}, Valid={isValid}, Justification={vote.Justification}";
        }
        
        return "No valid proposal found for consensus participation";
    }

    private async Task<string> PerformGeneralValidationAsync(AgentTask task)
    {
        var validationPrompt = $@"
You are a validation agent. Carefully analyze and validate the following:

Task: {task.Description}
Type: {task.Type}
Parameters: {string.Join(", ", task.Parameters.Select(p => $"{p.Key}: {p.Value}"))}

Please provide:
1. Validation of the task requirements
2. Assessment of parameter validity
3. Identification of any inconsistencies
4. Recommendations for improvement
5. Overall validation score (0-1)

Provide a comprehensive validation report.
";

        var result = await _kernel.InvokePromptAsync(validationPrompt);
        return result.ToString();
    }

    private async Task<(bool IsValid, double Confidence)> ValidateResponseContentAsync(AgentResponse response)
    {
        try
        {
            var validationPrompt = $@"
You are a content validator. Analyze the following response for quality and validity:

Response: {response.Result}
Agent: {response.AgentId}
Execution Time: {response.ExecutionTime.TotalSeconds} seconds
Confidence Score: {response.ConfidenceScore}

Evaluate:
1. Content coherence and logical consistency
2. Completeness of the response
3. Accuracy based on the context
4. Presence of any suspicious patterns
5. Overall quality assessment

Return a score between 0 and 1, where 1 is excellent and 0 is poor.
Also indicate if this is VALID or INVALID.
";

            var result = await _kernel.InvokePromptAsync(validationPrompt);
            var resultText = result.ToString().ToLowerInvariant();
            
            // Extract validation result and confidence
            var isValid = resultText.Contains("valid") && !resultText.Contains("invalid");
            var confidence = ExtractConfidenceScore(resultText);
            
            return (isValid, confidence);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating response content");
            return (false, 0);
        }
    }

    private async Task<bool> CheckForByzantinePatternAsync(AgentResponse response)
    {
        try
        {
            // Check if agent is providing contradictory responses
            var recentResponses = await GetRecentResponsesFromAgentAsync(response.AgentId);
            
            if (recentResponses.Count >= 3)
            {
                // Analyze pattern consistency
                var contradictions = await DetectContradictionsAsync(recentResponses);
                if (contradictions > 0.3) // More than 30% contradictory
                {
                    return true;
                }
            }

            // Check for impossibly fast responses
            if (response.ExecutionTime < TimeSpan.FromMilliseconds(100))
            {
                _logger.LogWarning("Suspiciously fast response from agent {AgentId}: {ExecutionTime}ms", 
                    response.AgentId, response.ExecutionTime.TotalMilliseconds);
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking Byzantine patterns");
            return false;
        }
    }

    private async Task RecordSuspiciousActivityAsync(string agentId, string reason)
    {
        if (!_suspiciousAgents.Contains(agentId))
        {
            _suspiciousAgents.Add(agentId);
        }

        // Record reputation update
        var reputationUpdate = new ReputationUpdate
        {
            AgentId = agentId,
            UpdatedBy = Id,
            ScoreChange = -0.1, // Decrease reputation
            Reason = reason,
            Evidence = new Dictionary<string, object>
            {
                ["DetectedBy"] = Id,
                ["DetectionTime"] = DateTime.UtcNow,
                ["SuspiciousActivity"] = reason
            }
        };

        await UpdateReputationAsync(reputationUpdate);
        
        _logger.LogWarning("Recorded suspicious activity for agent {AgentId}: {Reason}", agentId, reason);
    }

    private async Task<double> AnalyzeResponseCoherenceAsync(string responseText)
    {
        if (string.IsNullOrWhiteSpace(responseText))
            return 0;

        try
        {
            var coherencePrompt = $@"
Analyze the following text for coherence and logical consistency.
Rate the coherence on a scale of 0 to 1, where 1 is perfectly coherent and 0 is incoherent.

Text: {responseText}

Consider:
- Logical flow and structure
- Consistency of statements
- Clarity of expression
- Relevance to apparent context

Respond with just a number between 0 and 1.
";

            var result = await _kernel.InvokePromptAsync(coherencePrompt);
            var scoreText = result.ToString().Trim();
            
            if (double.TryParse(scoreText, out var score))
            {
                return Math.Max(0, Math.Min(1, score));
            }
            
            return 0.5; // Default if parsing fails
        }
        catch
        {
            return 0.5;
        }
    }

    private double CalculateTimeScore(TimeSpan executionTime)
    {
        // Optimal execution time is between 1-30 seconds
        var seconds = executionTime.TotalSeconds;
        
        if (seconds < 1) return 0.3; // Too fast, suspicious
        if (seconds <= 30) return 1.0; // Optimal range
        if (seconds <= 60) return 0.8; // Acceptable
        if (seconds <= 120) return 0.6; // Slow but acceptable
        
        return 0.3; // Too slow
    }

    private async Task<double> GetHistoricalAccuracyAsync(string agentId)
    {
        // Simplified implementation - would use actual historical data
        var reputation = await _reputationSystem.GetReputationAsync(agentId);
        return reputation;
    }

    private async Task<bool> CheckForConflictingMessagesAsync(string agentId)
    {
        // Simplified implementation
        return await Task.FromResult(false);
    }

    private async Task<bool> CheckForImpossibleTimingAsync(AgentMessage message)
    {
        // Check if message timing is impossible
        var timeSinceCreation = DateTime.UtcNow - message.Timestamp;
        return await Task.FromResult(Math.Abs(timeSinceCreation.TotalMinutes) > 10);
    }

    private async Task<bool> DetectPatternAnomalyAsync(AgentMessage message)
    {
        // Simplified pattern anomaly detection
        return await Task.FromResult(false);
    }

    private async Task<double> CalculateValidationConfidenceAsync(AgentTask task, string result)
    {
        // Calculate confidence based on validation factors
        var factors = new List<double>
        {
            string.IsNullOrEmpty(result) ? 0 : 0.8,
            task.Type == TaskType.SecurityValidation ? 0.9 : 0.7,
            0.85 // Base confidence for validator agent
        };
        
        return factors.Average();
    }

    private double ExtractConfidenceScore(string text)
    {
        // Simple regex to extract decimal numbers
        var match = System.Text.RegularExpressions.Regex.Match(text, @"0?\.\d+|1\.0+|[01]");
        if (match.Success && double.TryParse(match.Value, out var score))
        {
            return Math.Max(0, Math.Min(1, score));
        }
        return 0.5;
    }

    private async Task<List<AgentResponse>> GetRecentResponsesFromAgentAsync(string agentId)
    {
        // Simplified implementation - would query actual response history
        return new List<AgentResponse>();
    }

    private async Task<double> DetectContradictionsAsync(List<AgentResponse> responses)
    {
        // Simplified contradiction detection
        return await Task.FromResult(0.0);
    }
}
