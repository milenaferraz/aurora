using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aurora.Orchestrator.Agents
{
    /// <summary>
    /// Defines the contract for all agents in the Aurora system.
    /// </summary>
    public interface IAgent
    {
        /// <summary>
        /// Gets the unique identifier for this agent type.
        /// </summary>
        string AgentType { get; }

        /// <summary>
        /// Gets a human-readable name for this agent.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets a description of what this agent does.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets the capabilities that this agent provides.
        /// </summary>
        IReadOnlyCollection<string> Capabilities { get; }

        /// <summary>
        /// Executes the agent's logic for the given task.
        /// </summary>
        /// <param name="task">The task to execute.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The result of the agent's execution.</returns>
        Task<AgentExecutionResult> ExecuteAsync(AgentTask task, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// Represents a task to be executed by an agent.
    /// </summary>
    public class AgentTask
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Description { get; init; } = string.Empty;
        public Dictionary<string, object>? InputParameters { get; init; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
        public string? CorrelationId { get; init; }
    }

    /// <summary>
    /// Represents the result of an agent execution.
    /// </summary>
    public class AgentExecutionResult
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
    /// Base class for agents that provides common functionality.
    /// </summary>
    public abstract class AgentBase : IAgent
    {
        protected readonly ILogger<AgentBase> _logger;

        protected AgentBase(ILogger<AgentBase> logger)
        {
            _logger = logger;
        }

        public abstract string AgentType { get; }
        public abstract string Name { get; }
        public abstract string Description { get; }
        public abstract IReadOnlyCollection<string> Capabilities { get; }

        public virtual async Task<AgentExecutionResult> ExecuteAsync(AgentTask task, CancellationToken cancellationToken = default)
        {
            var startTime = DateTime.UtcNow;
            _logger.LogInformation("Agent {AgentType} starting task {TaskId}: {Description}", 
                AgentType, task.Id, task.Description);

            try
            {
                var result = await ExecuteInternalAsync(task, cancellationToken);
                
                var completionTime = DateTime.UtcNow;
                _logger.LogInformation("Agent {AgentType} completed task {TaskId} in {Duration}ms with success: {Success}", 
                    AgentType, task.Id, (completionTime - startTime).TotalMilliseconds, result.Success);

                return result with {
                    TaskId = task.Id,
                    StartedAt = startTime,
                    CompletedAt = completionTime
                };
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Agent {AgentType} task {TaskId} was cancelled.", AgentType, task.Id);
                return new AgentExecutionResult
                {
                    TaskId = task.Id,
                    Success = false,
                    ErrorMessage = "Task was cancelled.",
                    StartedAt = startTime,
                    CompletedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Agent {AgentType} task {TaskId} failed with exception.", AgentType, task.Id);
                return new AgentExecutionResult
                {
                    TaskId = task.Id,
                    Success = false,
                    ErrorMessage = ex.Message,
                    StartedAt = startTime,
                    CompletedAt = DateTime.UtcNow
                };
            }
        }

        /// <summary>
        /// Internal method that contains the agent's specific logic.
        /// </summary>
        /// <param name="task">The task to execute.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The result of the agent's execution.</returns>
        protected abstract Task<AgentExecutionResult> ExecuteInternalAsync(AgentTask task, CancellationToken cancellationToken = default);
    }
}
