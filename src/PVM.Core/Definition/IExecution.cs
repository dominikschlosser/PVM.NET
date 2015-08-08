namespace PVM.Core.Definition
{
    public interface IExecution
    {
        string Identifier { get; }
        bool IsActive { get; }
	    void Proceed(string transitionName);
	    void Proceed();
        void Start();
    }
}