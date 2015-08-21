using System;
using System.Collections.Generic;
using PVM.Core.Data;
using PVM.Core.Definition;

namespace PVM.Core.Builder
{
    public class WorkflowDefinitionBuilder<T> : IWorkflowPathBuilder<T>
    {
        private readonly IDictionary<string, INode<T>> endNodes = new Dictionary<string, INode<T>>();
        private readonly IDictionary<string, INode<T>> nodes = new Dictionary<string, INode<T>>();
        private readonly IDictionary<string, List<TransitionData>> transitions = new Dictionary<string, List<TransitionData>>();
        private string identifier = Guid.NewGuid().ToString();
        private INode<T> startNode;

        public NodeBuilder<T> AddNode()
        {
            return new NodeBuilder<T>(this);
        }

        public IWorkflowPathBuilder<T> WithIdentifier(string id)
        {
            identifier = id;

            return this;
        }

        public WorkflowDefinition<T> BuildWorkflow()
        {
            AssembleTransitions();

            return new WorkflowDefinition<T>(identifier, startNode, nodes.Values, endNodes.Values);
        }

        private void AssembleTransitions()
        {
            foreach (var transition in transitions)
            {
                var sourceNode = nodes[transition.Key];

                foreach (var transitionData in transition.Value)
                {
                    var targetNode = nodes[transitionData.Target];
                    var transitionToAdd = new Transition<T>(transitionData.Name, sourceNode, targetNode);
                    sourceNode.OutgoingTransitions.Add(transitionToAdd);
                    targetNode.IncomingTransitions.Add(transitionToAdd);
                }
            }
        }

        internal void AddNode(INode<T> node, bool isStartNode, bool isEndNode, List<TransitionData> transitions)
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