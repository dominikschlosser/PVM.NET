using PVM.Core.Definition;
using PVM.Core.Definition.Executions;

namespace PVM.Core.Test
{
	public class MockBehavior : IBehavior
	{
		public bool Executed { get; private set; }

		public void Execute(IExecution execution)
		{
			Executed = true;
			execution.Proceed();
		}
	}
}