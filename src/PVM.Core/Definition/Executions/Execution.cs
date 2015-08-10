using System.Collections.Generic;
using System.Linq;
using log4net;
using PVM.Core.Definition.Exceptions;
using PVM.Core.Definition.Nodes;
using PVM.Core.Utils;

namespace PVM.Core.Definition.Executions
{
    public class Execution : IExecution
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (Execution));
        private readonly List<Execution> childExecutions = new List<Execution>();

        public Execution(Execution parent, string identifier, INode startNode)
        {
            Parent = parent;
            Identifier = identifier;
            CurrentNode = startNode;
        }

        public Execution(string identifier, INode startNode)
        {
            Identifier = identifier;
            CurrentNode = startNode;
        }

        public INode CurrentNode { get; private set; }
        public string Identifier { get; }
        public IExecution Parent { get; }

        public IEnumerable<IExecution> ChildExecutions => childExecutions;

        public bool IsActive { get; private set; } = true;

        public void Proceed(string transitionName)
        {
            var transition = CurrentNode.OutgoingTransitions.SingleOrDefault(t => t.Identifier == transitionName);
            if (transition == null)
            {
                throw new TransitionNotFoundException(
                    $"Outgoing transition with name '{transitionName}' not found for node {CurrentNode.Name}");
            }

            Logger.Info($"Taking transition with name '{transition.Identifier}' to node '{transition.Destination.Name}'");
            CurrentNode = transition.Destination;
            transition.MarkAsExecuted();
            Execute();
        }

        public void Proceed()
        {
            var transition = CurrentNode.OutgoingTransitions.FirstOrDefault();
            if (transition == null)
            {
                Stop();
                return;
            }

            Logger.Info(
                $"Taking default-transition with name '{transition.Identifier}' to node '{transition.Destination.Name}'");
            CurrentNode = transition.Destination;
            transition.MarkAsExecuted();
            Execute();
        }

        public void CreateChild(INode node)
        {
            IsActive = false;
            var child = new Execution(this, Identifier + "_" + node.Name + "_" + childExecutions.Count, node);
            childExecutions.Add(child);
            child.Start();
        }

        public void Start()
        {
            CurrentNode.RequireNotNull($"Start node is null in execution '{Identifier}'");
            Execute();
        }

        public void Stop()
        {
            Logger.Info($"Execution '{Identifier}' ended.");
            IsActive = false;
        }

        private void Execute()
        {
            this.RequireActive();
            Logger.Info($"Executing node '{CurrentNode.Name}'");
            CurrentNode.Execute(this);
        }
    }
}