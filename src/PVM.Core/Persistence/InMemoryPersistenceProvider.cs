#region License

// -------------------------------------------------------------------------------
//  <copyright file="NullPersistenceProvider.cs" company="PVM.NET Project Contributors">
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
using Microsoft.Practices.ServiceLocation;
using PVM.Core.Definition;
using PVM.Core.Runtime;

namespace PVM.Core.Persistence
{
    public class InMemoryPersistenceProvider : IPersistenceProvider
    {
        private readonly IDictionary<string, IWorkflowDefinition> workflowDefinitions = new Dictionary<string, IWorkflowDefinition>();
        private readonly IDictionary<string, IExecution> executions = new Dictionary<string, IExecution>(); 

        public void Persist(IExecution execution, IWorkflowDefinition definition)
        {
            if (executions.ContainsKey(execution.Identifier))
            {
                executions.Remove(execution.Identifier);
            }

            executions.Add(execution.Identifier, execution);
        }

        public void Persist(IWorkflowDefinition workflowDefinition)
        {
            if (workflowDefinitions.ContainsKey(workflowDefinition.Identifier))
            {
                workflowDefinitions.Remove(workflowDefinition.Identifier);
            }
            workflowDefinitions.Add(workflowDefinition.Identifier, workflowDefinition);
        }

        public IWorkflowDefinition LoadWorkflowDefinition(string workflowDefinitionIdentifier)
        {
            if (!workflowDefinitions.ContainsKey(workflowDefinitionIdentifier))
            {
                return null;
            }

            return workflowDefinitions[workflowDefinitionIdentifier];
        }

        public IExecution LoadExecution(string executionIdentifier, IExecutionPlan plan)
        {
            return executions[executionIdentifier];
        }

        public IWorkflowInstance LoadWorkflowInstance(string identifier, Func<IWorkflowDefinition, IExecutionPlan> executionPlanCreatorCallback)
        {
            return executions[identifier] as IWorkflowInstance;
        }

    }
}