using PVM.Core.Definition;
using PVM.Core.Runtime;

namespace PVM.Core.Plan
{
    public interface IExecutionPlan
    {
        IWorkflowDefinition Definition { get; }
        void Proceed(IExecution execution, string operationType);
        void OnExecutionStarting(Execution execution);
        void OnExecutionStopped(Execution execution);
        void OnOutgoingTransitionIsNull(Execution execution, string transitionIdentifier);
        bool IsFinished { get; }
        void OnExecutionResuming(Execution execution);
        void OnExecutionReachesWaitState(Execution execution);
    }
}