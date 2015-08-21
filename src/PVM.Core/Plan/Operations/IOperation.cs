using PVM.Core.Data;
using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations
{
    public interface IOperation<T> where T : IProcessData<T>
    {
        void Execute(IExecution<T> execution);
    }
}