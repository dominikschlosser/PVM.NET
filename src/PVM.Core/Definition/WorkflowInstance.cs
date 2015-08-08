namespace PVM.Core.Definition
{
	public class WorkflowInstance
	{
		private IExecution rootExecution;
		private readonly WorkflowDefinition definition;

		public WorkflowInstance(WorkflowDefinition definition)
		{
			this.definition = definition;
		}

		public void Start()
		{
			rootExecution = new Execution(definition.Identifier + "_Root", definition.InitialNode);
            rootExecution.Start();
		}
	}
}