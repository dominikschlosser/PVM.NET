using System;
using System.Collections.Generic;
using PVM.Core.Data;
using PVM.Core.Definition.Nodes;

namespace PVM.Core.Builder
{
    public class NodeBuilder<T> where T : ICopyable<T>
    {
        private readonly WorkflowDefinitionBuilder<T> parentWorkflowBuilder;
        private readonly List<TransitionData> transitions = new List<TransitionData>();
        private bool isEndNode;
        private bool isStartNode;
        private string name = Guid.NewGuid().ToString();

        public NodeBuilder(WorkflowDefinitionBuilder<T> parentWorkflowBuilder)
        {
            this.parentWorkflowBuilder = parentWorkflowBuilder;
        }

        public NodeBuilder<T> WithName(string name)
        {
            this.name = name;

            foreach (var transition in transitions)
            {
                transition.Source = name;
            }
            return this;
        }

        public TransitionBuilder<T> AddTransition()
        {
            return new TransitionBuilder<T>(this, name);
        }

        internal void AddTransition(TransitionData data)
        {
            if (!transitions.Contains(data))
            {
                transitions.Add(data);
            }
        }

        public NodeBuilder<T> IsStartNode()
        {
            isStartNode = true;

            return this;
        }

        public NodeBuilder<T> IsEndNode()
        {
            isEndNode = true;

            return this;
        }

        public IWorkflowPathBuilder<T> BuildNode(Func<string, INode<T>> nodeFactory)
        {
            parentWorkflowBuilder.AddNode(nodeFactory(name), isStartNode, isEndNode, transitions);

            return parentWorkflowBuilder;
        } 
        public IWorkflowPathBuilder<T> BuildNode()
        {
            return BuildNode(n => new Node<T>(n));
        }

        public IWorkflowPathBuilder<T> BuildParallelGateway()
        {
            return BuildNode(n => new ParallelGatewayNode<T>(n));
        }
    }
}