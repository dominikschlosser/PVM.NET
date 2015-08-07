using System.Collections.Generic;
using PVM.Core.Definition.Nodes;

namespace PVM.Core.Definition
{
	public class WorkflowDefinition
	{
		public WorkflowDefinition(INode initialNode, IEnumerable<INode> nodes, IEnumerable<INode> endNodes)
		{
			InitialNode = initialNode;
			Nodes = nodes;
			EndNodes = endNodes;
		}

		public IEnumerable<INode> Nodes { get; private set; }
		public IEnumerable<INode> EndNodes { get; private set; }
		public INode InitialNode { get; private set; }
	}
}