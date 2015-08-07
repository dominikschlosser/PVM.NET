using System.Collections.Generic;
using PVM.Core.Definition.Executables;

namespace PVM.Core.Definition.Nodes
{
	public class EndNode : Node
	{
		public EndNode(string name, IEnumerable<Transition> incomingTransitions)
			: base(name, new TransientExecutable(), incomingTransitions, new List<Transition>())
		{
		}
	}
}