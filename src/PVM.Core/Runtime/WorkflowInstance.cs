#region License

// -------------------------------------------------------------------------------
//  <copyright file="WorkflowInstance.cs" company="PVM.NET Project Contributors">
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

using System.Collections.Generic;
using log4net;
using PVM.Core.Definition;

namespace PVM.Core.Runtime
{
    public class WorkflowInstance : Execution, IWorkflowInstance
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (WorkflowInstance));

        public WorkflowInstance(string identifier, IWorkflowDefinition definition, IExecutionPlan executionPlan) : base(identifier, executionPlan)
        {
            Definition = definition;
        }

        public WorkflowInstance(IExecution parent, INode currentNode, bool isActive, bool isFinished, IDictionary<string, object> data, string incomingTransition, string identifier, IExecutionPlan executionPlan, IList<IExecution> children)
            : base(parent, currentNode, isActive, isFinished, data, incomingTransition, identifier, executionPlan, children)
        {
        }

        public IWorkflowDefinition Definition { get; private set; }

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
    }
}