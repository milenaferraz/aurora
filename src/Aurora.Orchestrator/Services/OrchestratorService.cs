using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Aurora.Orchestrator.Interfaces;
using Aurora.Orchestrator.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Aurora.Orchestrator.Services
{
    /// <summary>
    /// Implements the Aurora Orchestrator, responsible for coordinating agent workflows.
    /// </summary>
    public class OrchestratorService : IOrchestrator
    {
        private readonly ILogger<OrchestratorService> _logger;
        private readonly OrchestratorOptions _options;
        private readonly ITaskDecomposer? _taskDecomposer;
        private readonly IAgentSelector? _agentSelector;
        private readonly IResultAggregator? _resultAggregator;
        
        // Track active workflows
        private readonly ConcurrentDictionary<Guid, Task<OrchestrationResult>> _activeWorkflows = new();
        private readonly ConcurrentDictionary<Guid, DateTime> _workflowStartTimes = new();
        
        // Counters for status tracking
        private int _completedWorkflows;
        private int _failedWorkflows;

        public OrchestratorService(
            ILogger<OrchestratorService> logger,
            IOptions<OrchestratorOptions> options,
            ITaskDecomposer? taskDecomposer = null,
            IAgentSelector? agentSelector = null,
            IResultAggregator? resultAggregator = null)
        {
            _logger = logger;
            _options = options.Value;
            _taskDecomposer = taskDecomposer;
            _agentSelector = agentSelector;
            _resultAggregator = resultAggregator;
            
            _logger.LogInformation("Aurora Orchestrator initialized with options: {Options}", _options);
        }

        public async Task<OrchestrationResult> ExecuteAsync(TaskDefinition taskDefinition, CancellationToken cancellationToken = default)
        {
            if (_activeWorkflows.Count >= _options.MaxConcurrentWorkflows)
            {
                throw new InvalidOperationException($"Maximum concurrent workflows ({_options.MaxConcurrentWorkflows}) exceeded.");
            }

            var workflowId = taskDefinition.Id;
            _logger.LogInformation("Starting workflow {WorkflowId} for task: {TaskDescription}", workflowId, taskDefinition.Description);
            
            var startTime = DateTime.UtcNow;
            _workflowStartTimes[workflowId] = startTime;
            
            // Create a task to execute the workflow
            var workflowTask = Task.Run(async () =>
            {
                try
                {
                    return await ExecuteWorkflowInternalAsync(taskDefinition, cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogWarning("Workflow {WorkflowId} was cancelled.", workflowId);
                    return new OrchestrationResult
                    {
                        TaskId = workflowId,
                        Success = false,
                        ErrorMessage = "Workflow was cancelled.",
                        StartedAt = startTime,
                        CompletedAt = DateTime.UtcNow
                    };
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Workflow {WorkflowId} failed with exception.", workflowId);
                    return new OrchestrationResult
                    {
                        TaskId = workflowId,
                        Success = false,
                        ErrorMessage = ex.Message,
                        StartedAt = startTime,
                        CompletedAt = DateTime.UtcNow
                    };
                }
                finally
                {
                    _activeWorkflows.TryRemove(workflowId, out _);
                    _workflowStartTimes.TryRemove(workflowId, out _);
                    
                    // Update counters
                    if (Interlocked.Exchange(ref _completedWorkflows, _completedWorkflows + 1) >= 0)
                    {
                        // Successfully completed
                    }
                    else
                    {
                        Interlocked.Increment(ref _failedWorkflows);
                    }
                }
            }, cancellationToken);

            _activeWorkflows[workflowId] = workflowTask;
            
            try
            {
                return await workflowTask;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Workflow {WorkflowId} was cancelled by caller.", workflowId);
                return new OrchestrationResult
                {
                    TaskId = workflowId,
                    Success = false,
                    ErrorMessage = "Workflow was cancelled by caller.",
                    StartedAt = startTime,
                    CompletedAt = DateTime.UtcNow
                };
            }
        }

        private async Task<OrchestrationResult> ExecuteWorkflowInternalAsync(TaskDefinition taskDefinition, CancellationToken cancellationToken)
        {
            var startTime = DateTime.UtcNow;
            
            // Step 1: Task Decomposition (if enabled)
            List<TaskDefinition> subtasks = new() { taskDefinition }; // Default: treat as single subtask
            if (_options.EnableTaskDecomposition && _taskDecomposer != null)
            {
                _logger.LogInformation("Decomposing task {TaskId}", taskDefinition.Id);
                var decomposed = await _taskDecomposer.DecomposeAsync(taskDefinition, cancellationToken);
                if (decomposed != null && decomposed.Count > 0)
                {
                    subtasks = new List<TaskDefinition>(decomposed);
                    _logger.LogInformation("Task {TaskId} decomposed into {Count} subtasks", taskDefinition.Id, subtasks.Count);
                }
            }

            // Step 2: Agent Selection and Execution
            List<AgentExecutionResult> agentResults = new();
            
            if (_options.EnableAgentSelection && _agentSelector != null)
            {
                // Select agents for each subtask
                var assignments = new List<AgentAssignment>();
                foreach (var subtask in subtasks)
                {
                    // In a real implementation, we would get available agents from a registry
                    var availableAgents = new Dictionary<string, object>
                    {
                        { "AvailableAt", DateTime.UtcNow },
                        { "Capabilities", new List<string> { "text-processing", "code-generation", "analysis" } }
                    };
                    
                    var assignment = await _agentSelector.SelectAgentAsync(subtask, availableAgents, cancellationToken);
                    assignments.Add(assignment);
                }
                
                // Execute assignments (simplified - in reality would dispatch to actual agents)
                foreach (var assignment in assignments)
                {
                    var agentStartTime = DateTime.UtcNow;
                    _logger.LogInformation("Executing assignment {AssignmentId} with agent {AgentType}", assignment.Id, assignment.AgentType);
                    
                    // Simulate agent execution - in reality this would call the actual agent
                    await Task.Delay(100, cancellationToken); // Simulate work
                    
                    // For now, create a mock successful result
                    agentResults.Add(new AgentExecutionResult
                    {
                        AssignmentId = assignment.Id,
                        Success = true,
                        Result = $"Result from {assignment.AgentType} for subtask: {assignment.Subtask.Description}",
                        StartedAt = agentStartTime,
                        CompletedAt = DateTime.UtcNow
                    });
                }
            }
            else
            {
                // Fallback: execute subtasks directly (simplified)
                foreach (var subtask in subtasks)
                {
                    var agentStartTime = DateTime.UtcNow;
                    await Task.Delay(50, cancellationToken); // Simulate work
                    
                    agentResults.Add(new AgentExecutionResult
                    {
                        Success = true,
                        Result = $"Direct execution of subtask: {subtask.Description}",
                        StartedAt = agentStartTime,
                        CompletedAt = DateTime.UtcNow
                    });
                }
            }

            // Step 3: Result Aggregation (if enabled)
            OrchestrationResult finalResult;
            if (_options.EnableResultAggregation && _resultAggregator != null && agentResults.Count > 0)
            {
                _logger.LogInformation("Aggregating results from {Count} agent executions", agentResults.Count);
                finalResult = await _resultAggregator.AggregateAsync(taskDefinition, agentResults, cancellationToken);
            }
            else
            {
                // Simple aggregation: combine all successful results
                var successfulResults = agentResults.Where(r => r.Success).Select(r => r.Result).ToList();
                var anyFailed = agentResults.Any(r => !r.Success);
                
                finalResult = new OrchestrationResult
                {
                    TaskId = taskDefinition.Id,
                    Success = !anyFailed,
                    Result = successfulResults.Count > 0 ? successfulResults : null,
                    ErrorMessage = anyFailed ? "Some subtasks failed" : null,
                    StartedAt = startTime,
                    CompletedAt = DateTime.UtcNow,
                    Metadata = new Dictionary<string, object>
                    {
                        { "SubtaskCount", subtasks.Count },
                        { "AgentExecutionCount", agentResults.Count },
                        { "SuccessfulExecutions", successfulResults.Count }
                    }
                };
            }

            _logger.LogInformation("Workflow {WorkflowId} completed in {Duration}ms with success: {Success}", 
                taskDefinition.Id, 
                (DateTime.UtcNow - startTime).TotalMilliseconds,
                finalResult.Success);
                
            return finalResult;
        }

        public async Task<OrchestratorStatus> GetStatusAsync(CancellationToken cancellationToken = default)
        {
            return new OrchestratorStatus
            {
                ActiveWorkflows = _activeWorkflows.Count,
                CompletedWorkflows = _completedWorkflows,
                FailedWorkflows = _failedWorkflows,
                LastUpdated = DateTime.UtcNow,
                Version = "1.0.0"
            };
        }
    }
}
