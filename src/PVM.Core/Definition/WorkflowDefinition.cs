using System;
using System.Collections.Generic;
using PVM.Core.Data;

namespace PVM.Core.Definition
{
    public class WorkflowDefinition
    {
        public WorkflowDefinition(string identifier, INode initialNode, IDataMapper dataMapper, IEnumerable<INode> nodes,
            IEnumerable<INode> endNodes)
        {
            DataMapper = dataMapper;
            InitialNode = initialNode;
            Nodes = nodes;
            EndNodes = endNodes;
            Identifier = identifier;
        }

        public IEnumerable<INode> Nodes { get; private set; }
        public IEnumerable<INode> EndNodes { get; private set; }
        public INode InitialNode { get; private set; }
        public string Identifier { get; private set; }
        public IDataMapper DataMapper { get; private set; }

        public class Builder
        {
            private readonly List<INode> endNodes = new List<INode>();
            private readonly List<INode> nodes = new List<INode>();
            private IDataMapper dataMapper = new DefaultDataMapper();
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

            public Builder WithDataMapper(IDataMapper dataMapper)
            {
                this.dataMapper = dataMapper;

                return this;
            }

            public WorkflowDefinition Build()
            {
                return new WorkflowDefinition(identifier, initialNode, dataMapper, nodes, endNodes);
            }
        }
    }
}