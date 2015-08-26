using System;
using System.Collections.Generic;
using PVM.Core.Plan.Operations;
using PVM.Core.Runtime;

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
            Identifier = identifier;
        }

        public IList<INode> Nodes { get; private set; }
        public IList<INode> EndNodes { get; private set; }
        public INode InitialNode { get; private set; }
        public string Identifier { get; private set; }

        public override void AddOutgoingTransition(Transition transition)
        {
            foreach (var endNode in EndNodes)
            {
                endNode.AddOutgoingTransition(transition);
            }
        }

        public WorkflowInstance CreateNewInstance()
        {
            return new WorkflowInstance(this);
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