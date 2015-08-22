using PVM.Core.Definition;
using PVM.Core.Plan.Operations;
using PVM.Core.Runtime;
using System.Collections.Generic;

namespace PVM.Core.Plan
{
    public interface IExecutionPlan
    {
        void Proceed(IExecution execution, IOperation operation);
        void Start(INode startNode, IDictionary<string, object> data);
        void OnExecutionStarting(Execution execution);
        void OnExecutionStopped(Execution execution);
        void OnOutgoingTransitionIsNull(Execution execution, string transitionIdentifier);
        bool IsFinished { get; }
        void OnExecutionResuming(Execution execution);
    }
}