using PVM.Core.Definition;
using System;
using System.Collections.Generic;

namespace PVM.Core.Builder
{
    public class WorkflowDefinitionBuilder : IWorkflowPathBuilder
    {
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

        public WorkflowDefinition BuildWorkflow()
        {
            AssembleTransitions();

            return
                new WorkflowDefinition.Builder().WithIdentifier(identifier)
                                                .WithInitialNode(startNode)
                                                .WithNodes(nodes.Values)
                                                .WithEndNodes(endNodes.Values)
                                                .Build();
        }

        private void AssembleTransitions()
        {
            foreach (var transition in transitions)
            {
                var sourceNode = nodes[transition.Key];

                foreach (var transitionData in transition.Value)
                {
                    var targetNode = nodes[transitionData.Target];
                    var transitionToAdd = new Transition(transitionData.Name, transitionData.IsDefault, sourceNode, targetNode);
                    sourceNode.OutgoingTransitions.Add(transitionToAdd);
                    targetNode.IncomingTransitions.Add(transitionToAdd);
                }
            }
        }

        internal void AddNode(INode node, bool isStartNode, bool isEndNode, List<TransitionData> transitions)
        {
            nodes.Add(node.Name, node);

            if (isStartNode)
            {
                startNode = node;
            }

            if (isEndNode)
            {
                endNodes.Add(node.Name, node);
            }

            this.transitions[node.Name] = transitions;
        }
    }
}