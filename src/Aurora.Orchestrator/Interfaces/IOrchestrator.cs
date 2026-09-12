using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aurora.Orchestrator.Interfaces
{
    /// <summary>
    /// Defines the contract for the Aurora Orchestrator, responsible for coordinating agent workflows.
    /// </summary>
    public interface IOrchestrator
    {
        /// <summary>
        /// Executes a workflow by decomposing the task, selecting agents, executing them, and aggregating results.
        /// </summary>
        /// <param name="taskDefinition">The task to be executed.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The result of the orchestrated workflow.</returns>
        Task<OrchestrationResult> ExecuteAsync(TaskDefinition taskDefinition, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the current status of the orchestrator and any active workflows.
        /// </summary>
        /// <returns>Current orchestrator status.</returns>
        Task<OrchestratorStatus> GetStatusAsync(CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Represents the definition of a task to be orchestrated.
    /// </summary>
    public class TaskDefinition
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Description { get; init; } = string.Empty;
        public string? Context { get; init; }
        public Dictionary<string, object>? InputParameters { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public DateTime? Deadline { get; init; }
        public string? CorrelationId { get; init; }
    }

    /// <summary>
    /// Represents the result of an orchestrated workflow.
    /// </summary>
    public class OrchestrationResult
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public Guid TaskId { get; init; }
        public bool Success { get; init; }
        public object? Result { get; init; }
        public string? ErrorMessage { get; init; }
        public DateTime StartedAt { get; init; }
        public DateTime CompletedAt { get; init; }
        public TimeSpan Duration => CompletedAt - StartedAt;
        public Dictionary<string, object>? Metadata { get; init; }
    }

    /// <summary>
    /// Represents the current status of the orchestrator.
    /// </summary>
    public class OrchestratorStatus
    {
        public int ActiveWorkflows { get; init; }
        public int CompletedWorkflows { get; init; }
        public int FailedWorkflows { get; init; }
        public DateTime LastUpdated { get; init; } = DateTime.UtcNow;
        public string? Version { get; init; }
    }
}
