namespace ByzantineSwarmDotnet.Models;

public class ConsensusFailedException : Exception
{
    public ConsensusFailedException(string message) : base(message) { }
    public ConsensusFailedException(string message, Exception innerException) : base(message, innerException) { }
}