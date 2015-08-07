using System.Collections.Generic;

namespace PVM.Core.Definition.Nodes
{
    public interface INode
    {
        IEnumerable<Transition> IncomingTransitions { get; }
        IEnumerable<Transition> OutgoingTransitions { get; }
        string Name { get; }
        void Execute(IExecution execution);
    }

    public class Node : INode
    {
        private readonly IExecutable executable;

        public Node(string name, IExecutable executable, IEnumerable<Transition> incomingTransitions,
            IEnumerable<Transition> outgoingTransitions)
        {
            IncomingTransitions = incomingTransitions;
            OutgoingTransitions = outgoingTransitions;
            Name = name;
            this.executable = executable;
        }

        public IEnumerable<Transition> IncomingTransitions { get; private set; }
        public IEnumerable<Transition> OutgoingTransitions { get; private set; }
        public string Name { get; private set; }

        public void Execute(IExecution execution)
        {
            executable.Execute(execution);
        }
    }
}