// -------------------------------------------------------------------------------
//  <copyright file="ExecutionPlan.cs" company="PVM.NET Project Contributors">
//    Copyright (c) 2015 PVM.NET Project Contributors
//    Authors: Dominik Schlosser (dominik.schlosser@gmail.com)
//            
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//    	http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
//  </copyright>
// -------------------------------------------------------------------------------

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

namespace PVM.Core.Plan
{
    public class ExecutionPlan : IExecutionPlan
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (ExecutionPlan));
        private readonly IWorkflowDefinition workflowDefinition;
        private readonly IServiceLocator serviceLocator;

        public ExecutionPlan(IWorkflowDefinition workflowDefinition, IServiceLocator serviceLocator)
        {
            this.workflowDefinition = workflowDefinition;
            this.serviceLocator = serviceLocator;
        }

        public void OnExecutionStarting(Execution execution)
        {
        }

        public void OnExecutionStopped(Execution execution)
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
                IsFinished = true;
                Logger.InfoFormat("Workflow instance with definition '{0}' ended", workflowDefinition.Identifier);
            }
        }

        public void OnOutgoingTransitionIsNull(Execution execution, string transitionIdentifier)
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

        public bool IsFinished { get; private set; }

        public void OnExecutionResuming(Execution execution)
        {
        }

        public void OnExecutionReachesWaitState(Execution execution)
        {
            serviceLocator.GetInstance<IPersistenceProvider>().Persist(execution);
        }

        public IWorkflowDefinition Definition
        {
            get { return workflowDefinition; }
        }

        public void Proceed(IExecution execution, string operationType)
        {
            if (workflowDefinition.EndNodes.Contains(execution.CurrentNode))
            {
                execution.Stop();
            }
            else
            {
                var operation = serviceLocator.GetInstance(Type.GetType(operationType)) as IOperation;
                if (operation == null)
                {
                    throw new InvalidOperationException(string.Format("Type '{0}' is not an operation", operationType));
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
            IExecution root = FindRoot(execution);
            var results = new List<IExecution>();
            root.Accept(new ExecutionVisitor(e =>
            {
                if (e.IsActive)
                {
                    results.Add(e);
                }
            }));

            return results;
        }

        private IExecution FindRoot(IExecution execution)
        {
            if (execution.Parent == null)
            {
                return execution;
            }

            return FindRoot(execution.Parent);
        }
    }
}