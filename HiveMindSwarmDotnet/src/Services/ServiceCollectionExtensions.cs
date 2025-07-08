using HiveMindSwarmDotnet.Console.Communication;
using HiveMindSwarmDotnet.Console.Configuration;
using HiveMindSwarmDotnet.Console.Core;
using HiveMindSwarmDotnet.Console.Interfaces;
using HiveMindSwarmDotnet.Console.Optimization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;

namespace HiveMindSwarmDotnet.Console.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwarmServices(this IServiceCollection services, IConfiguration configuration)
    {
        var swarmConfig = SwarmConfiguration.LoadFromConfiguration(configuration);
        
        services.AddSingleton<ISwarmConfiguration>(swarmConfig);
        
        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Information);
        });

        services.AddSingleton<Kernel>(serviceProvider =>
        {
            var config = serviceProvider.GetRequiredService<ISwarmConfiguration>();
            var logger = serviceProvider.GetRequiredService<ILogger<Kernel>>();
            
            try
            {
                var kernel = Kernel.CreateBuilder()
                    .AddOllamaChatCompletion(config.DefaultModel, new Uri(config.Ollama.Endpoint))
                    .Build();
                
                logger.LogInformation("Kernel initialized with Ollama endpoint: {Endpoint}", config.Ollama.Endpoint);
                return kernel;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to initialize Semantic Kernel with Ollama");
                throw;
            }
        });

        services.AddSingleton<ICommunicationHub, CommunicationHub>();
        services.AddSingleton<ITaskDistributor, TaskDistributor>();
        services.AddSingleton<IAgentPool, AgentPool>();
        services.AddSingleton<IOptimizationEngine, OptimizationEngine>();
        services.AddSingleton<ISwarmOrchestrator, SwarmOrchestrator>();
        services.AddSingleton<ISwarmLogger, SwarmLogger>();

        return services;
    }

    public static async Task<IServiceProvider> InitializeSwarmAsync(this IServiceProvider serviceProvider)
    {
        var logger = serviceProvider.GetRequiredService<ILogger<ISwarmOrchestrator>>();
        
        try
        {
            logger.LogInformation("Initializing Swarm services...");
            
            var agentPool = serviceProvider.GetRequiredService<IAgentPool>();
            await agentPool.InitializePoolAsync();
            
            var orchestrator = serviceProvider.GetRequiredService<ISwarmOrchestrator>();
            await orchestrator.InitializeAsync();
            
            logger.LogInformation("Swarm services initialized successfully");
            return serviceProvider;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to initialize Swarm services");
            throw;
        }
    }
}