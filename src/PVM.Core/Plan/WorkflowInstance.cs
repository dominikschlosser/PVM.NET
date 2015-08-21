using System;
using log4net;
using PVM.Core.Data;
using PVM.Core.Definition;

namespace PVM.Core.Plan
{
    public class WorkflowInstance<T> where T : ICopyable<T>
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(WorkflowInstance<T>));
        private readonly string identifier = Guid.NewGuid().ToString();
        private readonly IExecutionPlan<T> plan;
        private readonly WorkflowDefinition<T> definition;

        public WorkflowInstance(WorkflowDefinition<T> definition)
        {
            plan = new ExecutionPlan<T>(definition);
            this.definition = definition;
        }

        public string Identifier
        {
            get { return identifier; }
        }

        public void Start(T data)
        {
            plan.Start(definition.InitialNode, data);
        }
    }
}