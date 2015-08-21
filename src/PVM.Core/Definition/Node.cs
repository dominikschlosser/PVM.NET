using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using PVM.Core.Data;
using PVM.Core.Plan;
using PVM.Core.Plan.Operations;
using PVM.Core.Runtime;

namespace PVM.Core.Definition
{
    public interface INode<T> where T : IProcessData<T>
    {
        IList<Transition<T>> IncomingTransitions { get; }
        IList<Transition<T>> OutgoingTransitions { get; }
        string Name { get; }
        void Execute(IExecution<T> execution, IExecutionPlan<T> executionPlan);
    }


    public class Node<T> : INode<T> where T : IProcessData<T>
    {
        private readonly IBehavior<T> behavior;

        public Node(string name, [CanBeNull] IBehavior<T> behavior)
        {
            Name = name;
            IncomingTransitions = new List<Transition<T>>();
            OutgoingTransitions = new List<Transition<T>>();
            this.behavior = behavior;
        }

        public IList<Transition<T>> IncomingTransitions { get; private set; }
        public IList<Transition<T>> OutgoingTransitions { get; private set; }
        public string Name { get; private set; }

        public void Execute(IExecution<T> execution, IExecutionPlan<T> executionPlan)
        {
            IOperation<T> operation = behavior == null ? new TransientOperation<T>() : behavior.CreateOperation(this);

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