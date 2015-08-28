// -------------------------------------------------------------------------------
//  <copyright file="WorkflowDefinition.cs" company="PVM.NET Project Contributors">
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
using PVM.Core.Plan.Operations;

namespace PVM.Core.Definition
{
    public interface IWorkflowDefinition : INode
    {
        IList<INode> Nodes { get; }
        IList<INode> EndNodes { get; }
        INode InitialNode { get; }
    }

    public class WorkflowDefinition<T> : Node, IWorkflowDefinition where T : class
    {
        public WorkflowDefinition(string identifier, INode initialNode, IList<INode> nodes,
            IList<INode> endNodes) : base(identifier, new StartSubProcessOperation())
        {
            InitialNode = initialNode;
            Nodes = nodes;
            EndNodes = endNodes;
        }

        public IList<INode> Nodes { get; private set; }
        public IList<INode> EndNodes { get; private set; }
        public INode InitialNode { get; private set; }

        public override void AddOutgoingTransition(Transition transition)
        {
            foreach (INode endNode in EndNodes)
            {
                endNode.AddOutgoingTransition(transition);
            }
        }

        public class Builder
        {
            private readonly List<INode> endNodes = new List<INode>();
            private readonly List<INode> nodes = new List<INode>();
            private string identifier = Guid.NewGuid().ToString();
            private INode initialNode;

            public Builder WithNodes(IEnumerable<INode> nodes)
            {
                this.nodes.AddRange(nodes);

                return this;
            }

            public Builder WithEndNodes(IEnumerable<INode> nodes)
            {
                endNodes.AddRange(nodes);

                return this;
            }

            public Builder WithInitialNode(INode initial)
            {
                initialNode = initial;

                return this;
            }

            public Builder WithIdentifier(string identifier)
            {
                this.identifier = identifier;

                return this;
            }

            public WorkflowDefinition<T> Build()
            {
                return new WorkflowDefinition<T>(identifier, initialNode, nodes, endNodes);
            }
        }
    }
}