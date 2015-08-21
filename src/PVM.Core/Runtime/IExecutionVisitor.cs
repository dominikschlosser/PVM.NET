using PVM.Core.Data;

namespace PVM.Core.Runtime
{
    public interface IExecutionVisitor<T> where T : ICopyable<T>
    {
        void Visit(IExecution<T> execution);
    }
}