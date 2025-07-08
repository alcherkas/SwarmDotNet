using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using ByzantineSwarmDotnet.Configuration;
using ByzantineSwarmDotnet.Interfaces;
using ByzantineSwarmDotnet.Services;
using ByzantineSwarmDotnet.Models;

#pragma warning disable SKEXP0070

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        // Configuration
        var swarmConfig = new SwarmConfiguration();
        context.Configuration.Bind(swarmConfig);
        services.AddSingleton(swarmConfig);

        // Semantic Kernel
        services.AddSingleton<Kernel>(provider =>
        {
            var config = provider.GetRequiredService<SwarmConfiguration>();
            return Kernel.CreateBuilder()
                .AddOllamaChatCompletion(config.EnhancedSwarmConfiguration.DefaultModel, 
                    new Uri(config.Ollama.Endpoint))
                .Build();
        });

        // Core Services
        services.AddSingleton<ICryptographicSigner, CryptographicSigner>();
        services.AddSingleton<ICircuitBreakerRegistry, CircuitBreakerRegistry>();
        services.AddSingleton<IDistributedReputationStore, InMemoryReputationStore>();
        services.AddSingleton<IReputationSystem, ReputationSystem>();
        services.AddSingleton<IByzantineDetector, ByzantineDetector>();
        services.AddSingleton<IHierarchicalAgentPool, HierarchicalAgentPool>();
        services.AddSingleton<ILoadBalancer, SimpleLoadBalancer>();
        services.AddSingleton<IByzantineTaskDistributor, ByzantineTaskDistributor>();
        services.AddSingleton<IAdvancedFaultDetector, SimpleFaultDetector>();
        services.AddSingleton<IPBFTConsensusEngine, SimplePBFTConsensusEngine>();
        services.AddSingleton<IFaultTolerantCommunicationHub, SimpleFaultTolerantCommunicationHub>();
        services.AddSingleton<IByzantineTolerantSwarmOrchestrator, ByzantineTolerantSwarmOrchestrator>();

        // Add hosted service for the swarm demo
        services.AddHostedService<SwarmDemoService>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
        logging.SetMinimumLevel(LogLevel.Information);
    })
    .Build();

await host.RunAsync();

// Demo Service
public class SwarmDemoService : BackgroundService
{
    private readonly ILogger<SwarmDemoService> _logger;
    private readonly IByzantineTolerantSwarmOrchestrator _orchestrator;
    private readonly IHierarchicalAgentPool _agentPool;
    private readonly SwarmConfiguration _configuration;

    public SwarmDemoService(
        ILogger<SwarmDemoService> logger,
        IByzantineTolerantSwarmOrchestrator orchestrator,
        IHierarchicalAgentPool agentPool,
        SwarmConfiguration configuration)
    {
        _logger = logger;
        _orchestrator = orchestrator;
        _agentPool = agentPool;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Starting Byzantine Swarm Demo...");

            // Initialize the swarm with agents
            await InitializeSwarmAsync();

            // Run demo scenarios
            await RunDemoScenariosAsync(stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in swarm demo");
        }
    }

    private async Task InitializeSwarmAsync()
    {
        _logger.LogInformation("Initializing swarm with {RequiredAgents} agents...", 
            _configuration.EnhancedSwarmConfiguration.RequiredAgents);

        // Scale up to required agent count
        await _agentPool.ScaleUpAsync(_configuration.EnhancedSwarmConfiguration.RequiredAgents);

        // Verify Byzantine fault tolerance
        var isValid = await _orchestrator.ValidateByzantineToleranceRequirements();
        if (!isValid)
        {
            throw new InvalidOperationException("Failed to establish Byzantine fault tolerance");
        }

        var activeAgents = await _agentPool.GetActiveAgentsAsync();
        _logger.LogInformation("Swarm initialized with {Count} active agents", activeAgents.Count);

        // Display agent distribution
        foreach (var role in Enum.GetValues<AgentRole>())
        {
            var roleAgents = await _agentPool.GetAgentsByRoleAsync(role);
            if (roleAgents.Any())
            {
                _logger.LogInformation("  {Role}: {Count} agents", role, roleAgents.Count);
            }
        }
    }

    private async Task RunDemoScenariosAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Running demo scenarios...");

        // Scenario 1: Simple distributed task
        await RunSimpleTaskScenario();

        await Task.Delay(5000, cancellationToken);

        // Scenario 2: Complex consensus task
        await RunConsensusTaskScenario();

        await Task.Delay(5000, cancellationToken);

        // Scenario 3: Byzantine fault tolerance test
        await RunByzantineFaultToleranceTest();

        await Task.Delay(5000, cancellationToken);

        // Scenario 4: System monitoring and metrics
        await RunSystemMonitoringScenario();

