#region License

// -------------------------------------------------------------------------------
//  <copyright file="WorkflowDefinition.cs" company="PVM.NET Project Contributors">
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
using PVM.Core.Runtime.Operations.Base;

namespace PVM.Core.Definition
{
    public interface IWorkflowDefinition : INode
    {
        IList<INode> Nodes { get; }
        IList<INode> EndNodes { get; }
        INode InitialNode { get; }
    }

    public class WorkflowDefinition : Node, IWorkflowDefinition
    {
        public WorkflowDefinition(string identifier, IList<INode> nodes, IList<INode> endNodes, INode initialNode)
            : base(identifier, typeof (TakeDefaultTransitionOperation))
        {
            Nodes = nodes;
            EndNodes = endNodes;
            InitialNode = initialNode;
        }


        public IList<INode> Nodes { get; private set; }
        public IList<INode> EndNodes { get; private set; }
        public INode InitialNode { get; private set; }

        public class Builder
        {
            private readonly List<INode> nodes = new List<INode>();
            private readonly List<INode> endNodes = new List<INode>();
            private INode initialNode;
            private string identifier = Guid.NewGuid().ToString();

            public Builder WithNodes(IEnumerable<INode> nodes)
            {
                this.nodes.AddRange(nodes);

                return this;
            }
            
            public Builder WithEndNodes(IEnumerable<INode> nodes)
            {
                this.endNodes.AddRange(nodes);

                return this;
            }

            public Builder WithIdentifier(string identifier)
            {
                this.identifier = identifier;

                return this;
            }

            public Builder WithInitialNode(INode node)
            {
                initialNode = node;
                return this;
            }
            public WorkflowDefinition Build()
            {
                return new WorkflowDefinition(identifier, nodes, endNodes, initialNode);
            }
        }

        public void AddStartTransition()
        {
            if (InitialNode != null)
            {
                base.AddOutgoingTransition(new Transition(Identifier + "_startTransition", true, this, InitialNode));
            }
        }

        public override void AddOutgoingTransition(Transition transition)
        {
            foreach (INode node in EndNodes)
            {
                node.AddOutgoingTransition(new Transition(transition.Identifier, transition.IsDefault, node, transition.Destination));
            }
        }
    }
}