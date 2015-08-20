using System.Collections.Generic;

namespace PVM.Core.Definition
{
	public class WorkflowDefinition
	{
		public WorkflowDefinition(string identifier, INode initialNode, IEnumerable<INode> nodes, IEnumerable<INode> endNodes)
		{
			InitialNode = initialNode;
			Nodes = nodes;
			EndNodes = endNodes;
		    Identifier = identifier;
		}

		public IEnumerable<INode> Nodes { get; private set; }
		public IEnumerable<INode> EndNodes { get; private set; }
		public INode InitialNode { get; private set; }
        public string Identifier { get; private set; }
	}
}