        _logger.LogInformation("Demo scenarios completed successfully!");
    }

    private async Task RunSimpleTaskScenario()
    {
        _logger.LogInformation("\n=== Scenario 1: Simple Distributed Task ===");

        var task = new SwarmTask
        {
            Description = "Analyze the benefits and challenges of distributed artificial intelligence systems",
            RequiredRoles = new[] { "Supervisor", "Worker", "Validator" },
            MaxExecutionTime = TimeSpan.FromMinutes(2),
            RequiredConsensus = false,
            FaultToleranceLevel = FaultToleranceLevel.Basic,
            SecurityLevel = SecurityLevel.Medium
        };

        var startTime = DateTime.UtcNow;
        var result = await _orchestrator.ExecuteTaskAsync(task);
        var duration = DateTime.UtcNow - startTime;

        _logger.LogInformation("Task completed in {Duration:F2} seconds", duration.TotalSeconds);
        _logger.LogInformation("Success: {Success}, Consensus: {Consensus}, Trust Score: {TrustScore:F3}",
            result.Success, result.ConsensusReached, result.TrustScore);
        _logger.LogInformation("Participating Agents: {Count}, Byzantine Faults: {Faults}",
            result.ParticipatingAgents, result.ByzantineFaultsDetected);
    }

    private async Task RunConsensusTaskScenario()
    {
        _logger.LogInformation("\n=== Scenario 2: Consensus-Required Task ===");

        var task = new SwarmTask
        {
            Description = "Evaluate the security implications of a new distributed consensus protocol and provide recommendations",
            RequiredRoles = new[] { "Supervisor", "Validator", "Security", "ReputationManager" },
            MaxExecutionTime = TimeSpan.FromMinutes(3),
            RequiredConsensus = true,
            FaultToleranceLevel = FaultToleranceLevel.Byzantine,
            SecurityLevel = SecurityLevel.High
        };

        var startTime = DateTime.UtcNow;
        var result = await _orchestrator.ExecuteTaskAsync(task);
        var duration = DateTime.UtcNow - startTime;

        _logger.LogInformation("Consensus task completed in {Duration:F2} seconds", duration.TotalSeconds);
        _logger.LogInformation("Success: {Success}, Consensus: {Consensus}, Confidence: {Confidence:F3}",
            result.Success, result.ConsensusReached, result.ConsensusConfidence);
        _logger.LogInformation("Average Reputation: {Reputation:F3}, Trust Score: {TrustScore:F3}",
            result.AverageReputationScore, result.TrustScore);
    }

    private async Task RunByzantineFaultToleranceTest()
    {
        _logger.LogInformation("\n=== Scenario 3: Byzantine Fault Tolerance Test ===");

        // Simulate Byzantine faults by reducing some agents' reputation
        var agents = await _agentPool.GetActiveAgentsAsync();
        var faultyAgents = agents.Take(3).ToList(); // Simulate 3 faulty agents

        foreach (var agent in faultyAgents)
        {
            await _agentPool.UpdateAgentReputationAsync(agent.Id, 0.1); // Low reputation
            _logger.LogInformation("Simulated Byzantine fault for agent {AgentId}", agent.Id);
        }

        var task = new SwarmTask
        {
            Description = "Critical task: Design a fault-tolerant distributed system architecture that can withstand Byzantine attacks",
            RequiredRoles = new[] { "Supervisor", "Validator", "Security" },
            MaxExecutionTime = TimeSpan.FromMinutes(4),
            RequiredConsensus = true,
            FaultToleranceLevel = FaultToleranceLevel.Byzantine,
            SecurityLevel = SecurityLevel.Critical
        };

        var startTime = DateTime.UtcNow;
        var result = await _orchestrator.ExecuteTaskAsync(task);
        var duration = DateTime.UtcNow - startTime;

        _logger.LogInformation("Byzantine fault tolerance test completed in {Duration:F2} seconds", duration.TotalSeconds);
        _logger.LogInformation("System remained operational: {Success}", result.Success);
        _logger.LogInformation("Byzantine faults detected and handled: {Faults}", result.ByzantineFaultsDetected);
        _logger.LogInformation("Safety maintained: {Safety}", result.ConsensusReached);

        // Restore agent reputations
        foreach (var agent in faultyAgents)
        {
            await _agentPool.UpdateAgentReputationAsync(agent.Id, 0.5);
        }
    }

    private async Task RunSystemMonitoringScenario()
    {
        _logger.LogInformation("\n=== Scenario 4: System Monitoring and Metrics ===");

        var metrics = await _orchestrator.GetRealTimeMetricsAsync();

        _logger.LogInformation("Real-time Swarm Metrics:");
        _logger.LogInformation("  Active Agents: {ActiveAgents}", metrics.ActiveAgents);
        _logger.LogInformation("  Consensus Participation: {Participation:P2}", metrics.ConsensusParticipation);
        _logger.LogInformation("  Suspicious Activity Score: {SuspiciousActivity:P2}", metrics.SuspiciousActivityScore);
        _logger.LogInformation("  Average Response Time: {ResponseTime:F2} ms", metrics.AverageResponseTime);
        _logger.LogInformation("  Tasks Completed: {Completed}", metrics.TasksCompleted);
        _logger.LogInformation("  Tasks Failed: {Failed}", metrics.TasksFailed);
        _logger.LogInformation("  System Load: {Load:P2}", metrics.SystemLoad);

        // Test recovery mechanisms
        _logger.LogInformation("Testing recovery mechanisms...");
        await _orchestrator.InvestigateConsensusIssuesAsync();

        _logger.LogInformation("System monitoring scenario completed");
    }
}