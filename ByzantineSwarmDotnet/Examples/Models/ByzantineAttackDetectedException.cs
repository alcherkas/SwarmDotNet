namespace ByzantineSwarmDotnet.Examples.Models;

public class ByzantineAttackDetectedException : Exception
{
    public List<string> MaliciousAgents { get; }
    
    public ByzantineAttackDetectedException(List<string> maliciousAgents, string message) : base(message)
    {
        MaliciousAgents = maliciousAgents;
    }
}