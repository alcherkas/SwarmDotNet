using ByzantineSwarmDotnet.Examples.Models;

namespace ByzantineSwarmDotnet.Examples.SampleData;

/// <summary>
/// Sample pull request data for Byzantine swarm testing and demonstration purposes
/// Includes enhanced data for Byzantine fault tolerance testing
/// </summary>
public static class SamplePRData
{
    /// <summary>
    /// Sample bug fix PR data with Byzantine tolerance parameters
    /// </summary>
    public static SwarmTask CreateByzantineBugFixPR()
    {
        return new SwarmTask
        {
            Description = "Byzantine-tolerant analysis: Resolve memory leak in background service",
            RequiredRoles = new[]
            {
                "PRExtractor",
                "CodeAnalyzer",
                "RiskAssessment",
                "TestCoverage",
                "Validator",
                "SummaryGenerator"
            },
            RequiredConsensus = true,
            FaultToleranceLevel = FaultToleranceLevel.Byzantine,
            SecurityLevel = SecurityLevel.High,
            Parameters = new Dictionary<string, object>
            {
                ["pr_data"] = new
                {
                    id = 1234,
                    title = "Fix memory leak in BackgroundTaskService",
                    description = """
                        ## Problem
                        BackgroundTaskService was not properly disposing of HttpClient instances, 
                        causing memory leaks over time.

                        ## Solution
                        - Implemented proper disposal pattern
                        - Added using statements for HttpClient
                        - Updated service registration to use IHttpClientFactory

                        ## Testing
                        - Added memory leak tests
                        - Verified with dotMemory profiler
                        - Load tested for 24 hours

                        ## Byzantine Tolerance
                        - Requires consensus from multiple validators
                        - Cryptographic signatures for verification
                        - Reputation tracking for participating agents

                        Fixes: BUG-789
                        """,
                    author = "developer@company.com",
                    created_at = "2024-01-15T10:30:00Z",
                    updated_at = "2024-01-15T14:20:00Z",
                    status = "open",
                    labels = new[] { "bug", "performance", "backend", "byzantine-validated" },
                    linked_issues = new[] { "BUG-789" },
                    consensus_required = true,
                    min_validators = 3,
                    reputation_threshold = 0.7
                },
                ["files_changed"] = new[]
                {
                    new
                    {
                        filename = "src/Services/BackgroundTaskService.cs",
                        status = "modified",
                        additions = 15,
                        deletions = 8,
                        risk_level = "medium",
                        requires_consensus = true,
                        diff = """
                            @@ -45,8 +45,15 @@ public class BackgroundTaskService : BackgroundService
                             
                             private async Task ProcessTasksAsync(CancellationToken cancellationToken)
                             {
                            -    var httpClient = new HttpClient();
                            -    // Process tasks...
                            +    using var httpClient = _httpClientFactory.CreateClient();
                            +    try
                            +    {
                            +        // Process tasks...
                            +        await ProcessTaskBatchAsync(httpClient, cancellationToken);
                            +    }
                            +    catch (Exception ex)
                            +    {
                            +        _logger.LogError(ex, "Error processing tasks");
                            +    }
                             }
                            """
                    },
                    new
                    {
                        filename = "tests/Services/BackgroundTaskServiceTests.cs",
                        status = "added",
                        additions = 45,
                        deletions = 0,
                        risk_level = "low",
                        requires_consensus = false,
                        diff = """
                            +[Test]
                            +public async Task ProcessTasksAsync_ShouldNotLeakMemory()
                            +{
                            +    // Arrange
                            +    var mockFactory = new Mock<IHttpClientFactory>();
                            +    var service = new BackgroundTaskService(mockFactory.Object, _logger);
                            +    
                            +    // Act & Assert
                            +    var initialMemory = GC.GetTotalMemory(true);
                            +    
                            +    for (int i = 0; i < 100; i++)
                            +    {
                            +        await service.ProcessTasksAsync(CancellationToken.None);
                            +    }
                            +    
                            +    GC.Collect();
                            +    GC.WaitForPendingFinalizers();
                            +    var finalMemory = GC.GetTotalMemory(true);
                            +    
                            +    Assert.That(finalMemory - initialMemory, Is.LessThan(1024 * 1024)); // Less than 1MB growth
                            +}
                            """
                    }
                },
                ["byzantine_config"] = new
                {
                    fault_tolerance_level = "byzantine",
                    max_byzantine_faults = 1,
                    required_consensus_percentage = 0.67,
                    reputation_weighted_voting = true,
                    cryptographic_verification = true,
                    timeout_seconds = 300
                },
                ["validation_rules"] = new
                {
                    require_digital_signatures = true,
                    validate_agent_reputation = true,
                    detect_contradictory_responses = true,
                    minimum_trust_score = 0.6
                }
            }
        };
    }

