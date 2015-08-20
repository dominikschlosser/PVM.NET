using log4net;
using PVM.Core.Definition;

namespace PVM.Core.Plan
{
    public class WorkflowInstance
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (WorkflowInstance));
        private readonly WorkflowDefinition definition;
        private readonly IExecutionPlan plan;

        public WorkflowInstance(WorkflowDefinition definition)
        {
            plan = new ExecutionPlan(definition);
            this.definition = definition;
        }

        public void Start()
        {
            plan.Start(definition.InitialNode);
        }
    }
}