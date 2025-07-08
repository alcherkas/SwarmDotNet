using ByzantineSwarmDotnet.Models;

namespace ByzantineSwarmDotnet.Interfaces;

public interface ISwarmMonitor
{
    Task<SwarmMetrics> GetCurrentMetricsAsync();
    Task<List<SwarmMetrics>> GetMetricsHistoryAsync(TimeSpan period);
    Task RecordMetricAsync(string metricName, double value);
    Task<bool> CheckAlertThresholdsAsync();
    Task NotifyAlertAsync(string alertType, string message);
}