    /// <summary>
    /// Sample new feature PR data with enhanced Byzantine tolerance features
    /// </summary>
    public static SwarmTask CreateByzantineFeaturePR()
    {
        return new SwarmTask
        {
            Description = "Byzantine-tolerant analysis: OAuth2 integration with advanced consensus",
            RequiredRoles = new[]
            {
                "Orchestrator",
                "PRExtractor",
                "JiraContext",
                "CodeAnalyzer",
                "RequirementMapper",
                "TestCoverage",
                "RiskAssessment",
                "IntegrationAnalyzer",
                "Validator",
                "SummaryGenerator"
            },
            RequiredConsensus = true,
            FaultToleranceLevel = FaultToleranceLevel.Byzantine,
            SecurityLevel = SecurityLevel.Critical,
            Parameters = new Dictionary<string, object>
            {
                ["pr_data"] = new
                {
                    id = 5678,
                    title = "Add OAuth2 integration with Google and Microsoft - Byzantine Validated",
                    description = """
                        ## Feature Overview
                        Implements OAuth2 authentication with Google and Microsoft providers with 
                        Byzantine fault tolerance for enhanced security and reliability.

                        ## Implementation Details
                        - Added OAuth2 service with provider abstraction
                        - Implemented Google and Microsoft OAuth2 providers
                        - Added user account linking functionality
                        - Updated authentication middleware
                        - Added OAuth2 configuration settings

                        ## Byzantine Enhancements
                        - Multi-agent consensus validation
                        - Cryptographic signature verification
                        - Reputation-based trust scoring
                        - Automatic Byzantine fault detection

                        ## Security Considerations
                        - PKCE flow implementation for enhanced security
                        - Secure token storage and refresh handling
                        - Rate limiting for OAuth endpoints
                        - Comprehensive input validation
                        - Byzantine attack prevention

                        Implements: FEAT-456, FEAT-457
                        Related: SEC-123 (security review)
                        Byzantine-Validated: YES
                        """,
                    author = "senior-dev@company.com",
                    created_at = "2024-01-20T09:15:00Z",
                    updated_at = "2024-01-22T16:45:00Z",
                    status = "ready_for_review",
                    labels = new[] { "feature", "authentication", "oauth2", "security", "byzantine-validated" },
                    linked_issues = new[] { "FEAT-456", "FEAT-457", "SEC-123" },
                    consensus_required = true,
                    security_critical = true,
                    min_validators = 5,
                    reputation_threshold = 0.8
                },
                ["files_changed"] = new[]
                {
                    new
                    {
                        filename = "src/Authentication/OAuth2/OAuth2Service.cs",
                        status = "added",
                        additions = 120,
                        deletions = 0,
                        risk_level = "high",
                        security_impact = "critical",
                        requires_consensus = true
                    },
                    new
                    {
                        filename = "src/Authentication/OAuth2/Providers/GoogleOAuth2Provider.cs",
                        status = "added",
                        additions = 85,
                        deletions = 0,
                        risk_level = "high",
                        security_impact = "high",
                        requires_consensus = true
                    },
                    new
                    {
                        filename = "src/Controllers/AuthController.cs",
                        status = "modified",
                        additions = 45,
                        deletions = 5,
                        risk_level = "medium",
                        security_impact = "medium",
                        requires_consensus = true
                    },
                    new
                    {
                        filename = "tests/Authentication/OAuth2ServiceTests.cs",
                        status = "added",
                        additions = 150,
                        deletions = 0,
                        risk_level = "low",
                        security_impact = "low",
                        requires_consensus = false
                    }
                },
                ["api_changes"] = new[]
                {
                    "POST /api/auth/oauth2/google/authorize",
                    "POST /api/auth/oauth2/google/callback", 
                    "POST /api/auth/oauth2/microsoft/authorize",
                    "POST /api/auth/oauth2/microsoft/callback",
                    "GET /api/auth/oauth2/providers"
                },
                ["database_changes"] = new[]
                {
                    "add_oauth_provider_column_to_users",
                    "add_oauth_external_id_column_to_users",
                    "create_oauth_tokens_table"
                },
                ["byzantine_config"] = new
                {
                    fault_tolerance_level = "byzantine",
                    max_byzantine_faults = 3,
                    required_consensus_percentage = 0.75,
                    reputation_weighted_voting = true,
                    cryptographic_verification = true,
                    hierarchical_validation = true,
                    timeout_seconds = 600
                },
                ["consensus_requirements"] = new
                {
                    security_review_required = true,
                    integration_validation_required = true,
                    requirement_mapping_required = true,
                    test_coverage_validation = true,
                    min_agreement_threshold = 0.8
                }
            }
        };
    }

