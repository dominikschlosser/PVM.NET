using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using PVM.Core.Definition;
using PVM.Core.Plan.Operations;
using PVM.Core.Plan.Operations.Base;
using PVM.Core.Runtime;

namespace PVM.Core.Plan
{
    public class ExecutionPlan : IExecutionPlan
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (ExecutionPlan));
        private readonly IExecution rootExecution;
        private readonly WorkflowDefinition workflowDefinition;

        public ExecutionPlan(WorkflowDefinition workflowDefinition)
        {
            this.workflowDefinition = workflowDefinition;
            rootExecution = new Execution(Guid.NewGuid() + "_" + workflowDefinition.InitialNode.Name, this, workflowDefinition.DataMapper);
        }

        public void Start(INode startNode, IDictionary<string, object> data)
        {
            rootExecution.Start(startNode, data);
        }

        public void OnExecutionStarting(Execution execution)
        {
        }

        public void OnExecutionStopped(Execution execution)
        {
            var activeExecutions = GetActiveExecutions(execution);
            if (activeExecutions.Any())
            {
                Logger.InfoFormat("Execution '{0}' stopped but the following are still active: '{1}'",
                    execution.Identifier,
                    activeExecutions.Select(e => e.Identifier).Aggregate((e1, e2) => e1 + ", " + e2));
            }
            else if (!execution.CurrentNode.OutgoingTransitions.Any())
            {
                IsFinished = true;
                Logger.InfoFormat("Workflow instance with definition '{0}' ended", workflowDefinition.Identifier);
            }
        }

        public void OnOutgoingTransitionIsNull(Execution execution, string transitionIdentifier)
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

        public bool IsFinished { get; private set; }

        public void OnExecutionResuming(Execution execution)
        {
        }

        public void Proceed(IInternalExecution execution, IOperation operation)
        {
            if (workflowDefinition.EndNodes.Contains(execution.CurrentNode))
            {
                execution.Stop();
            }
            else
            {
                var genericOperationInterface = operation.GetType()
                                                         .GetInterfaces()
                                                         .FirstOrDefault(
                                                             i =>
                                                                 i.IsGenericType &&
                                                                 i.GetGenericTypeDefinition() == typeof (IOperation<>));
                if (
                    genericOperationInterface != null)
                {
                    var genericType = genericOperationInterface.GetGenericArguments().First();
                    var dataContext = Activator.CreateInstance(genericType);

                    workflowDefinition.DataMapper.MapData(dataContext, execution.Data);

                    operation.GetType().GetMethod("Execute", new[] {typeof (IExecution), genericType})
                             .Invoke(operation, new[] {execution, dataContext});
                }
                else
                {
                    operation.Execute(execution);
                }
            }
        }

        private IList<IExecution> GetActiveExecutions(IExecution execution)
        {
            var root = FindRoot(execution);
            var results = new List<IExecution>();
            root.Accept(new ExecutionVisitor(e =>
            {
                if (e.IsActive)
                {
                    results.Add(e);
                }
            }));

            return results;
        }

        private IExecution FindRoot(IExecution execution)
        {
            if (execution.Parent == null)
            {
                return execution;
            }

            return FindRoot(execution.Parent);
        }
    }
}