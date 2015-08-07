using PVM.Core.Definition;

namespace PVM.Core.Test
{
	public class MockExecutable : IExecutable
	{
		public bool Executed { get; private set; }

		public void Execute(IExecution execution)
		{
			Executed = true;
			execution.Proceed();
		}
	}
}