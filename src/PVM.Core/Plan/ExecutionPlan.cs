#region License

// -------------------------------------------------------------------------------
//  <copyright file="ExecutionPlan.cs" company="PVM.NET Project Contributors">
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
using log4net;
using Microsoft.Practices.ServiceLocation;
using PVM.Core.Data.Attributes;
using PVM.Core.Data.Proxy;
using PVM.Core.Definition;
using PVM.Core.Persistence;
using PVM.Core.Plan.Operations.Base;
using PVM.Core.Runtime;
using PVM.Core.Utils;

namespace PVM.Core.Plan
{
    public class ExecutionPlan : IExecutionPlan
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (ExecutionPlan));
        private readonly IServiceLocator serviceLocator;
        private readonly IWorkflowDefinition workflowDefinition;

        public ExecutionPlan(IWorkflowDefinition workflowDefinition, IServiceLocator serviceLocator)
        {
            this.workflowDefinition = workflowDefinition;
            this.serviceLocator = serviceLocator;
        }

        public void OnExecutionStarting(IExecution execution)
        {
        }

        public void OnExecutionStopped(IExecution execution)
        {
            IList<IExecution> activeExecutions = GetActiveExecutions(execution);
            if (activeExecutions.Any())
            {
                Logger.InfoFormat("Execution '{0}' stopped but the following are still active: '{1}'",
                    execution.Identifier,
                    activeExecutions.Select(e => e.Identifier).Aggregate((e1, e2) => e1 + ", " + e2));
            }
            else if (!execution.CurrentNode.OutgoingTransitions.Any())
            {
                execution.Kill();
                KillInactiveParentExecution(execution.Parent);
            }
            else
            {
                Logger.InfoFormat("Execution '{0}' stopped but has the following outgoing transitions: '{1}'",
                    execution.Identifier,
                    execution.CurrentNode.OutgoingTransitions.Select(t => t.Identifier).Aggregate((t1, t2) => t1 + ", " + t2));
            }
        }

        private void KillInactiveParentExecution(IExecution parent)
        {
            if (parent != null && !parent.IsActive)
            {
                parent.Kill();
                KillInactiveParentExecution(parent.Parent);
            }
        }

        public void OnOutgoingTransitionIsNull(IExecution execution, string transitionIdentifier)
        {
            if (workflowDefinition.EndNodes.Contains(execution.CurrentNode))
            {
                Logger.InfoFormat("Execution '{0}' ended in null transition. Stopping...", execution.Identifier);
                execution.Stop();

                return;
            }

            throw new TransitionNotFoundException(string.Format(
                "Outgoing transition with name '{0}' not found for node {1}", transitionIdentifier,
                execution.CurrentNode.Identifier));
        }

        public void OnExecutionResuming(IExecution execution)
        {
        }

        public void OnExecutionReachesWaitState(IExecution execution)
        {
            serviceLocator.GetInstance<IPersistenceProvider>().Persist(execution);
        }

        public void OnExecutionSignaled(IExecution execution)
        {
            execution.Resume();
        }

        public void Proceed(IExecution execution, INode node)
        {
            if (workflowDefinition.EndNodes.Contains(execution.CurrentNode))
            {
                execution.Stop();
            }
            else
            {
                IOperation operation = serviceLocator.GetInstance(node.Operation) as IOperation;

                if (operation == null)
                {
                    throw new InvalidOperationException(string.Format("'{0}' is not an operation", node.Operation.FullName));
                }

                Type genericOperationInterface = operation.GetType()
                                                          .GetInterfaces()
                                                          .FirstOrDefault(
                                                              i =>
                                                                  i.IsGenericType &&
                                                                  i.GetGenericTypeDefinition() == typeof (IOperation<>));
                if (
                    genericOperationInterface != null)
                {
                    Type genericType =
                        genericOperationInterface.GetGenericArguments()
                                                 .First(t => t.HasAttribute<WorkflowDataAttribute>());
                    object dataContext = DataMapper.CreateProxyFor(genericType, execution.Data);

                    operation.GetType().GetMethod("Execute", new[] {typeof (IExecution), genericType})
                             .Invoke(operation, new[] {execution, dataContext});
                }
                else
                {
                    operation.Execute(execution);
                }
            }
        }

        private IList<IExecution> GetActiveExecutions(IExecution execution)
        {
            var results = new List<IExecution>();
            execution.Accept(new ExecutionVisitor(e =>
            {
                if (e.IsActive)
                {
                    results.Add(e);
                }
            }));

            return results;
        }
    }
}