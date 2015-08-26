using PVM.Core.Definition;
using System.Collections.Generic;
using PVM.Core.Plan;

namespace PVM.Core.Runtime
{
    public interface IExecution
    {
        string Identifier { get; }
        IExecution  Parent { get; }
        IList<IExecution> Children { get; }
        INode CurrentNode { get; }
        IExecutionPlan Plan { get; }
        bool IsActive { get; }
        IDictionary<string, object> Data { get; }


        void Proceed();
        void Proceed(string transitionName);
        void Resume();
        void Resume(INode currentNode);
        void Stop();
        void Start(INode startNode, IDictionary<string, object> data);
        void CreateChild(INode startNode);
        void Accept(IExecutionVisitor visitor);
        void Proceed(INode currentNode);
        void Wait(string signal);
    }
}