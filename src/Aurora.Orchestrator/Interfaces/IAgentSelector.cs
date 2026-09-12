using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aurora.Orchestrator.Interfaces
{
    /// <summary>
    /// Defines the contract for selecting appropriate agents to execute specific subtasks.
    /// </summary>
    public interface IAgentSelector
    {
        /// <summary>
        /// Selects the most suitable agent(s) for executing a given subtask.
        /// </summary>
        /// <param name="subtask">The subtask to be executed.</param>
        /// <param name="availableAgents">Information about available agents and their capabilities.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>Selected agent assignment for the subtask.</returns>
        Task<AgentAssignment> SelectAgentAsync(TaskDefinition subtask, Dictionary<string, object> availableAgents, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Represents an assignment of a subtask to a specific agent.
    /// </summary>
    public class AgentAssignment
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public TaskDefinition Subtask { get; init; } = null!;
        public string AgentType { get; init; } = string.Empty;
        public string AgentId { get; init; } = string.Empty;
        public Dictionary<string, object>? InputParameters { get; init; }
        public DateTime AssignedAt { get; init; } = DateTime.UtcNow;
    }
}
