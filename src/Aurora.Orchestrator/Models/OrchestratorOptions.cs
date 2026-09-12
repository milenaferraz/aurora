using System;
using System.Collections.Generic;

namespace Aurora.Orchestrator.Models
{
    /// <summary>
    /// Configuration options for the Aurora Orchestrator.
    /// </>
    public class OrchestratorOptions
    {
        /// <summary>
        /// Maximum number of concurrent workflows the orchestrator can handle.
        /// </summary>
        public int MaxConcurrentWorkflows { get; set; } = 10;

        /// <summary>
        /// Default timeout for individual workflow executions (in seconds).
        /// </summary>
        public int DefaultWorkflowTimeoutSeconds { get; set; } = 300;

        /// <summary>
        /// Whether to enable automatic task decomposition.
        /// </summary>
        public bool EnableTaskDecomposition { get; set; } = true;

        /// <summary>
        /// Whether to enable automatic agent selection.
        /// </summary>
        public bool EnableAgentSelection { get; set; } = true;

        /// <summary>
        /// Whether to enable automatic result aggregation.
        /// </summary>
        public bool EnableResultAggregation { get; set; } = true;

        /// <summary>
        /// Custom settings for task decomposition strategies.
        /// </summary>
        public Dictionary<string, object>? TaskDecompositionSettings { get; set; }

        /// <summary>
        /// Custom settings for agent selection strategies.
        /// </summary>
        public Dictionary<string, object>? AgentSelectionSettings { get; set; }

        /// <summary>
        /// Custom settings for result aggregation strategies.
        /// </summary>
        public Dictionary<string, object>? ResultAggregationSettings { get; set; }
    }
}
