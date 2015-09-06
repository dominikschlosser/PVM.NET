#region License

// -------------------------------------------------------------------------------
//  <copyright file="WorkflowEngine.cs" company="PVM.NET Project Contributors">
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
using Microsoft.Practices.ServiceLocation;
using PVM.Core.Definition;
using PVM.Core.Persistence;
using PVM.Core.Tasks;

namespace PVM.Core.Runtime
{
    public class WorkflowEngine : IDisposable
    {
        private IServiceLocator serviceLocator;
        private readonly IPersistenceProvider persistenceProvider;

        public WorkflowEngine(IServiceLocator serviceLocator)
        {
            this.serviceLocator = serviceLocator;
            this.persistenceProvider = serviceLocator.GetInstance<IPersistenceProvider>();
        }

        public void Dispose()
        {
            if (serviceLocator != null)
            {
                var disposable = serviceLocator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }

                serviceLocator = null;
            }
        }

        public void RegisterNewWorkflowDefinition(IWorkflowDefinition definition)
        {
            persistenceProvider.Persist(definition);
        }


        public WorkflowInstance CreateNewInstance(IWorkflowDefinition definition)
        {
            RegisterNewWorkflowDefinition(definition);
            return CreateNewInstance(definition.Identifier);
        }

        public WorkflowInstance CreateNewInstance(string workflowDefinitionIdentifier)
        {
            IWorkflowDefinition workflowDefinition = persistenceProvider.LoadWorkflowDefinition(workflowDefinitionIdentifier);
            if (workflowDefinition == null)
            {
                throw new InvalidOperationException(string.Format("Workflow definition with identifier '{0}' not found", workflowDefinitionIdentifier));
            }
            return new WorkflowInstance(workflowDefinition, serviceLocator);
        }

        public void Complete(UserTask task)
        {
            IExecution execution = persistenceProvider.LoadExecution(task.ExecutionIdentifier);
            if (execution == null)
            {
                throw new InvalidOperationException(string.Format("Execution with identifier '{0}' not found", task.ExecutionIdentifier));
            }

            execution.Signal();
        }
    }
}