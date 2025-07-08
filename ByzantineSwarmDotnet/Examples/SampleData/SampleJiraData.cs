namespace ByzantineSwarmDotnet.Examples.SampleData;

/// <summary>
/// Sample Jira context data for Byzantine swarm demonstration
/// </summary>
public static class SampleJiraData
{
    public static Dictionary<string, object> GetByzantineJiraContext(string ticketId)
    {
        return ticketId switch
        {
            "BUG-789" => new Dictionary<string, object>
            {
                ["ticket"] = new
                {
                    id = "BUG-789",
                    title = "Memory leak in background service causing OOM errors",
                    description = """
                                  ## Problem Description
                                  Production servers are experiencing memory leaks that eventually lead to OutOfMemoryExceptions.
                                  Memory usage grows continuously during normal operation.

                                  ## Byzantine Validation Requirements
                                  - Requires consensus from multiple validators
                                  - Cryptographic signatures for all validations
                                  - Reputation tracking for participating agents
                                  - Enhanced security due to production impact

                                  ## Acceptance Criteria
                                  - [ ] Fix memory leak in BackgroundTaskService
                                  - [ ] Verify with memory profiler
                                  - [ ] Add memory leak prevention tests
                                  - [ ] Document proper HttpClient usage patterns
                                  - [ ] Pass Byzantine consensus validation
                                  - [ ] Maintain reputation score above threshold
                                  """,
                    priority = "High",
                    type = "Bug",
                    status = "In Progress",
                    byzantine_validated = true,
                    consensus_required = true,
                    security_level = "high",
                    reputation_threshold = 0.7
                }
            },
            "FEAT-456" => new Dictionary<string, object>
            {
                ["ticket"] = new
                {
                    id = "FEAT-456",
                    title = "OAuth2 Integration - Google Provider with Byzantine Validation",
                    description = """
                                  ## Business Need
                                  Users want to sign in using their Google accounts with enhanced security validation.

                                  ## Byzantine Security Requirements
                                  - Multi-agent consensus validation
                                  - Cryptographic signature verification
                                  - Advanced reputation tracking
                                  - Real-time Byzantine attack detection

                                  ## Acceptance Criteria
                                  - [ ] Users can click "Sign in with Google" button
                                  - [ ] OAuth2 flow redirects to Google authentication
                                  - [ ] Successful authentication creates or links user account
                                  - [ ] User profile information is populated from Google
                                  - [ ] Secure token handling and refresh
                                  - [ ] Error handling for failed authentication
                                  - [ ] Pass Byzantine consensus validation
                                  - [ ] Cryptographic signatures verified
                                  - [ ] Reputation system integration
                                  """,
                    priority = "Medium",
                    type = "Story",
                    status = "In Development",
                    byzantine_validated = true,
                    consensus_required = true,
                    security_level = "critical",
                    reputation_threshold = 0.8
                }
            },
            "SEC-890" => new Dictionary<string, object>
            {
                ["ticket"] = new
                {
                    id = "SEC-890",
                    title = "Enhance JWT Token Validation with Maximum Byzantine Protection",
                    description = """
                                  ## Security Requirement
                                  Current JWT validation needs maximum Byzantine protection for critical security functions.

                                  ## Byzantine Protection Requirements
                                  - Multi-layer validation with consensus
                                  - Cryptographic signature verification
                                  - Advanced reputation tracking
                                  - Real-time Byzantine attack detection
                                  - Automatic recovery and quarantine

                                  ## Security Requirements
                                  - Implement strict token expiration validation
                                  - Add token blacklisting capability
                                  - Support automatic key rotation
                                  - Enhance issuer and audience validation
                                  - Prevent timing attacks in validation
                                  - Byzantine fault tolerance integration
                                  - Maximum security consensus protocols
                                  """,
                    priority = "Critical",
                    type = "Security",
                    status = "Code Review",
                    byzantine_validated = true,
                    consensus_required = true,
                    security_level = "critical",
                    reputation_threshold = 0.9,
                    max_byzantine_protection = true
                }
            },
            _ => new Dictionary<string, object>
            {
                ["error"] = $"Byzantine Jira ticket {ticketId} not found",
                ["byzantine_config"] = new
                {
                    default_consensus_required = true,
                    default_security_level = "medium",
                    default_reputation_threshold = 0.6
                }
            }
        };
    }
}