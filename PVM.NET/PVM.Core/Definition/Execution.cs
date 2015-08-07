using System.Linq;
using PVM.Core.Definition.Exception;

namespace PVM.Core.Definition
{
    public class Execution : IExecution
    {
        private Node currentNode;
        public bool IsActive { get; } = true;

        public void Proceed(string transitionName)
        {
            if (currentNode == null)
            {
                throw new ExecutionBrokenException("Current node is null");
            }

            var transition = currentNode.OutgoingTransitions.SingleOrDefault(t => t.Name == transitionName);
            if (transition == null)
            {
                throw new TransitionNotFoundException($"Transition with name {transitionName} was not found");
            }

            currentNode = transition.Destination;
            if (currentNode == null)
            {
                throw new ExecutionBrokenException($"Destination node of transition \"{transitionName}\" is null");
            }

            currentNode.Execute(this);
        }
    }
}