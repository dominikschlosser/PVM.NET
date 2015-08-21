using System.Collections.Generic;
using PVM.Core.Plan;
using PVM.Core.Plan.Operations;
using PVM.Core.Runtime;

namespace PVM.Core.Definition
{
    public interface INode<T>
    {
        IList<Transition<T>> IncomingTransitions { get; }
        IList<Transition<T>> OutgoingTransitions { get; }
        string Name { get; }
        void Execute(IExecution<T> execution, IExecutionPlan<T> executionPlan);
    }


    public class Node<T> : INode<T>
    {
        public Node(string name) : this(name, new TakeDefaultTransitionOperation<T>())
        {
            
        }

        public Node(string name, IOperation<T> operation)
        {
            this.operation = operation;
            Name = name;
            IncomingTransitions = new List<Transition<T>>();
            OutgoingTransitions = new List<Transition<T>>();
        }

        public IList<Transition<T>> IncomingTransitions { get; }
        public IList<Transition<T>> OutgoingTransitions { get; }
        public string Name { get; }
        private IOperation<T> operation;
         
        public virtual void Execute(IExecution<T> execution, IExecutionPlan<T> executionPlan)
        {
            executionPlan.Proceed(execution, operation);
        }

        protected bool Equals(Node<T> other)
        {
            return string.Equals(Name, other.Name);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Node<T>) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}