using PVM.Core.Definition.Executions;

namespace PVM.Core.Definition.Executables
{
	public class TransientBehavior : IBehavior
	{
		public void Execute(IExecution execution)
		{
			execution.Proceed();
		}
	}
}