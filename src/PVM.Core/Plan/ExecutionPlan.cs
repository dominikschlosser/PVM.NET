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
            CheckIfEnded(execution);
        }

        private bool CheckIfEnded(IExecution execution)
        {
            if (workflowDefinition.EndNodes.Contains(execution.CurrentNode))
            {
                Logger.InfoFormat("Execution '{0}' ended", execution.Identifier);
                execution.Kill();

                return true;
            }

            return false;
        }

        public void OnOutgoingTransitionIsNull(IExecution execution, string transitionIdentifier)
        {
            if (!CheckIfEnded(execution))
            {
                throw new TransitionNotFoundException(string.Format(
                    "Outgoing transition with name '{0}' not found for node {1}", transitionIdentifier,
                    execution.CurrentNode.Identifier));
            }
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
                var operation = serviceLocator.GetInstance(node.Operation) as IOperation;

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
    }
}