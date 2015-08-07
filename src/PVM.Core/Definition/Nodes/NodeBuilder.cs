using System.Collections.Generic;

namespace PVM.Core.Definition.Nodes
{
    public class NodeBuilder
    {
        private readonly List<Transition> incomingTransitions = new List<Transition>();
        private readonly List<Transition> outgoingTransitions = new List<Transition>();
        private IExecutable executable;
        private string name;

        public NodeBuilder WithIncomingTransition(Transition transition)
        {
            incomingTransitions.Add(transition);

            return this;
        }

        public NodeBuilder WithOutgoingTransition(Transition transition)
        {
            outgoingTransitions.Add(transition);

            return this;
        }

        public NodeBuilder WithExectuable(IExecutable executable)
        {
            this.executable = executable;

            return this;
        }

        public NodeBuilder WithName(string name)
        {
            this.name = name;

            return this;
        }

        public Node Build()
        {
            return new Node(name, executable, incomingTransitions, outgoingTransitions);
        }
    }
}