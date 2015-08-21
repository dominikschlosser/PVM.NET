using PVM.Core.Data;
using PVM.Core.Definition;
using PVM.Core.Plan.Operations;
using PVM.Core.Runtime;

namespace PVM.Core.Plan
{
    public interface IExecutionPlan<T> where T : IProcessData<T>
    {
        void Proceed(IExecution<T> execution, IOperation<T> operation);
        void Start(INode<T> startNode, T data);
        void OnExecutionStarting(Execution<T> execution);
        void OnExecutionStopped(Execution<T> execution);
        void OnOutgoingTransitionIsNull(Execution<T> execution, string transitionIdentifier);
    }
}