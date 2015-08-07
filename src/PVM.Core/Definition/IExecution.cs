namespace PVM.Core.Definition
{
    public interface IExecution
    {
	    void Proceed(string transitionName);
	    void Proceed();
    }
}