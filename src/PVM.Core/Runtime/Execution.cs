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
using JetBrains.Annotations;
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

        public IExecutionPlan Plan
        {
            get { return executionPlan; }
        }

        public bool IsFinished { get; private set; }

        public string Identifier { get; private set; }
        public bool IsActive { get; private set; }
        public IDictionary<string, object> Data { get; private set; }

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

        public void CreateChildren(IEnumerable<INode> nodes)
        {
            Stop();

            foreach (var node in nodes)
            {
                var child = new Execution(this, Guid.NewGuid() + "_" + node.Identifier, executionPlan);
                child.CurrentNode = node;
                child.IsActive = true;
                child.Data = Data;
                Children.Add(child);
            }

            foreach (var child in Children)
            {
                Logger.InfoFormat("Child-Execution '{0}' started.", child.Identifier);

                executionPlan.OnExecutionStarting(child);
                child.CurrentNode.Execute(child, executionPlan);
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

        private void Execute(string transitionIdentifier, Transition transition)
        {
            if (transition == null)
            {
                executionPlan.OnOutgoingTransitionIsNull(this, transitionIdentifier);
                return;
            }

            Logger.InfoFormat("Taking transition with name '{0}' to node '{1}'", transition.Identifier,
                transition.Destination.Identifier);

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

        public void Signal()
        {
            Logger.InfoFormat("Signaling Execution '{0}'", Identifier);
            executionPlan.OnExecutionSignaled(this);
        }

        public void Kill()
        {
            IsFinished = true;
            IsActive = false;
            Logger.InfoFormat("Execution '{0}' was killed.", Identifier);
        }
    }
}