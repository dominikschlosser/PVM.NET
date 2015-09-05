#region License

// -------------------------------------------------------------------------------
//  <copyright file="WorkflowDefinitionBuilder.cs" company="PVM.NET Project Contributors">
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
using log4net;
using PVM.Core.Definition;

namespace PVM.Core.Builder
{
    public class WorkflowDefinitionBuilder : IWorkflowPathBuilder
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (WorkflowDefinitionBuilder));
        private readonly IDictionary<string, INode> endNodes = new Dictionary<string, INode>();
        private readonly IDictionary<string, INode> nodes = new Dictionary<string, INode>();

        private readonly IDictionary<string, List<TransitionData>> transitions =
            new Dictionary<string, List<TransitionData>>();

        private string identifier = Guid.NewGuid().ToString();
        private INode startNode;

        public NodeBuilder AddNode()
        {
            return new NodeBuilder(this);
        }

        // keep type to add validation later
        public WorkflowDefinition BuildWorkflow<T>() where T : class
        {
            AssembleTransitions();

            return
                new WorkflowDefinition.Builder().WithIdentifier(identifier)
                                                   .WithInitialNode(startNode)
                                                   .WithNodes(nodes.Values)
                                                   .WithEndNodes(endNodes.Values)
                                                   .Build();
        }

        public WorkflowDefinition BuildWorkflow()
        {
            return BuildWorkflow<object>();
        }

        public WorkflowDefinitionBuilder AsDefinitionBuilder()
        {
            return this;
        }

        public IWorkflowPathBuilder WithIdentifier(string id)
        {
            identifier = id;

            return this;
        }

        private void AssembleTransitions()
        {
            foreach (var transition in transitions)
            {
                Logger.DebugFormat("Assembling transition from '{0}'", transition.Key);
                var sourceNode = nodes[transition.Key];

                foreach (var transitionData in transition.Value)
                {
                    var targetNode = nodes[transitionData.Target];
                    Logger.DebugFormat("  - Source: {0}, Target: {1}", sourceNode.Identifier, targetNode.Identifier);

                    var transitionToAdd = new Transition(transitionData.Name, transitionData.IsDefault, transitionData.Executed, sourceNode,
                        targetNode);
                    sourceNode.AddOutgoingTransition(transitionToAdd);
                    targetNode.AddIncomingTransition(transitionToAdd);
                }
            }
        }

        internal void AddNode(INode node, bool isStartNode, bool isEndNode, List<TransitionData> transitions)
        {
            nodes.Add(node.Identifier, node);

            if (isStartNode)
            {
                startNode = node;
            }

            if (isEndNode)
            {
                endNodes.Add(node.Identifier, node);
            }

            this.transitions[node.Identifier] = transitions;
        }
    }
}