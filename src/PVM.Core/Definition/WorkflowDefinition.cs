using System.Collections.Generic;

namespace PVM.Core.Definition
{
    public class WorkflowDefinition
    {
        public WorkflowDefinition(INode initialNode, IEnumerable<INode> nodes)
        {
            InitialNode = initialNode;
            Nodes = nodes;
        }

        public IEnumerable<INode> Nodes { get; private set; }
        public INode InitialNode { get; private set; }
    }
}