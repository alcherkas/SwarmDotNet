namespace ByzantineSwarmDotnet.Interfaces;

public interface ICircuitBreakerRegistry
{
    ICircuitBreaker GetOrCreate(string key);
    Task<bool> IsOpenAsync(string key);
    Task ResetAsync(string key);
    Task<Dictionary<string, string>> GetAllStatesAsync();
}