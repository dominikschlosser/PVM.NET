using System;

namespace PVM.Core.Builder
{
    public class TransitionBuilder
    {
        private readonly NodeBuilder parentNodeBuilder;
        private readonly string source;
        private string name = Guid.NewGuid().ToString();
        private string target;

        public TransitionBuilder(NodeBuilder parentNodeBuilder, string source)
        {
            this.parentNodeBuilder = parentNodeBuilder;
            this.source = source;
        }

        public TransitionBuilder WithName(string name)
        {
            this.name = name;

            return this;
        }

        public TransitionBuilder To(string targetNode)
        {
            target = targetNode;

            return this;
        }

        public NodeBuilder BuildTransition()
        {
            parentNodeBuilder.AddTransition(new TransitionData(name, target, source));
            return parentNodeBuilder;
        }
    }
}