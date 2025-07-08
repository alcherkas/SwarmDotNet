using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using ByzantineSwarmDotnet.Configuration;
using ByzantineSwarmDotnet.Interfaces;
using ByzantineSwarmDotnet.Services;

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