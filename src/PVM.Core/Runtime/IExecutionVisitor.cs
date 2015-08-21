namespace PVM.Core.Runtime
{
    public interface IExecutionVisitor<T>
    {
        void Visit(IExecution<T> execution);
    }
}