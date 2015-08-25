using System;
using System.Collections.Generic;
using PVM.Core.Definition;
using PVM.Core.Plan.Operations;
using PVM.Core.Plan.Operations.Base;

namespace PVM.Core.Builder
{
    public class NodeBuilder
    {
        private readonly WorkflowDefinitionBuilder parentWorkflowBuilder;
        private readonly List<TransitionData> transitions = new List<TransitionData>();
        private bool isEndNode;
        private bool isStartNode;
        private string name = Guid.NewGuid().ToString();
        private IOperation operation = new TakeDefaultTransitionOperation(); 

        public NodeBuilder(WorkflowDefinitionBuilder parentWorkflowBuilder)
        {
            this.parentWorkflowBuilder = parentWorkflowBuilder;
        }

        public NodeBuilder WithOperation(IOperation operation)
        {
            this.operation = operation;

            return this;
        }

        public NodeBuilder WithName(string name)
        {
            this.name = name;

            foreach (var transition in transitions)
            {
                transition.Source = name;
            }
            return this;
        }

        public TransitionBuilder AddTransition()
        {
            return new TransitionBuilder(this, name);
        }

        internal void AddTransition(TransitionData data)
        {
            if (!transitions.Contains(data))
            {
                transitions.Add(data);
            }
        }

        public NodeBuilder IsStartNode()
        {
            isStartNode = true;

            return this;
        }

        public NodeBuilder IsEndNode()
        {
            isEndNode = true;

            return this;
        }

        public IWorkflowPathBuilder BuildNode(Func<string, INode> nodeFactory)
        {
            parentWorkflowBuilder.AddNode(nodeFactory(name), isStartNode, isEndNode, transitions);

            return parentWorkflowBuilder;
        }

        public IWorkflowPathBuilder BuildNode()
        {
            return BuildNode(n => new Node(n, operation));
        }

        public IWorkflowPathBuilder BuildSubWorkflow(WorkflowDefinitionBuilder subWorkflowDefinition)
        {
            return BuildNode(n => subWorkflowDefinition.WithIdentifier(n).BuildWorkflow());
        }

        public IWorkflowPathBuilder BuildParallelGateway()
        {
            return BuildNode(n => new Node(n, new ParallelGatewayOperation()));
        }

        public IWorkflowPathBuilder BuildParallelSplit()
        {
            return BuildNode(n => new Node(n, new ParallelSplitOperation()));
        }

        public IWorkflowPathBuilder BuildParallelJoin()
        {
            return BuildNode(n => new Node(n, new ParallelJoinOperation()));
        }
    }
}