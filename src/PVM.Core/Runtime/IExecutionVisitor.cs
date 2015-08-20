namespace PVM.Core.Runtime
{
    public interface IExecutionVisitor
    {
        void Visit(IExecution execution);
    }
}