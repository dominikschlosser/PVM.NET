using System;
using log4net;
using PVM.Core.Definition;

namespace PVM.Core.Plan
{
    public class WorkflowInstance<T>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (WorkflowInstance<T>));
        private readonly WorkflowDefinition<T> definition;
        private readonly IExecutionPlan<T> plan;

        public WorkflowInstance(WorkflowDefinition<T> definition)
        {
            plan = new ExecutionPlan<T>(definition);
            this.definition = definition;
        }

        public string Identifier { get; } = Guid.NewGuid().ToString();

        public bool IsFinished
        {
            get { return plan.IsFinished; }
        }

        public void Start(T data)
        {
            plan.Start(definition.InitialNode, data);
        }
    }
}