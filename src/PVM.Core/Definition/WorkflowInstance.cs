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
			rootExecution = new Execution(definition.InitialNode, definition.EndNodes);
            definition.InitialNode.Execute(rootExecution);
		}
	}
}