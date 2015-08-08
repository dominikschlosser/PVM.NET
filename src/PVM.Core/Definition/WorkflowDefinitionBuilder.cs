using System;
using System.Collections.Generic;
using PVM.Core.Definition.Nodes;

namespace PVM.Core.Definition
{
	public class WorkflowDefinitionBuilder
	{
		private readonly List<INode> endNodes = new List<INode>();
		private readonly List<INode> nodes = new List<INode>();
		private INode initialNode;
	    private string identifier = Guid.NewGuid().ToString();

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

		public WorkflowDefinitionBuilder AddEndNode(INode endNode)
		{
			endNodes.Add(endNode);
			nodes.Add(endNode);

			return this;
		}

	    public WorkflowDefinitionBuilder WithIdentifier(string identifier)
	    {
	        this.identifier = identifier;

	        return this;
	    }

		public WorkflowDefinition Build()
		{
			return new WorkflowDefinition(identifier, initialNode, nodes, endNodes);
		}
	}
}