using System.Collections.Generic;
using PVM.Core.Plan;
using PVM.Core.Plan.Operations.Base;
using PVM.Core.Runtime;

namespace PVM.Core.Definition
{
    public interface INode
    {
        IList<Transition> IncomingTransitions { get; }
        IList<Transition> OutgoingTransitions { get; }
        string Name { get; }
        void Execute(IInternalExecution execution, IExecutionPlan executionPlan);
    }


    public class Node : INode
    {
        public Node(string name) : this(name, new TakeDefaultTransitionOperation())
        {
            
        }

        public Node(string name, IOperation operation)
        {
            this.operation = operation;
            Name = name;
            IncomingTransitions = new List<Transition>();
            OutgoingTransitions = new List<Transition>();
        }

        public IList<Transition> IncomingTransitions { get; private set; }
        public IList<Transition> OutgoingTransitions { get; private set; }
        public string Name { get; private set; }
        private readonly IOperation operation;

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
            return Equals((Node)obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}