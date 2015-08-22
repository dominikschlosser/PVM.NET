using log4net;
using PVM.Core.Definition;
using System;
using System.Collections.Generic;

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

        public string Identifier { get; } = Guid.NewGuid().ToString();

        public bool IsFinished
        {
            get { return plan.IsFinished; }
        }

        public void Start(IDictionary<string, object> data)
        {
            plan.Start(definition.InitialNode, data);
        }

        public void Start()
        {
            Start(new Dictionary<string, object>());
        }
    }
}