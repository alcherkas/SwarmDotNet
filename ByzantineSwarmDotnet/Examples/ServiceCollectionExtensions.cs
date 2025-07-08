using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ByzantineSwarmDotnet.Examples;

/// <summary>
/// Extension methods for service registration
/// </summary>
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddByzantineSwarmServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Register Byzantine swarm services here
        services.AddScoped<ByzantineSwarmDotnet.Examples.Interfaces.IByzantineTolerantSwarmOrchestrator, ByzantineSwarmDotnet.Examples.Interfaces.MockByzantineTolerantSwarmOrchestrator>();
        
        // This is a placeholder - actual implementation would register all the Byzantine swarm services
        return services;
    }
}