    /// <summary>
    /// Sample security-focused PR data with maximum Byzantine protection
    /// </summary>
    public static SwarmTask CreateByzantineSecurityPR()
    {
        return new SwarmTask
        {
            Description = "Maximum Byzantine security analysis: JWT validation and rate limiting",
            RequiredRoles = new[]
            {
                "Orchestrator",
                "PRExtractor",
                "CodeAnalyzer",
                "RiskAssessment",
                "IntegrationAnalyzer",
                "Validator",
                "Security",
                "SummaryGenerator"
            },
            RequiredConsensus = true,
            FaultToleranceLevel = FaultToleranceLevel.Byzantine,
            SecurityLevel = SecurityLevel.Critical,
            Parameters = new Dictionary<string, object>
            {
                ["pr_data"] = new
                {
                    id = 9999,
                    title = "CRITICAL: Enhanced JWT validation and API rate limiting with Byzantine protection",
                    description = """
                        ## Security Improvements

                        ### JWT Validation Enhancements
                        - Added stricter token expiration validation
                        - Implemented token blacklisting for revoked tokens
                        - Enhanced issuer and audience validation
                        - Added support for key rotation

                        ### Rate Limiting
                        - Implemented sliding window rate limiter
                        - Different limits for authenticated vs anonymous users
                        - Per-endpoint rate limiting configuration
                        - Distributed rate limiting with Redis support

                        ### Byzantine Protection
                        - Multi-layer validation with consensus
                        - Cryptographic signature verification
                        - Advanced reputation tracking
                        - Real-time Byzantine attack detection
                        - Automatic recovery and quarantine

                        ## Security Testing
                        - Penetration testing completed
                        - Byzantine attack simulation passed
                        - OWASP security scan passed
                        - Load testing with rate limiting verified

                        Addresses: SEC-890, SEC-891, SEC-892
                        Security Review: APPROVED by security team
                        Byzantine-Validated: MAXIMUM SECURITY
                        """,
                    author = "security-team@company.com",
                    created_at = "2024-01-25T11:00:00Z",
                    updated_at = "2024-01-25T15:30:00Z",
                    status = "approved",
                    labels = new[] { "security", "critical", "jwt", "rate-limiting", "byzantine-maximum" },
                    linked_issues = new[] { "SEC-890", "SEC-891", "SEC-892" },
                    security_review = true,
                    consensus_required = true,
                    min_validators = 7,
                    reputation_threshold = 0.9
                },
                ["files_changed"] = new[]
                {
                    new
                    {
                        filename = "src/Authentication/JwtValidator.cs",
                        status = "modified",
                        additions = 65,
                        deletions = 20,
                        risk_level = "critical",
                        security_impact = "critical",
                        requires_consensus = true,
                        byzantine_validation = true
                    },
                    new
                    {
                        filename = "src/Authentication/TokenBlacklist.cs",
                        status = "added",
                        additions = 45,
                        deletions = 0,
                        risk_level = "high",
                        security_impact = "high",
                        requires_consensus = true,
                        byzantine_validation = true
                    },
                    new
                    {
                        filename = "src/Middleware/RateLimitingMiddleware.cs",
                        status = "added",
                        additions = 120,
                        deletions = 0,
                        risk_level = "high",
                        security_impact = "high",
                        requires_consensus = true,
                        byzantine_validation = true
                    }
                },
                ["security_considerations"] = new
                {
                    sensitive_endpoints = new[] { "/api/auth/*", "/api/admin/*", "/api/users/*/sensitive" },
                    authentication_changes = true,
                    authorization_changes = true,
                    data_encryption_changes = false,
                    external_dependencies = false,
                    breaking_changes = false,
                    byzantine_protection_level = "maximum"
                },
                ["byzantine_config"] = new
                {
                    fault_tolerance_level = "byzantine",
                    max_byzantine_faults = 5,
                    required_consensus_percentage = 0.85,
                    reputation_weighted_voting = true,
                    cryptographic_verification = true,
                    hierarchical_validation = true,
                    real_time_monitoring = true,
                    automatic_quarantine = true,
                    timeout_seconds = 900
                },
                ["advanced_validation"] = new
                {
                    multi_layer_consensus = true,
                    cross_validation = true,
                    reputation_history_analysis = true,
                    behavioral_pattern_detection = true,
                    timing_attack_prevention = true,
                    adversarial_input_detection = true
                }
            }
        };
    }

