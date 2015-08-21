using PVM.Core.Definition;
using PVM.Core.Plan.Operations;

namespace PVM.Core.Runtime
{
    public interface IBehavior
    {
        IOperation CreateOperation(INode node);
    }
}