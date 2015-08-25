using log4net;
using PVM.Core.Data.Proxy;
using PVM.Core.Definition;
using System;

namespace PVM.Core.Plan
{
    public class WorkflowInstance<T> where T : class
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof (WorkflowInstance<T>));
        private readonly IWorkflowDefinition definition;
        private readonly IExecutionPlan plan;

        public WorkflowInstance(IWorkflowDefinition definition)
        {
            Identifier = Guid.NewGuid().ToString();
            plan = new ExecutionPlan(definition);
            this.definition = definition;
        }

        public string Identifier { get; private set; }

        public bool IsFinished
        {
            get { return plan.IsFinished; }
        }

        public void Start(T data)
        {
            plan.Start(definition.InitialNode, DataMapper.ExtractData<T>(data));
        }

        public void Start()
        {
            plan.Start(definition.InitialNode);
        }
    }
}