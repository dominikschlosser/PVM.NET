using System.Collections.Generic;
using JetBrains.Annotations;
using PVM.Core.Plan;
using PVM.Core.Plan.Operations;
using PVM.Core.Runtime;

namespace PVM.Core.Definition
{
    public interface INode
    {
        IList<Transition> IncomingTransitions { get; }
        IList<Transition> OutgoingTransitions { get; }
        string Name { get; }
        void Execute(IExecution execution, IExecutionPlan executionPlan);
    }

    public class Node : INode
    {
        private readonly IBehavior behavior;

        public Node(string name, [CanBeNull] IBehavior behavior)
        {
            Name = name;
            IncomingTransitions = new List<Transition>();
            OutgoingTransitions = new List<Transition>();
            this.behavior = behavior;
        }

        public IList<Transition> IncomingTransitions { get; private set; }
        public IList<Transition> OutgoingTransitions { get; private set; }
        public string Name { get; private set; }

        public void Execute(IExecution execution, IExecutionPlan executionPlan)
        {
            IOperation operation = behavior == null ? new TransientOperation() : behavior.CreateOperation(this);

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
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Node) obj);
        }

        public override int GetHashCode()
        {
            return (Name != null ? Name.GetHashCode() : 0);
        }
    }
}