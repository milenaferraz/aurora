using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aurora.Orchestrator.Interfaces
{
    /// <summary>
    /// Defines the contract for aggregating results from multiple agent executions into a coherent result.
    /// </summary>
    public interface IResultAggregator
    {
        /// <summary>
        /// Aggregates the results from multiple agent executions into a single orchestration result.
        /// </summary>
        /// <param name="taskDefinition">The original task definition.</param>
        /// <param name="agentResults">The results from individual agent executions.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The aggregated orchestration result.</returns>
        Task<OrchestrationResult> AggregateAsync(TaskDefinition taskDefinition, IReadOnlyList<AgentExecutionResult> agentResults, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Represents the result of a single agent execution.
    /// </summary>
    public class AgentExecutionResult
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public Guid AssignmentId { get; init; }
        public bool Success { get; init; }
        public object? Result { get; init; }
        public string? ErrorMessage { get; init; }
        public DateTime StartedAt { get; init; }
        public DateTime CompletedAt { get; init; }
        public TimeSpan Duration => CompletedAt - StartedAt;
        public Dictionary<string, object>? Metadata { get; init; }
    }
}
