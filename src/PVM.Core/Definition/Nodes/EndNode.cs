using PVM.Core.Definition.Executables;

namespace PVM.Core.Definition.Nodes
{
    public class EndNode : Node
    {
        public EndNode(string name)
            : base(name, new TransientBehavior())
        {
        }
    }
}