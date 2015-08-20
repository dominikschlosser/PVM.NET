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
            this.executionPlan = executionPlan;
        }

        public Execution(IExecution parent, string identifier, IExecutionPlan executionPlan)
        {
            Identifier = identifier;
            this.executionPlan = executionPlan;
            Parent = parent;
        }

        public IExecution Parent { get; }
        public IList<IExecution> Children { get; } = new List<IExecution>();
        public INode CurrentNode { get; private set; }
        public string Identifier { get; }
        public bool IsActive { get; private set; } = true;

        public void Proceed()
        {
            RequireActive();

            Logger.Info($"Executing node '{CurrentNode.Name}'");
            var transition = CurrentNode.OutgoingTransitions.FirstOrDefault();
            if (transition == null)
            {
                Stop();
                return;
            }

            Logger.Info(
                $"Taking default-transition with name '{transition.Identifier}' to node '{transition.Destination.Name}'");
            CurrentNode = transition.Destination;
            CurrentNode.Execute(executionPlan);
        }

        public void Proceed(string transitionName)
        {
            RequireActive();

            Logger.Info($"Executing node '{CurrentNode.Name}'");
            var transition = CurrentNode.OutgoingTransitions.SingleOrDefault(t => t.Identifier == transitionName);
            if (transition == null)
            {
                throw new TransitionNotFoundException(
                    $"Outgoing transition with name '{transitionName}' not found for node {CurrentNode.Name}");
            }

            Logger.Info($"Taking transition with name '{transition.Identifier}' to node '{transition.Destination.Name}'");

            CurrentNode = transition.Destination;
            CurrentNode.Execute(executionPlan);
        }

        public void Stop()
        {
            Logger.Info($"Execution '{Identifier}' ended.");
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
                throw new ExecutionInactiveException($"Execution '{Identifier}' is inactive.");
            }
        }
    }
}