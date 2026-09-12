using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Orchestrator.Interfaces;

namespace Aurora.Orchestrator.Services
{
    /// <summary>
    /// Default implementation of IAgentSelector that assigns all tasks to a generic agent.
    /// </summary>
    public class DefaultAgentSelector : IAgentSelector
    {
        public Task<AgentAssignment> SelectAgentAsync(TaskDefinition subtask, Dictionary<string, object> availableAgents, CancellationToken cancellationToken = default)
        {
            var assignment = new AgentAssignment
            {
                Subtask = subtask,
                AgentType = "DefaultAgent",
                AgentId = "default-agent-001",
                InputParameters = subtask.InputParameters
            };
            
            return Task.FromResult(assignment);
        }
    }
}
