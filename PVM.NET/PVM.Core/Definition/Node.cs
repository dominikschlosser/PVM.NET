using System.Collections.Generic;

namespace PVM.Core.Definition
{
    public class Node
    {
        private readonly IExecutable executable;

        public Node(IExecutable executable, IEnumerable<Transition> incomingTransitions,
            IEnumerable<Transition> outgoingTransitions)
        {
            IncomingTransitions = incomingTransitions;
            OutgoingTransitions = outgoingTransitions;
            this.executable = executable;
        }

        public IEnumerable<Transition> IncomingTransitions { get; private set; }
        public IEnumerable<Transition> OutgoingTransitions { get; private set; }

        public void Execute(IExecution execution)
        {
            executable.Execute(execution);
        }
    }
}