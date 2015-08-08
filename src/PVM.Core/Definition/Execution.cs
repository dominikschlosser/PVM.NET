using System.Linq;
using log4net;
using PVM.Core.Definition.Exception;
using PVM.Core.Definition.Nodes;
using PVM.Core.Utils;

namespace PVM.Core.Definition
{
    public class Execution : IExecution
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (Execution));
        private INode currentNode;

        public Execution(string identifier, INode startNode)
        {
            Identifier = identifier;
            currentNode = startNode;
        }

        public string Identifier { get; }
        public bool IsActive { get; private set; } = true;

        public void Proceed(string transitionName)
        {
            var transition = currentNode.OutgoingTransitions.SingleOrDefault(t => t.Name == transitionName);
            if (transition == null)
            {
                throw new TransitionNotFoundException(
                    $"Outgoing transition with name '{transitionName}' not found for node {currentNode.Name}");
            }

            currentNode = transition.Destination;
            Execute();
        }

        public void Proceed()
        {
            var transition = currentNode.OutgoingTransitions.FirstOrDefault();
            if (transition == null)
            {
                EndExecution();
                return;
            }

            currentNode = transition.Destination;
            Execute();
        }

        public void Start()
        {
            currentNode.RequireNotNull($"Start node is null in execution '{Identifier}'");
            Execute();
        }

        private void Execute()
        {
            this.RequireActive();
            Logger.Info($"Executing node '{currentNode.Name}'");
            currentNode.Execute(this);
        }

        private void EndExecution()
        {
            Logger.Info($"Execution '{Identifier}' ended.");
            IsActive = false;
        }
    }
}