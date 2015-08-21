using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using PVM.Core.Data;
using PVM.Core.Definition;
using PVM.Core.Plan;

namespace PVM.Core.Runtime
{
    public class Execution<T> : IExecution<T> where T : IProcessData<T>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Execution<T>));
        private readonly IExecutionPlan<T> executionPlan;

        public Execution(string identifier, IExecutionPlan<T> executionPlan)
        {
            Identifier = identifier;
            Children = new List<IExecution<T>>();
            this.executionPlan = executionPlan;
        }

        public Execution(IExecution<T> parent, string identifier, IExecutionPlan<T> executionPlan)
            : this(identifier, executionPlan)
        {
            Parent = parent;
        }

        public IExecution<T> Parent { get; private set; }
        public T Data { get; private set; }
        public IList<IExecution<T>> Children { get; private set; }
        public INode<T> CurrentNode { get; private set; }
        public string Identifier { get; private set; }
        public bool IsActive { get; private set; }

        public void Proceed()
        {
            RequireActive();

            Logger.InfoFormat("Executing node '{0}'", CurrentNode.Name);
            Transition<T> transition = CurrentNode.OutgoingTransitions.FirstOrDefault();

            Execute("Default", transition);
        }

        public void Proceed(string transitionName)
        {
            RequireActive();

            Logger.InfoFormat("Executing node '{0}'", CurrentNode.Name);
            Transition<T> transition = CurrentNode.OutgoingTransitions.SingleOrDefault(t => t.Identifier == transitionName);

            Execute(transitionName, transition);
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

        public void Start(INode<T> startNode, T data)
        {
            if (!IsActive)
            {
                Logger.InfoFormat("Execution '{0}' started. (Data: {1})", Identifier, data);
                CurrentNode = startNode;
                Data = data;
                IsActive = true;
                executionPlan.OnExecutionStarting(this);
                CurrentNode.Execute(this, executionPlan);
            }
        }

        public void CreateChild(INode<T> startNode)
        {
            Stop();
            var child = new Execution<T>(this, Guid.NewGuid() + "_" + startNode.Name, executionPlan);
            Children.Add(child);

            child.Start(startNode, Data.Copy());
        }

        public void Accept(IExecutionVisitor<T> visitor)
        {
            visitor.Visit(this);
            foreach (IExecution<T> child in Children)
            {
                child.Accept(visitor);
            }
        }

        private void Execute(string transitionIdentifier, Transition<T> transition)
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