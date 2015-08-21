using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using PVM.Core.Data;
using PVM.Core.Definition;
using PVM.Core.Plan.Operations;
using PVM.Core.Runtime;

namespace PVM.Core.Plan
{
    public class ExecutionPlan<T> : IExecutionPlan<T> where T : IProcessData<T>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(ExecutionPlan<T>));
        private readonly IExecution<T> rootExecution;
        private readonly WorkflowDefinition<T> workflowDefinition;

        public ExecutionPlan(WorkflowDefinition<T> workflowDefinition)
        {
            this.workflowDefinition = workflowDefinition;
            rootExecution = new Execution<T>(Guid.NewGuid() + "_" + workflowDefinition.InitialNode.Name, this);
        }

        public void Start(INode<T> startNode, T data)
        {
            rootExecution.Start(startNode, data);
        }

        public void OnExecutionStarting(Execution<T> execution)
        {
        }

        public void OnExecutionStopped(Execution<T> execution)
        {
            IList<IExecution<T>> activeExecutions = GetActiveExecutions(execution);
            if (activeExecutions.Any())
            {
                Logger.InfoFormat("Execution '{0}' stopped but the following are still active: '{1}'",
                    execution.Identifier,
                    activeExecutions.Select(e => e.Identifier).Aggregate((e1, e2) => e1 + ", " + e2));
            }
            else if (!execution.CurrentNode.OutgoingTransitions.Any())
            {
                Logger.InfoFormat("Workflow instance with definition '{0}' ended", workflowDefinition.Identifier);
            }
        }

        public void OnOutgoingTransitionIsNull(Execution<T> execution, string transitionIdentifier)
        {
            if (workflowDefinition.EndNodes.Contains(execution.CurrentNode))
            {
                Logger.InfoFormat("Execution '{0}' ended in null transition. Stopping...", execution.Identifier);
                execution.Stop();

                return;
            }

            throw new TransitionNotFoundException(string.Format(
                "Outgoing transition with name '{0}' not found for node {1}", transitionIdentifier,
                execution.CurrentNode.Name));
        }

        public void Proceed(IExecution<T> execution, IOperation<T> operation)
        {
            operation.Execute(execution);
        }

        private IList<IExecution<T>> GetActiveExecutions(IExecution<T> execution)
        {
            IExecution<T> root = FindRoot(execution);
            var results = new List<IExecution<T>>();
            root.Accept(new ExecutionVisitor<T>(e =>
            {
                if (e.IsActive)
                {
                    results.Add(e);
                }
            }));

            return results;
        }

        private IExecution<T> FindRoot(IExecution<T> execution)
        {
            if (execution.Parent == null)
            {
                return execution;
            }

            return FindRoot(execution.Parent);
        }
    }
}