namespace ByzantineSwarmDotnet.Interfaces;

public interface IPerformanceMonitor
{
    Task<TimeSpan> GetAverageResponseTimeAsync(string agentId);
    Task<double> GetSuccessRateAsync(string agentId);
    Task<int> GetCurrentLoadAsync(string agentId);
    Task RecordTaskExecutionAsync(string agentId, TimeSpan executionTime, bool success);
}