using PVM.Core.Definition;
using PVM.Core.Plan;

namespace PVM.Core.Runtime
{
    public interface IBehavior
    {
        void Execute(INode node, IExecutionPlan executionPlan);
    }
}