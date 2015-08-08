using System;
using System.Collections.Generic;
using System.Linq;
using PVM.Core.Definition;
using PVM.Core.Definition.Nodes;

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

            return new WorkflowDefinition(identifier, startNode, nodes.Values, endNodes.Values);
        }

        private void AssembleTransitions()
        {
            foreach (var transition in transitions)
            {
                var sourceNode = nodes[transition.Key];

                if (transition.Value.Any() && endNodes.ContainsKey(sourceNode.Name))
                {
                    string aggregatedTargetNames = transition.Value.Select(t => t.Target).Aggregate((t1, t2) => "'" + t1 + "', '" + t2 + "'");

                    throw new WorkflowValidationException(
                        $"Node '{sourceNode.Name}' is start node but has transitions to nodes {aggregatedTargetNames}");
                }

                foreach (var transitionData in transition.Value)
                {
                    var targetNode = nodes[transitionData.Target];
                    var transitionToAdd = new Transition(transitionData.Name, sourceNode, targetNode);
                    sourceNode.OutgoingTransitions.Add(transitionToAdd);
                    targetNode.IncomingTransitions.Add(transitionToAdd);
                }
            }
        }

        internal void AddNode(Node node, bool isStartNode, bool isEndNode, List<TransitionData> transitions)
        {
            nodes.Add(node.Name, node);

            if (isStartNode)
            {
                if (startNode != null)
                {
                    throw new WorkflowValidationException(
                        $"Cannot set '{node.Name}' as start node since there is already a start node ('{startNode.Name}')");
                }

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