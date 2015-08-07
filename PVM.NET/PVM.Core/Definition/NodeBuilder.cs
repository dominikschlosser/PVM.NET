using System.Collections.Generic;

namespace PVM.Core.Definition
{
    public class NodeBuilder
    {
        private readonly List<Transition> incomingTransitions = new List<Transition>();
        private readonly List<Transition> outgoingTransitions = new List<Transition>();
        private IExecutable executable;
        private bool IsInitialNode { get; set; }

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

        public NodeBuilder WithExectuable(IExecutable exectuable)
        {
            executable = exectuable;

            return this;
        }

        public NodeBuilder IsInitial()
        {
            IsInitialNode = true;

            return this;
        }

        public Node Build()
        {
            return new Node(executable, incomingTransitions, outgoingTransitions);
        }
    }
}