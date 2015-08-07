using System.Collections.Generic;

namespace PVM.Core.Definition
{
    public class WorkflowDefinitionBuilder
    {
        private readonly List<INode> nodes = new List<INode>();
        private INode initialNode;

        public WorkflowDefinitionBuilder AddNode(INode node)
        {
            nodes.Add(node);

            return this;
        }

        public WorkflowDefinitionBuilder AddInitialNode(INode node)
        {
            nodes.Add(node);
            initialNode = node;

            return this;
        }

        public WorkflowDefinition Build()
        {
            return new WorkflowDefinition(initialNode, nodes);
        }
    }
}