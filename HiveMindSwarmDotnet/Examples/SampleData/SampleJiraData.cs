namespace HiveMindSwarmDotnet.Examples.SampleData;

/// <summary>
/// Sample Jira context data for demonstration
/// </summary>
public static class SampleJiraData
{
    public static Dictionary<string, object> GetJiraContext(string ticketId)
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

                                  ## Impact
                                  - Production servers require daily restarts
                                  - Performance degradation over time
                                  - Potential service outages

                                  ## Investigation Results
                                  Memory profiling shows HttpClient instances are not being disposed properly in BackgroundTaskService.

                                  ## Acceptance Criteria
                                  - [ ] Fix memory leak in BackgroundTaskService
                                  - [ ] Verify with memory profiler
                                  - [ ] Add memory leak prevention tests
                                  - [ ] Document proper HttpClient usage patterns
                                  """,
                    priority = "High",
                    type = "Bug",
                    status = "In Progress",
                    assignee = "developer@company.com",
                    reporter = "ops-team@company.com",
                    created = "2024-01-10T08:00:00Z",
                    updated = "2024-01-15T10:30:00Z",
                    labels = new[] { "production", "memory-leak", "performance" },
                    components = new[] { "Background Services" },
                    affects_versions = new[] { "v2.1.0", "v2.1.1" }
                }
            },
            "FEAT-456" => new Dictionary<string, object>
            {
                ["ticket"] = new
                {
                    id = "FEAT-456",
                    title = "OAuth2 Integration - Google Provider",
                    description = """
                                  ## Business Need
                                  Users want to sign in using their Google accounts to reduce friction and improve user experience.

                                  ## User Story
                                  As a user, I want to sign in with my Google account so that I don't have to remember another password.

                                  ## Acceptance Criteria
                                  - [ ] Users can click "Sign in with Google" button
                                  - [ ] OAuth2 flow redirects to Google authentication
                                  - [ ] Successful authentication creates or links user account
                                  - [ ] User profile information is populated from Google
                                  - [ ] Secure token handling and refresh
                                  - [ ] Error handling for failed authentication

                                  ## Technical Requirements
                                  - Implement OAuth2 PKCE flow for security
                                  - Store minimal required user data
                                  - Support account linking for existing users
                                  - Implement proper token lifecycle management
                                  """,
                    priority = "Medium",
                    type = "Story",
                    status = "In Development",
                    assignee = "senior-dev@company.com",
                    reporter = "product-manager@company.com",
                    epic = "AUTH-100",
                    story_points = 8,
                    labels = new[] { "oauth2", "google", "authentication" }
                }
            },
            "SEC-890" => new Dictionary<string, object>
            {
                ["ticket"] = new
                {
                    id = "SEC-890",
                    title = "Enhance JWT Token Validation Security",
                    description = """
                                  ## Security Requirement
                                  Current JWT validation needs to be enhanced to address security audit findings.

                                  ## Issues Identified
                                  1. Token expiration validation could be more strict
                                  2. No mechanism for token revocation/blacklisting
                                  3. Key rotation not properly supported
                                  4. Issuer validation could be strengthened

                                  ## Security Requirements
                                  - Implement strict token expiration validation
                                  - Add token blacklisting capability
                                  - Support automatic key rotation
                                  - Enhance issuer and audience validation
                                  - Prevent timing attacks in validation

                                  ## Compliance
                                  - Meets OWASP JWT security requirements
                                  - Addresses security audit findings SA-2024-001
                                  - Supports SOC2 compliance requirements
                                  """,
                    priority = "Critical",
                    type = "Security",
                    status = "Code Review",
                    assignee = "security-team@company.com",
                    reporter = "security-audit@company.com",
                    security_level = "High",
                    compliance_requirements = new[] { "OWASP", "SOC2" }
                }
            },
            _ => new Dictionary<string, object>
            {
                ["error"] = $"Jira ticket {ticketId} not found"
            }
        };
    }
}