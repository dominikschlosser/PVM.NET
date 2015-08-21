using PVM.Core.Data;

namespace PVM.Core.Runtime
{
    public interface IExecutionVisitor<T> where T : IProcessData<T>
    {
        void Visit(IExecution<T> execution);
    }
}