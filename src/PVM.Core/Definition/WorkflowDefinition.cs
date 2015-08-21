using System.Collections.Generic;
using PVM.Core.Data;

namespace PVM.Core.Definition
{
	public class WorkflowDefinition<T> where T : IProcessData<T>
	{
		public WorkflowDefinition(string identifier, INode<T> initialNode, IEnumerable<INode<T>> nodes, IEnumerable<INode<T>> endNodes)
		{
			InitialNode = initialNode;
			Nodes = nodes;
			EndNodes = endNodes;
			Identifier = identifier;
		}

		public IEnumerable<INode<T>> Nodes { get; private set; }
		public IEnumerable<INode<T>> EndNodes { get; private set; }
		public INode<T> InitialNode { get; private set; }
		public string Identifier { get; private set; }
	}
}