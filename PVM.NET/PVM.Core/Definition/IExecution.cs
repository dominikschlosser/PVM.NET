namespace PVM.Core.Definition
{
    public interface IExecution
    {
        bool IsActive { get; }
        void Proceed(string transitionName);
    }
}