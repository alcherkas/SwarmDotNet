using ByzantineSwarmDotnet.Examples.Workflows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ByzantineSwarmDotnet.Examples;

/// <summary>
/// Main program entry point for running Byzantine Swarm examples
/// </summary>
public class Program
{
    public static async Task Main(string[] args)
    {
        System.Console.WriteLine("🚀 Byzantine Swarm Examples");
        System.Console.WriteLine("===========================");
        System.Console.WriteLine();

        try
        {
            // Build configuration
            var configuration = BuildConfiguration();

            // Build and configure host
            using var host = CreateHostBuilder(args, configuration).Build();

            // Initialize swarm services
            await InitializeSwarmAsync(host.Services);

            // Check for non-interactive mode or command-line arguments
            if (args.Length > 0)
            {
                await RunNonInteractiveAsync(host.Services, args);
            }
            else if (Environment.UserInteractive && !System.Console.IsInputRedirected)
            {
                // Show menu and run selected examples
                await RunExampleMenuAsync(host.Services);
            }
            else
            {
                // Non-interactive environment - run all examples
                System.Console.WriteLine("🤖 Running in non-interactive mode - executing all examples");
                await RunAllExamplesAsync(host.Services);
            }
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"❌ Error running examples: {ex.Message}");
            System.Console.WriteLine($"📋 Details: {ex}");
            Environment.Exit(1);
        }

