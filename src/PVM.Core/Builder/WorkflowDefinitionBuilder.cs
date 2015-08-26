using log4net;
using PVM.Core.Definition;
using System;
using System.Collections.Generic;

namespace PVM.Core.Builder
{
    public class WorkflowDefinitionBuilder : IWorkflowPathBuilder
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (WorkflowDefinitionBuilder));

        private readonly IDictionary<string, INode> endNodes = new Dictionary<string, INode>();
        private readonly IDictionary<string, INode> nodes = new Dictionary<string, INode>();
        private readonly IDictionary<string, List<TransitionData>> transitions = new Dictionary<string, List<TransitionData>>();
        private string identifier = Guid.NewGuid().ToString();
        private INode startNode;

        public NodeBuilder AddNode()
        {
            return new NodeBuilder(this);
        }

        public IWorkflowPathBuilder WithIdentifier(string id)
        {
            identifier = id;

            return this;
        }

        public WorkflowDefinition<T> BuildWorkflow<T>() where T : class
        {
            AssembleTransitions();

            return
                new WorkflowDefinition<T>.Builder().WithIdentifier(identifier)
                                                .WithInitialNode(startNode)
                                                .WithNodes(nodes.Values)
                                                .WithEndNodes(endNodes.Values)
                                                .Build();
        }

        public WorkflowDefinition<object> BuildWorkflow()
        {
            return BuildWorkflow<object>();
        }

        public WorkflowDefinitionBuilder AsDefinitionBuilder()
        {
            return this;
        }

        private void AssembleTransitions()
        {
            foreach (var transition in transitions)
            {
                Logger.DebugFormat("Assembling transition from '{0}'", transition.Key);
                var sourceNode = nodes[transition.Key];

                foreach (var transitionData in transition.Value)
                {

                    var targetNode = nodes[transitionData.Target];
                    Logger.DebugFormat("  - Source: {0}, Target: {1}", sourceNode.Identifier, targetNode.Identifier);

                    var transitionToAdd = new Transition(transitionData.Name, transitionData.IsDefault, sourceNode, targetNode);
                    sourceNode.AddOutgoingTransition(transitionToAdd);
                    targetNode.AddIncomingTransition(transitionToAdd);
                }
            }
        }

        internal void AddNode(INode node, bool isStartNode, bool isEndNode, List<TransitionData> transitions)
        {
            nodes.Add(node.Identifier, node);

            if (isStartNode)
            {
                startNode = node;
            }

            if (isEndNode)
            {
                endNodes.Add(node.Identifier, node);
            }

            this.transitions[node.Identifier] = transitions;
        }
    }
}