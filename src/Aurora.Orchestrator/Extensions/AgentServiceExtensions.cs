using System;
using Microsoft.Extensions.DependencyInjection;
using Aurora.Orchestrator.Agents;

namespace Aurora.Orchestrator.Extensions
{
    /// <summary>
    /// Extension methods for registering agent services.
    /// </summary>
    public static class AgentServiceExtensions
    {
        /// <summary>
        /// Adds agent services to the service collection.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <returns>The service collection for chaining.</returns>
        public static IServiceCollection AddAgentServices(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            // Register agent factory
            services.AddSingleton<IAgentFactory, AgentFactory>();

            // Register specific agents
            services.AddTransient<DevOpsAgent>();
            services.AddTransient<SecurityAgent>();
            services.AddTransient<QAAgent>();

            // Register loggers for agents
            services.AddLogging(builder => 
                builder.AddFilter("Aurora.Orchestrator.Agents", LogLevel.Information));

            return services;
        }
    }
}