    /// <summary>
    /// Sample Byzantine attack simulation data
    /// </summary>
    public static SwarmTask CreateByzantineAttackSimulation()
    {
        return new SwarmTask
        {
            Description = "Byzantine attack simulation: Test system resilience under adversarial conditions",
            RequiredRoles = new[]
            {
                "Orchestrator",
                "PRExtractor",
                "CodeAnalyzer",
                "Validator",
                "RiskAssessment",
                "Security",
                "SummaryGenerator"
            },
            RequiredConsensus = true,
            FaultToleranceLevel = FaultToleranceLevel.Byzantine,
            SecurityLevel = SecurityLevel.Critical,
            Parameters = new Dictionary<string, object>
            {
                ["simulation_config"] = new
                {
                    attack_type = "coordinated_byzantine",
                    malicious_agent_percentage = 0.30, // 30% malicious agents
                    attack_patterns = new[]
                    {
                        "contradictory_responses",
                        "timing_attacks",
                        "reputation_manipulation",
                        "consensus_disruption",
                        "signature_forgery_attempts"
                    },
                    duration_minutes = 10,
                    intensity_level = "high"
                },
                ["pr_data"] = new
                {
                    id = 99999,
                    title = "Byzantine Attack Simulation - System Resilience Test",
                    description = """
                        ## Simulation Overview
                        This is a controlled Byzantine attack simulation to test system resilience.
                        
                        ## Attack Scenarios
                        - Coordinated malicious agent responses
                        - Consensus disruption attempts
                        - Reputation system manipulation
                        - Timing-based attacks
                        - Signature forgery attempts
                        
                        ## Expected Outcomes
                        - System should maintain safety and liveness
                        - Consensus should be reached despite attacks
                        - Malicious agents should be detected and quarantined
                        - Reputation system should remain intact
                        - Recovery mechanisms should activate
                        
                        ## Success Criteria
                        - System survives 30% Byzantine agents
                        - Consensus reached within timeout
                        - All attacks detected and mitigated
                        - No data corruption or security breaches
                        """,
                    author = "security-testing@company.com",
                    status = "simulation",
                    labels = new[] { "simulation", "byzantine", "security-test", "resilience" },
                    consensus_required = true,
                    attack_simulation = true
                },
                ["byzantine_config"] = new
                {
                    fault_tolerance_level = "byzantine",
                    max_byzantine_faults = 7, // Higher than normal for testing
                    required_consensus_percentage = 0.67,
                    reputation_weighted_voting = true,
                    cryptographic_verification = true,
                    real_time_monitoring = true,
                    automatic_quarantine = true,
                    recovery_mechanisms = true,
                    timeout_seconds = 1200
                },
                ["monitoring_config"] = new
                {
                    track_attack_patterns = true,
                    log_byzantine_behavior = true,
                    measure_recovery_time = true,
                    analyze_consensus_impact = true,
                    reputation_impact_analysis = true,
                    security_breach_detection = true
                }
            }
        };
    }
}