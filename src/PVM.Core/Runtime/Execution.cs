using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using log4net;
using PVM.Core.Data;
using PVM.Core.Definition;
using PVM.Core.Plan;

namespace PVM.Core.Runtime
{
    public class Execution : IInternalExecution
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
        public IDictionary<string, object> Data { get; private set; }

        public void Proceed()
        {
            RequireActive();

            IList<Transition> eligibleNodes = CurrentNode.OutgoingTransitions.Count > 1
                ? CurrentNode.OutgoingTransitions.Where(t => t.IsDefault).ToList()
                : CurrentNode.OutgoingTransitions.ToList();

            if (!eligibleNodes.Any())
            {
                throw new InvalidOperationException(
                    string.Format("There are no eligible nodes to take in node '{0}'",
                        CurrentNode.Name));
            }

            if (eligibleNodes.Count > 1)
            {
                throw new InvalidOperationException(
                    string.Format("Cannot take default node since there are '{0}' eligible nodes",
                        CurrentNode.OutgoingTransitions.Count));
            }
            Logger.InfoFormat("Executing node '{0}'", CurrentNode.Name);
            var transition = eligibleNodes.First();

            Execute("Default", transition);
        }

        public void Proceed(string transitionName)
        {
            RequireActive();

            Logger.InfoFormat("Executing node '{0}'", CurrentNode.Name);
            var transition = CurrentNode.OutgoingTransitions.SingleOrDefault(t => t.Identifier == transitionName);

            Execute(transitionName, transition);
        }

        public void Resume(INode node)
        {
            if (!IsActive)
            {
                Logger.InfoFormat("Resuming execution '{0}'.", Identifier);
                IsActive = true;
                executionPlan.OnExecutionResuming(this);
                Proceed(node);
            }
        }
        public void Resume()
        {
            Resume(CurrentNode);
        }

        public void Stop()
        {
            if (IsActive)
            {
                Logger.InfoFormat("Execution '{0}' stopped.", Identifier);
                IsActive = false;
                executionPlan.OnExecutionStopped(this);
            }
        }

        public void Start(INode startNode, IDictionary<string, object> data)
        {
            if (!IsActive)
            {
                Logger.InfoFormat("Execution '{0}' started.", Identifier);
                CurrentNode = startNode;
                Data = data;
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

            child.Start(startNode, Data);
        }

        public void Accept(IExecutionVisitor visitor)
        {
            visitor.Visit(this);
            foreach (var child in Children)
            {
                child.Accept(visitor);
            }
        }

        public void Proceed([CanBeNull] INode node)
        {
            RequireActive();

            if (node == null)
            {
                Logger.InfoFormat("Node is null. Execution '{0}' stopping...", Identifier);
                Stop();
            }
            else
            {
                CurrentNode = node;
                Proceed();
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