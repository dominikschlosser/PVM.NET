using System;
using System.Collections.Generic;
using PVM.Core.Definition;
using PVM.Core.Definition.Executables;
using PVM.Core.Definition.Nodes;

namespace PVM.Core.Builder
{
    public class NodeBuilder
    {
        private readonly WorkflowDefinitionBuilder parentWorkflowBuilder;
        private readonly List<TransitionData> transitions = new List<TransitionData>();
        private IExecutable executable = new TransientExecutable();
        private bool isEndNode;
        private bool isStartNode;
        private string name = Guid.NewGuid().ToString();

        public NodeBuilder(WorkflowDefinitionBuilder parentWorkflowBuilder)
        {
            this.parentWorkflowBuilder = parentWorkflowBuilder;
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

        public NodeBuilder WithExecutable(IExecutable executable)
        {
            this.executable = executable;

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

        public IWorkflowPathBuilder BuildNode()
        {
            parentWorkflowBuilder.AddNode(new Node(name, executable), isStartNode, isEndNode, transitions);

            return parentWorkflowBuilder;
        }
    }
}