using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using PVM.Core.Definition;
using PVM.Core.Plan;

namespace PVM.Core.Runtime
{
    public class Execution : IExecution
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (Execution));
        private readonly IExecutionPlan executionPlan;

        public Execution(string identifier, IExecutionPlan executionPlan)
        {
            Identifier = identifier;
            Children = new List<IExecution>();
            IsActive = true;
            this.executionPlan = executionPlan;
        }

        public Execution(IExecution parent, string identifier, IExecutionPlan executionPlan) : this(identifier, executionPlan)
        {
            Parent = parent;
        }

        public IExecution Parent { get; private set; }
        public IList<IExecution> Children { get; private set; }
        public INode CurrentNode { get; private set; }
        public string Identifier { get; private set; }
        public bool IsActive { get; private set; }

        public void Proceed()
        {
            RequireActive();

            Logger.InfoFormat("Executing node '{0}'", CurrentNode.Name);
            var transition = CurrentNode.OutgoingTransitions.FirstOrDefault();
            if (transition == null)
            {
                Stop();
                return;
            }

            Logger.InfoFormat(
                "Taking default-transition with name '{0}' to node '{1}'", transition.Identifier, transition.Destination.Name);
            CurrentNode = transition.Destination;
            CurrentNode.Execute(executionPlan);
        }

        public void Proceed(string transitionName)
        {
            RequireActive();

            Logger.InfoFormat("Executing node '{0}'", CurrentNode.Name);
            var transition = CurrentNode.OutgoingTransitions.SingleOrDefault(t => t.Identifier == transitionName);
            if (transition == null)
            {
                throw new TransitionNotFoundException(string.Format(
                    "Outgoing transition with name '{0}' not found for node {1}", transitionName, CurrentNode.Name));
            }

            Logger.InfoFormat("Taking transition with name '{0}' to node '{1}'", transition.Identifier, transition.Destination.Name);

            CurrentNode = transition.Destination;
            CurrentNode.Execute(executionPlan);
        }

        public void Stop()
        {
            Logger.InfoFormat("Execution '{0}' ended.", Identifier);
            IsActive = false;
        }

        public void Accept(IExecutionVisitor visitor)
        {
            visitor.Visit(this);
            foreach (var child in Children)
            {
                child.Accept(visitor);
            }
        }

        public void Start(INode startNode)
        {
            CurrentNode = startNode;
            IsActive = true;
            CurrentNode.Execute(executionPlan);
        }

        public void CreateChild(INode startNode)
        {
            Stop();
            var child = new Execution(this, Guid.NewGuid() + "_" + startNode.Name, executionPlan);
            Children.Add(child);

            child.Start(startNode);
        }

        private void RequireActive()
        {
            if (!IsActive)
            {
                throw new ExecutionInactiveException(string.Format("Execution '{0}' is inactive.", Identifier));
            }
        }
    }
}