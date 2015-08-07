namespace PVM.Core.Definition
{
    public interface IExecution
    {
        bool IsActive { get; }
        INode CurrentNode { get; }
        void Proceed(string transitionName);
    }
}