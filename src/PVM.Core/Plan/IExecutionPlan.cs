using PVM.Core.Definition;
using PVM.Core.Plan.Operations;

namespace PVM.Core.Plan
{
    public interface IExecutionPlan
    {
        void Proceed(INode node, IOperation operation);
        void Start(INode startNode);
    }
}