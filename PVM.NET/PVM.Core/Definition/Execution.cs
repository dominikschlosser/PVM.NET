using System.Linq;
using PVM.Core.Definition.Exception;

namespace PVM.Core.Definition
{
    public class Execution : IExecution
    {
        public bool IsActive { get; } = true;
        public INode CurrentNode { get; private set; }

        public void Proceed(string transitionName)
        {
            if (CurrentNode == null)
            {
                throw new ExecutionBrokenException("Current node is null");
            }

            var transition = CurrentNode.OutgoingTransitions.SingleOrDefault(t => t.Name == transitionName);
            if (transition == null)
            {
                throw new TransitionNotFoundException($"Transition with name {transitionName} was not found");
            }

            CurrentNode = transition.Destination;
            if (CurrentNode == null)
            {
                throw new ExecutionBrokenException($"Destination node of transition \"{transitionName}\" is null");
            }

            CurrentNode.Execute(this);
        }
    }
}