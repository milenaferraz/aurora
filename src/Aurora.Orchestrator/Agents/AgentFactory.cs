using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aurora.Orchestrator.Agents
{
    /// <summary>
    /// Factory for creating and managing agent instances.
    /// </summary>
    public class AgentFactory : IAgentFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<AgentFactory> _logger;
        private readonly ConcurrentDictionary<string, Lazy<IAgent>> _agentCache = new();

        public AgentFactory(IServiceProvider serviceProvider, ILogger<AgentFactory> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public IAgent GetAgent(string agentType)
        {
            if (string.IsNullOrWhiteSpace(agentType))
                throw new ArgumentException("Agent type cannot be null or empty.", nameof(agentType));

            return _agentCache.GetOrAdd(agentType.ToUpperInvariant(), 
                key => new Lazy<IAgent>(() => 
                {
                    // Try to get the agent from the service provider
                    var serviceType = Type.GetType($"Aurora.Orchestrator.Agents.{agentType}Agent, Aurora.Orchestrator");
                    if (serviceType != null && typeof(IAgent).IsAssignableFrom(serviceType))
                    {
                        var agent = (IAgent)_serviceProvider.GetService(serviceType);
                        if (agent != null)
                        {
                            _logger.LogInformation("Resolved agent of type {AgentType} from service provider.", agentType);
                            return agent;
                        }
                    }

                    // Fallback to known agent types
                    return agentType.ToUpperInvariant() switch
                    {
                        "DEVOPS" => new DevOpsAgent(_serviceProvider.GetRequiredService<ILogger<DevOpsAgent>>()),
                        "SECURITY" => new SecurityAgent(_serviceProvider.GetRequiredService<ILogger<SecurityAgent>>()),
                        "QA" => new QAAgent(_serviceProvider.GetRequiredService<ILogger<QAAgent>>()),
                        _ => throw new NotSupportedException($"Agent type '{agentType}' is not supported.")
                    };
                })).Value;
        }

        public IReadOnlyCollection<string> GetAvailableAgentTypes()
        {
            // Return the hardcoded list for now - in a real system this would be dynamically discovered
            return new List<string> { "DevOps", "Security", "QA" };
        }
    }

    /// <summary>
    /// Defines the contract for the agent factory.
    /// </summary>
    public interface IAgentFactory
    {
        /// <summary>
        /// Gets an agent of the specified type.
        /// </summary>
        /// <param name="agentType">The type of agent to retrieve.</param>
        /// <returns>An agent instance.</returns>
        IAgent GetAgent(string agentType);

        /// <summary>
        /// Gets a list of all available agent types.
        /// </returns>
        IReadOnlyCollection<string> GetAvailableAgentTypes();
    }
}
