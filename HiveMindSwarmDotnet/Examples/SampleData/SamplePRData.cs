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