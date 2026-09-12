using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Orchestrator.Interfaces;

namespace Aurora.Orchestrator.Services
{
    /// <summary>
    /// Default implementation of IResultAggregator that combines results simply.
    /// </summary>
    public class DefaultResultAggregator : IResultAggregator
    {
        public Task<OrchestrationResult> AggregateAsync(TaskDefinition taskDefinition, IReadOnlyList<AgentExecutionResult> agentResults, CancellationToken cancellationToken = default)
        {
            var successfulResults = agentResults.Where(r => r.Success).Select(r => r.Result).ToList();
            var anyFailed = agentResults.Any(r => !r.Success);
            
            var result = new OrchestrationResult
            {
                TaskId = taskDefinition.Id,
                Success = !anyFailed,
                Result = successfulResults.Count > 0 ? successfulResults : null,
                ErrorMessage = anyFailed ? "Some subtasks failed" : null,
                StartedAt = agentResults.Min(r => r.StartedAt),
                CompletedAt = agentResults.Max(r => r.CompletedAt),
                Metadata = new Dictionary<string, object>
                {
                    { "SubtaskCount", agentResults.Count },
                    { "SuccessfulExecutions", successfulResults.Count },
                    { "FailedExecutions", agentResults.Count - successfulResults.Count }
                }
            };
            
            return Task.FromResult(result);
        }
    }
}
