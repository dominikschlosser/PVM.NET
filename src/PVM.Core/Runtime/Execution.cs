#region License

// -------------------------------------------------------------------------------
//  <copyright file="Execution.cs" company="PVM.NET Project Contributors">
//    Copyright (c) 2015 PVM.NET Project Contributors
//    Authors: Dominik Schlosser (dominik.schlosser@gmail.com)
//            
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//  </copyright>
// -------------------------------------------------------------------------------

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Internal;
using JetBrains.Annotations;
using log4net;
using PVM.Core.Definition;
using PVM.Core.Runtime.Algorithms;
using PVM.Core.Runtime.Plan;

namespace PVM.Core.Runtime
{
    public class Execution : IExecution
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (Execution));
        private readonly IExecutionPlan executionPlan;
        private readonly IWorkflowDefinition workflowDefinition;

        public Execution(string identifier, string workflowInstanceIdentifier, IWorkflowDefinition workflowDefinition, IExecutionPlan executionPlan)
        {
            Identifier = identifier;
            WorkflowInstanceIdentifier = workflowInstanceIdentifier;
            Children = new List<IExecution>();
            this.executionPlan = executionPlan;
            this.workflowDefinition = workflowDefinition;
            Data = new Dictionary<string, object>();
        }

        // TODO: builder
        public Execution(IExecution parent, INode currentNode, bool isActive, bool isFinished,
            IDictionary<string, object> data, string incomingTransition, string identifier, string workflowInstanceIdentifier, IExecutionPlan executionPlan,
            IList<IExecution> children, IWorkflowDefinition workflowDefinition)
            : this(identifier, workflowInstanceIdentifier, workflowDefinition, executionPlan)
        {
            Parent = parent;
            IncomingTransition = incomingTransition;
            CurrentNode = currentNode;
            IsActive = isActive;
            Data = data;
            Children = children;
            IsFinished = isFinished;
        }

        public string WorkflowInstanceIdentifier { get; private set; }
        public IExecution Parent { get; protected set; }
        public IList<IExecution> Children { get; protected set; }
        public INode CurrentNode { get; private set; }
        public string IncomingTransition { get; protected set; }

        public IExecutionPlan Plan
        {
            get { return executionPlan; }
        }

        public bool IsFinished { get; protected set; }
        public string Identifier { get; protected set; }
        public bool IsActive { get; protected set; }
        public IDictionary<string, object> Data { get; protected set; }

        public IWorkflowDefinition WorkflowDefinition
        {
            get { return workflowDefinition; }
        }

        public void Proceed()
        {
            RequireActive();

            IList<Transition> eligibleNodes = CurrentNode.OutgoingTransitions.Count() > 1
                ? CurrentNode.OutgoingTransitions.Where(t => t.IsDefault).ToList()
                : CurrentNode.OutgoingTransitions.ToList();

            if (!eligibleNodes.Any())
            {
                throw new InvalidOperationException(
                    string.Format("There are no eligible nodes to take in node '{0}'",
                        CurrentNode.Identifier));
            }

            if (eligibleNodes.Count > 1)
            {
                throw new InvalidOperationException(
                    string.Format("Cannot take default node since there are '{0}' eligible nodes",
                        CurrentNode.OutgoingTransitions.Count()));
            }
            Logger.InfoFormat("Executing node '{0}'", CurrentNode.Identifier);
            Transition transition = eligibleNodes.First();

            Execute("Default", transition);
        }

        public void Proceed(string transitionName)
        {
            RequireActive();

            Logger.InfoFormat("Executing node '{0}'", CurrentNode.Identifier);
            Transition transition = CurrentNode.OutgoingTransitions.SingleOrDefault(t => t.Identifier == transitionName);

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

        public void Split(INode node)
        {
            Stop();

            if (node.OutgoingTransitions.Count() == 1)
            {
                Resume(node);
                return;
            }

            Children.ForEach(c => c.Kill());

            var children = new List<IExecution>();

            foreach (var outgoingTransition in node.OutgoingTransitions)
            {
                var outgoingNode = outgoingTransition.Destination;
                var child = new Execution(this, outgoingNode, true, false, Data, outgoingTransition.Identifier,
                    Guid.NewGuid() + "_" + outgoingNode.Identifier, WorkflowInstanceIdentifier, executionPlan, new List<IExecution>(),
                    workflowDefinition);
                Children.Add(child);
                children.Add(child);
            }


            foreach (var outgoing in children)
            {
                Logger.InfoFormat("Child-Execution '{0}' started.", outgoing.Identifier);

                executionPlan.OnExecutionStarting(outgoing);
                outgoing.CurrentNode.Execute(outgoing, executionPlan);
            }
        }

        public void Accept(IExecutionVisitor visitor)
        {
            visitor.Visit(this);
            foreach (IExecution child in Children)
            {
                child.Accept(visitor);
            }
        }

        public void Proceed([CanBeNull] INode currentNode)
        {
            RequireActive();

            if (currentNode == null)
            {
                Logger.InfoFormat("Node is null. Execution '{0}' stopping...", Identifier);
                Stop();
            }
            else
            {
                CurrentNode = currentNode;
                Proceed();
            }
        }

        public void Wait()
        {
            Logger.InfoFormat("Execution '{0}' is reaching wait state", Identifier);
            Stop();
            executionPlan.OnExecutionReachesWaitState(this);
        }

        public void Signal()
        {
            Logger.InfoFormat("Signaling Execution '{0}'", Identifier);
            executionPlan.OnExecutionSignaled(this);
        }

        public void Kill()
        {
            if (IsFinished)
            {
                return;
            }

            IsFinished = true;
            IsActive = false;
            Logger.InfoFormat("Execution '{0}' was killed.", Identifier);

            foreach (var child in Children)
            {
                child.Kill();
            }
        }

        public IExecution GetConcurrentRoot()
        {
            if (Parent == null || Parent.IsFinished)
            {
                return this;
            }

            return Parent.GetConcurrentRoot();
        }

        private void Execute(string transitionIdentifier, Transition transition)
        {
            if (transition == null)
            {
                executionPlan.OnOutgoingTransitionIsNull(this, transitionIdentifier);
                return;
            }

            Logger.InfoFormat("Taking transition with name '{0}' to node '{1}'", transition.Identifier,
                transition.Destination.Identifier);

            IncomingTransition = transition.Identifier;
            CurrentNode = transition.Destination;
            CurrentNode.Execute(this, executionPlan);
        }

        private void RequireActive()
        {
            if (IsFinished)
            {
                throw new ExecutionInactiveException(string.Format("Execution '{0}' is finished.", Identifier));
            }

            if (!IsActive)
            {
                throw new ExecutionInactiveException(string.Format("Execution '{0}' is inactive.", Identifier));
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
                Plan.OnExecutionStarting(this);
                CurrentNode.Execute(this, Plan);
            }
        }

        protected bool Equals(Execution other)
        {
            return string.Equals(Identifier, other.Identifier);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((Execution) obj);
        }

        public override int GetHashCode()
        {
            return (Identifier != null ? Identifier.GetHashCode() : 0);
        }
    }
}