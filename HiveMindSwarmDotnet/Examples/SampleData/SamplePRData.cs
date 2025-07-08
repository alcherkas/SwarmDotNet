using HiveMindSwarmDotnet.Console.Models;

namespace HiveMindSwarmDotnet.Examples.SampleData;

/// <summary>
/// Sample pull request data for testing and demonstration purposes
/// </summary>
public static class SamplePRData
{
    /// <summary>
    /// Sample bug fix PR data
    /// </summary>
    public static SwarmTask CreateBugFixPR()
    {
        return new SwarmTask
        {
            Description = "Analyze bug fix: Resolve memory leak in background service",
            RequiredRoles = new[]
            {
                AgentRole.PRExtractor,
                AgentRole.CodeAnalyzer,
                AgentRole.RiskAssessment,
                AgentRole.TestCoverage,
                AgentRole.SummaryGenerator
            },
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

                        Fixes: BUG-789
                        """,
                    author = "developer@company.com",
                    created_at = "2024-01-15T10:30:00Z",
                    updated_at = "2024-01-15T14:20:00Z",
                    status = "open",
                    labels = new[] { "bug", "performance", "backend" },
                    linked_issues = new[] { "BUG-789" }
                },
                ["files_changed"] = new[]
                {
                    new
                    {
                        filename = "src/Services/BackgroundTaskService.cs",
                        status = "modified",
                        additions = 15,
                        deletions = 8,
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
                        filename = "src/Program.cs",
                        status = "modified",
                        additions = 2,
                        deletions = 1,
                        diff = """
                            @@ -25,7 +25,8 @@ var builder = WebApplication.CreateBuilder(args);
                             builder.Services.AddScoped<ITaskProcessor, TaskProcessor>();
                             builder.Services.AddScoped<INotificationService, NotificationService>();
                            -builder.Services.AddSingleton<BackgroundTaskService>();
                            +builder.Services.AddHttpClient();
                            +builder.Services.AddSingleton<BackgroundTaskService>();
                            """
                    },
                    new
                    {
                        filename = "tests/Services/BackgroundTaskServiceTests.cs",
                        status = "added",
                        additions = 45,
                        deletions = 0,
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
                ["commit_messages"] = new[]
                {
                    "fix: implement proper HttpClient disposal in BackgroundTaskService",
                    "test: add memory leak test for BackgroundTaskService",
                    "refactor: use IHttpClientFactory for better resource management"
                }
            }
        };
    }

    /// <summary>
    /// Sample new feature PR data
    /// </summary>
    public static SwarmTask CreateNewFeaturePR()
    {
        return new SwarmTask
        {
            Description = "Analyze new feature: OAuth2 integration with third-party providers",
            RequiredRoles = new[]
            {
                AgentRole.PRExtractor,
                AgentRole.JiraContext,
                AgentRole.CodeAnalyzer,
                AgentRole.RequirementMapper,
                AgentRole.TestCoverage,
                AgentRole.RiskAssessment,
                AgentRole.IntegrationAnalyzer,
                AgentRole.SummaryGenerator
            },
            Parameters = new Dictionary<string, object>
            {
                ["pr_data"] = new
                {
                    id = 5678,
                    title = "Add OAuth2 integration with Google and Microsoft",
                    description = """
                        ## Feature Overview
                        Implements OAuth2 authentication with Google and Microsoft providers to allow 
                        users to sign in with their existing accounts.

                        ## Implementation Details
                        - Added OAuth2 service with provider abstraction
                        - Implemented Google and Microsoft OAuth2 providers
                        - Added user account linking functionality
                        - Updated authentication middleware
                        - Added OAuth2 configuration settings

                        ## Security Considerations
                        - PKCE flow implementation for enhanced security
                        - Secure token storage and refresh handling
                        - Rate limiting for OAuth endpoints
                        - Comprehensive input validation

                        ## Dependencies
                        - Microsoft.AspNetCore.Authentication.Google v6.0.0
                        - Microsoft.AspNetCore.Authentication.MicrosoftAccount v6.0.0

                        Implements: FEAT-456, FEAT-457
                        Related: SEC-123 (security review)
                        """,
                    author = "senior-dev@company.com",
                    created_at = "2024-01-20T09:15:00Z",
                    updated_at = "2024-01-22T16:45:00Z",
                    status = "ready_for_review",
                    labels = new[] { "feature", "authentication", "oauth2", "security" },
                    linked_issues = new[] { "FEAT-456", "FEAT-457", "SEC-123" }
                },
                ["files_changed"] = new[]
                {
                    new
                    {
                        filename = "src/Authentication/OAuth2/IOAuth2Service.cs",
                        status = "added",
                        additions = 35,
                        deletions = 0
                    },
                    new
                    {
                        filename = "src/Authentication/OAuth2/OAuth2Service.cs",
                        status = "added",
                        additions = 120,
                        deletions = 0
                    },
                    new
                    {
                        filename = "src/Authentication/OAuth2/Providers/GoogleOAuth2Provider.cs",
                        status = "added",
                        additions = 85,
                        deletions = 0
                    },
                    new
                    {
                        filename = "src/Authentication/OAuth2/Providers/MicrosoftOAuth2Provider.cs",
                        status = "added",
                        additions = 90,
                        deletions = 0
                    },
                    new
                    {
                        filename = "src/Controllers/AuthController.cs",
                        status = "modified",
                        additions = 45,
                        deletions = 5
                    },
                    new
                    {
                        filename = "src/Models/User.cs",
                        status = "modified",
                        additions = 8,
                        deletions = 2
                    },
                    new
                    {
                        filename = "src/Configuration/OAuth2Configuration.cs",
                        status = "added",
                        additions = 25,
                        deletions = 0
                    },
                    new
                    {
                        filename = "tests/Authentication/OAuth2ServiceTests.cs",
                        status = "added",
                        additions = 150,
                        deletions = 0
                    },
                    new
                    {
                        filename = "tests/Integration/OAuth2IntegrationTests.cs",
                        status = "added",
                        additions = 200,
                        deletions = 0
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
                }
            }
        };
    }

    /// <summary>
    /// Sample security-focused PR data
    /// </summary>
    public static SwarmTask CreateSecurityPR()
    {
        return new SwarmTask
        {
            Description = "Security analysis: Update JWT validation and add rate limiting",
            RequiredRoles = new[]
            {
                AgentRole.PRExtractor,
                AgentRole.CodeAnalyzer,
                AgentRole.RiskAssessment,
                AgentRole.IntegrationAnalyzer,
                AgentRole.SummaryGenerator
            },
            Parameters = new Dictionary<string, object>
            {
                ["pr_data"] = new
                {
                    id = 9999,
                    title = "Security: Enhanced JWT validation and API rate limiting",
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

                        ### Additional Security Measures
                        - Updated security headers middleware
                        - Enhanced request validation
                        - Improved error handling to prevent information leakage

                        ## Breaking Changes
                        - None - all changes are backward compatible

                        ## Security Testing
                        - Penetration testing completed
                        - OWASP security scan passed
                        - Load testing with rate limiting verified

                        Addresses: SEC-890, SEC-891, SEC-892
                        Security Review: APPROVED by security team
                        """,
                    author = "security-team@company.com",
                    created_at = "2024-01-25T11:00:00Z",
                    updated_at = "2024-01-25T15:30:00Z",
                    status = "approved",
                    labels = new[] { "security", "critical", "jwt", "rate-limiting" },
                    linked_issues = new[] { "SEC-890", "SEC-891", "SEC-892" },
                    security_review = true
                },
                ["files_changed"] = new[]
                {
                    new
                    {
                        filename = "src/Authentication/JwtValidator.cs",
                        status = "modified",
                        additions = 65,
                        deletions = 20,
                        security_impact = "high"
                    },
                    new
                    {
                        filename = "src/Authentication/TokenBlacklist.cs",
                        status = "added",
                        additions = 45,
                        deletions = 0,
                        security_impact = "medium"
                    },
                    new
                    {
                        filename = "src/Middleware/RateLimitingMiddleware.cs",
                        status = "added",
                        additions = 120,
                        deletions = 0,
                        security_impact = "high"
                    },
                    new
                    {
                        filename = "src/Middleware/SecurityHeadersMiddleware.cs",
                        status = "modified",
                        additions = 15,
                        deletions = 5,
                        security_impact = "low"
                    },
                    new
                    {
                        filename = "src/Configuration/SecurityConfiguration.cs",
                        status = "modified",
                        additions = 30,
                        deletions = 8,
                        security_impact = "medium"
                    }
                },
                ["security_considerations"] = new
                {
                    sensitive_endpoints = new[] { "/api/auth/*", "/api/admin/*", "/api/users/*/sensitive" },
                    authentication_changes = true,
                    authorization_changes = false,
                    data_encryption_changes = false,
                    external_dependencies = false,
                    breaking_changes = false
                }
            }
        };
    }
}

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