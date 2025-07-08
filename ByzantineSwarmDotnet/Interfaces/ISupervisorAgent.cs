using ByzantineSwarmDotnet.Models;

namespace ByzantineSwarmDotnet.Interfaces;

public interface ISupervisorAgent : IEnhancedSwarmAgent
{
    Task<List<AgentResponse>> ExecuteSubtasksWithConsensusAsync(List<AgentTask> subtasks);
    Task<AgentTask> DecomposeTaskAsync(SwarmTask task);
    Task<AgentResponse> AggregateResponsesWithValidationAsync(List<AgentResponse> responses);
}