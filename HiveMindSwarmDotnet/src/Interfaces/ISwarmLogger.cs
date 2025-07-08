using HiveMindSwarmDotnet.Console.Models;

namespace HiveMindSwarmDotnet.Console.Interfaces;

public interface ISwarmLogger
{
    Task LogTaskStartAsync(SwarmTask task);
    Task LogTaskCompletedAsync(SwarmTask task, SwarmResult result);
    Task LogAgentResponseAsync(AgentResponse response);
    Task LogCommunicationAsync(AgentMessage message);
    Task LogErrorAsync(string error, Exception? exception = null);
}