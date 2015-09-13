#region License

// -------------------------------------------------------------------------------
//  <copyright file="WorkflowDefinitionTransformer.cs" company="PVM.NET Project Contributors">
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
using PVM.Core.Builder;
using PVM.Core.Definition;
using PVM.Persistence.Sql.Model;

namespace PVM.Persistence.Sql.Transform
{
    public class WorkflowDefinitionTransformer : IWorkflowDefinitionTransformer
    {
        public WorkflowDefinitionModel Transform(IWorkflowDefinition workflowDefinition)
        {
            var workflowDefinitionModel = new WorkflowDefinitionModel
            {
                Identifier = workflowDefinition.Identifier,
                OperationType = workflowDefinition.Operation.AssemblyQualifiedName
            };

            foreach (var node in workflowDefinition.Nodes)
            {
                NodeModel nodeModel = CreateNode(workflowDefinition, (dynamic) node);
                workflowDefinitionModel.Nodes.Add(nodeModel);
            }

            return workflowDefinitionModel;
        }

        private NodeModel CreateNode(IWorkflowDefinition parentWorkflow, INode node)
        {
            return new NodeModel
            {
                Identifier = node.Identifier,
                IsInitialNode = Equals(parentWorkflow.InitialNode, node),
                IsEndNode = parentWorkflow.EndNodes.Contains(node),
                OperationType = node.Operation.AssemblyQualifiedName,
                OutgoingTransitions = CreateOutgoingTransitions(node).ToList()
            };
        }

        private NodeModel CreateNode(IWorkflowDefinition parentWorkflow, IWorkflowDefinition nestedWorkflow)
        {
            return Transform(nestedWorkflow);
        }

        private IEnumerable<TransitionModel> CreateOutgoingTransitions(INode node)
        {
            foreach (var transition in node.OutgoingTransitions)
            {
                yield return new TransitionModel
                {
                    Identifier = transition.Identifier,
                    Source = transition.Source.Identifier,
                    Destination = transition.Destination.Identifier,
                    IsDefault = transition.IsDefault
                };
            }
        }

        public IWorkflowDefinition TransformBack(WorkflowDefinitionModel model)
        {
            var builder = new WorkflowDefinitionBuilder();
            return BuildWorkflowDefinition(builder, model).BuildWorkflow();
        }

        private WorkflowDefinitionBuilder BuildWorkflowDefinition(WorkflowDefinitionBuilder builder, WorkflowDefinitionModel model)
        {
            builder.WithIdentifier(model.Identifier);
            foreach (var node in model.Nodes)
            {
                NodeBuilder nodeBuilder = builder.AddNode()
                                                 .WithName(node.Identifier);

                if (node.OperationType != null)
                {
                    nodeBuilder.WithOperation(Type.GetType(node.OperationType));
                }

                if (node.IsInitialNode)
                {
                    nodeBuilder.IsStartNode();
                }

                if (node.IsEndNode)
                {
                    nodeBuilder.IsEndNode();
                }

                foreach (var transition in node.OutgoingTransitions)
                {
                    TransitionBuilder transitionBuilder = nodeBuilder.AddTransition()
                        .WithName(transition.Identifier)
                        .To(transition.Destination);

                    if (transition.IsDefault)
                    {
                        transitionBuilder.IsDefault();
                    }

                    transitionBuilder.BuildTransition();
                }

                BuildNode(builder, nodeBuilder, (dynamic) node);
            }

            return builder;
        }
        private void BuildNode(WorkflowDefinitionBuilder workflowBuilder, NodeBuilder nodeBuilder, NodeModel node)
        {
            nodeBuilder.BuildNode();
        }

        private void BuildNode(WorkflowDefinitionBuilder workflowBuilder, NodeBuilder nodeBuilder, WorkflowDefinitionModel node)
        {
            nodeBuilder.BuildSubWorkflow(BuildWorkflowDefinition(workflowBuilder, node));
        }
    }
}