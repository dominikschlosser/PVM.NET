using System;
using log4net;
using PVM.Core.Definition;
using PVM.Core.Plan.Operations;
using PVM.Core.Runtime;

namespace PVM.Core.Plan
{
    public class ExecutionPlan : IExecutionPlan
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (ExecutionPlan));
        private readonly IExecution rootExecution;

        public ExecutionPlan(WorkflowDefinition definition)
        {
            rootExecution = new Execution(Guid.NewGuid() + "_" + definition.InitialNode.Name, this);
        }

        public void Start(INode startNode)
        {
            rootExecution.Start(startNode);
        }

        public void Proceed(INode node, IOperation operation)
        {
            var finder = new ExecutionFinder(node);
            rootExecution.Accept(finder);

            if (finder.FoundExecution == null)
            {
                throw new InvalidExecutionStateException($"There is no execution with active node '{node.Name}'.");
            }

            operation.Execute(finder.FoundExecution);
        }

        private class ExecutionFinder : IExecutionVisitor
        {
            private readonly INode node;

            public ExecutionFinder(INode node)
            {
                this.node = node;
            }

            public IExecution FoundExecution { get; private set; }

            public void Visit(IExecution execution)
            {
                if (execution.CurrentNode == node)
                {
                    if (FoundExecution != null)
                    {
                        throw new InvalidExecutionStateException($"Node '{node.Name}' found in at least two executions.");
                    }
                    FoundExecution = execution;
                }
            }
        }
    }
}