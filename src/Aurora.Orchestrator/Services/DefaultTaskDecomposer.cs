using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Orchestrator.Interfaces;

namespace Aurora.Orchestrator.Services
{
    /// <summary>
    /// Default implementation of ITaskDecomposer that treats tasks as atomic (no decomposition).
    /// </summary>
    public class DefaultTaskDecomposer : ITaskDecomposer
    {
        public Task<IReadOnlyList<TaskDefinition>> DecomposeAsync(TaskDefinition taskDefinition, CancellationToken cancellationToken = default)
        {
            // Return the task as a single subtask - no decomposition
            return Task.FromResult<IReadOnlyList<TaskDefinition>>(new List<TaskDefinition> { taskDefinition });
        }
    }
}
