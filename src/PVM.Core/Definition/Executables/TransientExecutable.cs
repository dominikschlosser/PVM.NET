namespace PVM.Core.Definition.Executables
{
	public class TransientExecutable : IExecutable
	{
		public void Execute(IExecution execution)
		{
			execution.Proceed();
		}
	}
}