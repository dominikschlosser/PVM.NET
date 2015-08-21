using System;
using log4net;
using PVM.Core.Definition;

namespace PVM.Core.Plan
{
    public class WorkflowInstance
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (WorkflowInstance));
        private readonly string identifier = Guid.NewGuid().ToString();
        private readonly IExecutionPlan plan;
        private readonly WorkflowDefinition definition;

        public WorkflowInstance(WorkflowDefinition definition)
        {
            plan = new ExecutionPlan(definition);
            this.definition = definition;
        }

        public string Identifier
        {
            get { return identifier; }
        }

        public void Start()
        {
            plan.Start(definition.InitialNode);
        }
    }
}