using System.Collections.Generic;
using PVM.Core.Definition.Executions;

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
        private readonly IBehavior behavior;

        public Node(string name, IBehavior behavior)
        {
            Name = name;
            this.behavior = behavior;
        }

        public IList<Transition> IncomingTransitions { get; } = new List<Transition>();
        public IList<Transition> OutgoingTransitions { get; } = new List<Transition>();
        public string Name { get; }

        public void Execute(IExecution execution)
        {
            behavior.Execute(execution);
        }
    }
}