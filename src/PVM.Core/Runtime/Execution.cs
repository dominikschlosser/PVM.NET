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
            this.executionPlan = executionPlan;
        }

        public Execution(IExecution parent, string identifier, IExecutionPlan executionPlan)
            : this(identifier, executionPlan)
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
            Transition transition = CurrentNode.OutgoingTransitions.FirstOrDefault();

            Execute("Default", transition);
        }

        public void Proceed(string transitionName)
        {
            RequireActive();

            Logger.InfoFormat("Executing node '{0}'", CurrentNode.Name);
            Transition transition = CurrentNode.OutgoingTransitions.SingleOrDefault(t => t.Identifier == transitionName);

            Execute(transitionName, transition);
        }

        public void Stop()
        {
            if (IsActive)
            {
                Logger.InfoFormat("Execution '{0}' ended.", Identifier);
                IsActive = false;
                executionPlan.OnExecutionStopped(this);
            }
        }

        public void Start(INode startNode)
        {
            if (!IsActive)
            {
                Logger.InfoFormat("Execution '{0}' started.", Identifier);
                CurrentNode = startNode;
                IsActive = true;
                executionPlan.OnExecutionStarting(this);
                CurrentNode.Execute(this, executionPlan);
            }
        }

        public void CreateChild(INode startNode)
        {
            Stop();
            var child = new Execution(this, Guid.NewGuid() + "_" + startNode.Name, executionPlan);
            Children.Add(child);

            child.Start(startNode);
        }

        public void Accept(IExecutionVisitor visitor)
        {
            visitor.Visit(this);
            foreach (IExecution child in Children)
            {
                child.Accept(visitor);
            }
        }

        private void Execute(string transitionIdentifier, Transition transition)
        {
            if (transition == null)
            {
                executionPlan.OnOutgoingTransitionIsNull(this, transitionIdentifier);
                return;
            }

            Logger.InfoFormat("Taking transition with name '{0}' to node '{1}'", transition.Identifier,
                transition.Destination.Name);

            transition.Executed = true;

            CurrentNode = transition.Destination;
            CurrentNode.Execute(this, executionPlan);
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