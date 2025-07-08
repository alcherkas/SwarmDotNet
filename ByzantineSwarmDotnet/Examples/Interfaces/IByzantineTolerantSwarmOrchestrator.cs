using ByzantineSwarmDotnet.Examples.Models;

namespace ByzantineSwarmDotnet.Examples.Interfaces;

public interface IByzantineTolerantSwarmOrchestrator
{
    Task<SwarmResult> ExecuteTaskAsync(SwarmTask task);
}