using System.Collections.Generic;

namespace PVM.Core.Definition.Nodes
{
    public interface INode
    {
        IList<Transition> IncomingTransitions { get; }
        IList<Transition> OutgoingTransitions { get; }
        string Name { get; }
        void Execute(IExecution execution);
    }

    public class Node : INode
    {
        private readonly IExecutable executable;

        public Node(string name, IExecutable executable)
        {
            Name = name;
            this.executable = executable;
        }

        public IList<Transition> IncomingTransitions { get; } = new List<Transition>();
        public IList<Transition> OutgoingTransitions { get; } = new List<Transition>();
        public string Name { get; }

        public void Execute(IExecution execution)
        {
            executable.Execute(execution);
        }
    }
}