        if (Environment.UserInteractive && !System.Console.IsInputRedirected)
        {
            System.Console.WriteLine("\n✨ Examples completed! Press any key to exit...");
            System.Console.ReadKey();
        }
        else
        {
            System.Console.WriteLine("\n✨ Examples completed!");
        }
    }

    private static IConfiguration BuildConfiguration()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables()
            .Build();
    }

    private static IHostBuilder CreateHostBuilder(string[] args, IConfiguration configuration)
    {
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((context, services) =>
            {
                // Add Byzantine swarm services
                services.AddByzantineSwarmServices(configuration);
                
                // Add example services
                services.AddScoped<ByzantineSwarmUsageExample>();
                services.AddScoped<ByzantineCodeReviewWorkflow>();
                services.AddScoped<ByzantineAdvancedWorkflowScenarios>();
                
                // Configure logging
                services.AddLogging(builder =>
                {
                    builder.ClearProviders();
                    builder.AddConsole();
                    builder.SetMinimumLevel(LogLevel.Information);
                });
            });
    }

    private static async Task InitializeSwarmAsync(IServiceProvider serviceProvider)
    {
        // Initialize Byzantine swarm services
        var logger = serviceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogInformation("🔧 Initializing Byzantine Swarm Services...");
        
        // Add any specific initialization logic here
        await Task.Delay(500); // Simulate initialization
        
        logger.LogInformation("✅ Byzantine Swarm Services initialized");
    }

    private static async Task RunNonInteractiveAsync(IServiceProvider serviceProvider, string[] args)
    {
        var option = args[0].ToLower();
        
        switch (option)
        {
            case "1" or "basic":
                await RunBasicExamplesAsync(serviceProvider);
                break;
            case "2" or "workflow":
                await RunWorkflowExamplesAsync(serviceProvider);
                break;
            case "3" or "advanced":
                await RunAdvancedExamplesAsync(serviceProvider);
                break;
            case "4" or "byzantine":
                await RunByzantineExamplesAsync(serviceProvider);
                break;
            case "5" or "all":
                await RunAllExamplesAsync(serviceProvider);
                break;
            default:
                System.Console.WriteLine($"❌ Unknown option: {option}");
                System.Console.WriteLine("Available options: 1/basic, 2/workflow, 3/advanced, 4/byzantine, 5/all");
                Environment.Exit(1);
                break;
        }
    }

    private static async Task RunExampleMenuAsync(IServiceProvider serviceProvider)
    {
        while (true)
        {
            System.Console.WriteLine("\n📋 Available Examples:");
            System.Console.WriteLine("1. Basic Pull Request Analysis Examples");
            System.Console.WriteLine("2. Detailed Code Review Workflow");
            System.Console.WriteLine("3. Advanced Workflow Scenarios");
            System.Console.WriteLine("4. Byzantine Fault Tolerance Examples");
            System.Console.WriteLine("5. Run All Examples");
            System.Console.WriteLine("0. Exit");
            System.Console.Write("\nSelect an option (0-5): ");

            var input = System.Console.ReadLine();
            
            try
            {
                switch (input)
                {
                    case "1":
                        await RunBasicExamplesAsync(serviceProvider);
                        break;
                    case "2":
                        await RunWorkflowExamplesAsync(serviceProvider);
                        break;
                    case "3":
                        await RunAdvancedExamplesAsync(serviceProvider);
                        break;
                    case "4":
                        await RunByzantineExamplesAsync(serviceProvider);
                        break;
                    case "5":
                        await RunAllExamplesAsync(serviceProvider);
                        break;
                    case "0":
                        return;
                    default:
                        System.Console.WriteLine("❌ Invalid option. Please select 0-5.");
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine($"❌ Error running example: {ex.Message}");
                System.Console.WriteLine("Press any key to continue...");
                System.Console.ReadKey();
            }

            System.Console.WriteLine("\nPress any key to continue...");
            System.Console.ReadKey();
        }
    }

    private static async Task RunBasicExamplesAsync(IServiceProvider serviceProvider)
    {
        System.Console.WriteLine("\n🎯 Running Basic PR Analysis Examples");
        System.Console.WriteLine("=====================================");

        var example = serviceProvider.GetRequiredService<ByzantineSwarmUsageExample>();
        await example.RunPullRequestAnalysisExampleAsync();
    }

    private static async Task RunWorkflowExamplesAsync(IServiceProvider serviceProvider)
    {
        System.Console.WriteLine("\n🔄 Running Detailed Workflow Examples");
        System.Console.WriteLine("====================================");

        var workflow = serviceProvider.GetRequiredService<ByzantineCodeReviewWorkflow>();
        await workflow.RunCompleteWorkflowAsync();
    }

    private static async Task RunAdvancedExamplesAsync(IServiceProvider serviceProvider)
    {
        System.Console.WriteLine("\n⚡ Running Advanced Workflow Scenarios");
        System.Console.WriteLine("=====================================");

        var advanced = serviceProvider.GetRequiredService<ByzantineAdvancedWorkflowScenarios>();
        
        System.Console.WriteLine("\n📊 Parallel Workflow Example:");
        await advanced.RunParallelWorkflowAsync();
        
        System.Console.WriteLine("\n🧠 Adaptive Workflow Example:");
        await advanced.RunAdaptiveWorkflowAsync();
        
        System.Console.WriteLine("\n📚 Learning Workflow Example:");
        await advanced.RunLearningWorkflowAsync();
    }

    private static async Task RunByzantineExamplesAsync(IServiceProvider serviceProvider)
    {
        System.Console.WriteLine("\n🛡️ Running Byzantine Fault Tolerance Examples");
        System.Console.WriteLine("=============================================");

        var advanced = serviceProvider.GetRequiredService<ByzantineAdvancedWorkflowScenarios>();
        
        System.Console.WriteLine("\n🔒 Byzantine Fault Tolerance Example:");
        await advanced.RunByzantineFaultToleranceExampleAsync();
        
        System.Console.WriteLine("\n🎯 Consensus Algorithm Example:");
        await advanced.RunConsensusAlgorithmExampleAsync();
        
        System.Console.WriteLine("\n🛡️ Reputation System Example:");
        await advanced.RunReputationSystemExampleAsync();
    }

    private static async Task RunAllExamplesAsync(IServiceProvider serviceProvider)
    {
        System.Console.WriteLine("\n🚀 Running All Examples");
        System.Console.WriteLine("======================");

        await RunBasicExamplesAsync(serviceProvider);
        await RunWorkflowExamplesAsync(serviceProvider);
        await RunAdvancedExamplesAsync(serviceProvider);
        await RunByzantineExamplesAsync(serviceProvider);

        // Also run custom workflow example
        var example = serviceProvider.GetRequiredService<ByzantineSwarmUsageExample>();
        await example.RunCustomWorkflowExampleAsync();
    }
}