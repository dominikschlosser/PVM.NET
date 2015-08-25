using System.Collections.Generic;
using PVM.Core.Plan;
using PVM.Core.Plan.Operations.Base;
using PVM.Core.Runtime;

namespace PVM.Core.Definition
{
    public interface INode
    {
        IEnumerable<Transition> IncomingTransitions { get; }
        IEnumerable<Transition> OutgoingTransitions { get; }
        string Name { get; }
        void AddOutgoingTransition(Transition transition);
        void AddIncomingTransition(Transition transition);
        void Execute(IInternalExecution execution, IExecutionPlan executionPlan);
    }


    public class Node : INode
    {
        private readonly List<Transition> incomingTransitions = new List<Transition>();
        private readonly IOperation operation;
        private readonly List<Transition> outgoingTransitions = new List<Transition>();

        public Node(string name) : this(name, new TakeDefaultTransitionOperation())
        {
        }

        public Node(string name, IOperation operation)
        {
            this.operation = operation;
            Name = name;
        }

        public IEnumerable<Transition> IncomingTransitions
        {
            get { return incomingTransitions; }
        }

        public IEnumerable<Transition> OutgoingTransitions
        {
            get { return outgoingTransitions; }
        }

        public virtual void AddOutgoingTransition(Transition transition)
        {
            outgoingTransitions.Add(transition);
        }

        public virtual void AddIncomingTransition(Transition transition)
        {
            incomingTransitions.Add(transition);
        }

        public string Name { get; private set; }

        public virtual void Execute(IInternalExecution execution, IExecutionPlan executionPlan)
        {
            executionPlan.Proceed(execution, operation);
        }

        protected bool Equals(Node other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Node) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}