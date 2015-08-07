using System.Collections.Generic;

namespace PVM.Core.Definition
{
    public class WorkflowDefinitionBuilder
    {
        private readonly List<INode> nodes = new List<INode>();
        private Node initialNode;

        public WorkflowDefinitionBuilder AddNode(INode node)
        {
            nodes.Add(node);

            return this;
        }

        public WorkflowDefinition Build()
        {
            return new WorkflowDefinition(initialNode, nodes);
        }
    }
}