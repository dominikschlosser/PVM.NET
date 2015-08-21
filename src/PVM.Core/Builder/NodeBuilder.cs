using System;
using System.Collections.Generic;
using PVM.Core.Data;
using PVM.Core.Definition;
using PVM.Core.Runtime;

namespace PVM.Core.Builder
{
    public class NodeBuilder<T> where T : IProcessData<T>
    {
        private readonly WorkflowDefinitionBuilder<T> parentWorkflowBuilder;
        private readonly List<TransitionData> transitions = new List<TransitionData>();
        private IBehavior<T> behavior;
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

        public NodeBuilder<T> WithBehavior(IBehavior<T> withBehavior)
        {
            behavior = withBehavior;

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

        public IWorkflowPathBuilder<T> BuildNode()
        {
            parentWorkflowBuilder.AddNode(new Node<T>(name, behavior), isStartNode, isEndNode, transitions);

            return parentWorkflowBuilder;
        }
    }
}