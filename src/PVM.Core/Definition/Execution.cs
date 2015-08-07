using System;
using System.Collections.Generic;
using System.Linq;
using PVM.Core.Definition.Exception;
using PVM.Core.Definition.Nodes;

namespace PVM.Core.Definition
{
	public class Execution : IExecution
	{
		private readonly IEnumerable<INode> endNodes;
		private bool active = true;
		private INode currentNode;

		public Execution(INode startNode, IEnumerable<INode> endNodes)
		{
			currentNode = startNode;
			this.endNodes = endNodes;
		}

		public void Proceed(string transitionName)
		{
			Proceed(n => n.OutgoingTransitions.SingleOrDefault(t => t.Name == transitionName));
		}

		public void Proceed()
		{
			Proceed(n => n.OutgoingTransitions.FirstOrDefault());
		}

		private void Proceed(Func<INode, Transition> transitionSelector)
		{
			if (currentNode == null)
			{
				throw new ExecutionBrokenException("Current node is null");
			}

            if (endNodes.Contains(currentNode))
            {
                active = false;
	            return;
            }

			var transition = transitionSelector(currentNode);
			if (transition == null)
			{
				throw new TransitionNotFoundException($"Outgoing transition not found for node {currentNode.Name}");
			}

			currentNode = transition.Destination;
            currentNode.Execute(this);
		}
	}
}