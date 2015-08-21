using PVM.Core.Data;
using PVM.Core.Definition;
using PVM.Core.Plan.Operations;

namespace PVM.Core.Runtime
{
    public interface IBehavior<T> where T : IProcessData<T>
    {
        IOperation<T> CreateOperation(INode<T> node);
    }
}