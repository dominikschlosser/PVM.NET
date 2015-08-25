using PVM.Core.Definition;
using PVM.Core.Plan.Operations.Base;
using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations
{
    public class SubProcessOperation : IOperation
    {
        public void Execute(IExecution execution)
        {
            var workflowDefinition = execution.CurrentNode as WorkflowDefinition;
            if (workflowDefinition == null)
            {
                throw new WorkflowInconsistentException(
                    string.Format("SubProcessOperation can only operate on workflow definition nodes. ({0})",
                        execution.CurrentNode.Name));
            }

            execution.Proceed(workflowDefinition.InitialNode);
        }
    }
}