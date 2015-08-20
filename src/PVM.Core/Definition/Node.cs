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
        void Execute(IExecutionPlan executionPlan);
    }

    public class Node : INode
    {
        private readonly IBehavior behavior;

        public Node(string name, [CanBeNull] IBehavior behavior)
        {
            Name = name;
            this.behavior = behavior;
        }

        public IList<Transition> IncomingTransitions { get; } = new List<Transition>();
        public IList<Transition> OutgoingTransitions { get; } = new List<Transition>();
        public string Name { get; }

        public void Execute(IExecutionPlan executionPlan)
        {
            if (behavior == null)
            {
                executionPlan.Proceed(this, new TransientOperation());
            }
            else
            {
                behavior.Execute(this, executionPlan);
            }
        }
    }
}