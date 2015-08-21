using System;
using PVM.Core.Data;

namespace PVM.Core.Builder
{
    public class TransitionBuilder<T> where T : ICopyable<T>
    {
        private readonly NodeBuilder<T> parentNodeBuilder;
        private readonly string source;
        private string name = Guid.NewGuid().ToString();
        private string target;

        public TransitionBuilder(NodeBuilder<T> parentNodeBuilder, string source)
        {
            this.parentNodeBuilder = parentNodeBuilder;
            this.source = source;
        }

        public TransitionBuilder<T> WithName(string name)
        {
            this.name = name;

            return this;
        }

        public TransitionBuilder<T> To(string targetNode)
        {
            target = targetNode;

            return this;
        }

        public NodeBuilder<T> BuildTransition()
        {
            parentNodeBuilder.AddTransition(new TransitionData(name, target, source));
            return parentNodeBuilder;
        }
    }
}