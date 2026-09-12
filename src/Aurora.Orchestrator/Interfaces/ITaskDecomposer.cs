using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aurora.Orchestrator.Interfaces
{
    /// <summary>
    /// Defines the contract for decomposing complex tasks into smaller, manageable subtasks.
    /// </summary>
    public interface ITaskDecomposer
    {
        /// <summary>
        /// Decomposes a task definition into a list of subtasks that can be executed in parallel or sequence.
        /// </summary>
        /// <param name="taskDefinition">The task to decompose.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>A list of subtask definitions.</returns>
        Task<IReadOnlyList<TaskDefinition>> DecomposeAsync(TaskDefinition taskDefinition, CancellationToken cancellationToken = default);
    }
}
