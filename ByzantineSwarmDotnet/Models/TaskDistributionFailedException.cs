namespace ByzantineSwarmDotnet.Models;

public class TaskDistributionFailedException : Exception
{
    public TaskDistributionFailedException(string message) : base(message) { }
    public TaskDistributionFailedException(string message, Exception innerException) : base(message, innerException) { }
}