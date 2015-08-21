using PVM.Core.Definition.Nodes;
using PVM.Core.Plan.Operations;
using PVM.Core.Runtime;

namespace PVM.Core.Plan
{
    public interface IExecutionPlan<T>
    {
        void Proceed(IExecution<T> execution, IOperation<T> operation);
        void Start(INode<T> startNode, T data);
        void OnExecutionStarting(Execution<T> execution);
        void OnExecutionStopped(Execution<T> execution);
        void OnOutgoingTransitionIsNull(Execution<T> execution, string transitionIdentifier);
    }
}