// -------------------------------------------------------------------------------
//  <copyright file="WorkflowDefinitionModel.cs" company="PVM.NET Project Contributors">
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

using System.Collections.Generic;
using System.Linq;
using PVM.Core.Definition;

namespace PVM.Persistence.Sql.Model
{
    public class WorkflowDefinitionModel : NodeModel
    {
        public virtual IList<NodeModel> Nodes { get; set; }
        public virtual IList<NodeModel> EndNodes { get; set; }

        public static WorkflowDefinitionModel FromWorkflowDefinition(IWorkflowDefinition workflowDefinition)
        {
            return new WorkflowDefinitionModel
            {
                Identifier = workflowDefinition.Identifier,
                OperationType = workflowDefinition.OperationType,
                IncomingTransitions =
                    workflowDefinition.IncomingTransitions.Select(TransitionModel.FromTransition).ToList(),
                OutgoingTransitions =
                    workflowDefinition.OutgoingTransitions.Select(TransitionModel.FromTransition).ToList(),
                Nodes = workflowDefinition.Nodes.Select(FromNode).ToList(),
                EndNodes = workflowDefinition.Nodes.Select(FromNode).ToList(),
            };
        }
    }
}