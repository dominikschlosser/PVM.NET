using PVM.Core.Definition;
using System.Collections.Generic;

namespace PVM.Core.Runtime
{
    public interface IExecution
    {
        string Identifier { get; }
        IExecution  Parent { get; }
        IDictionary<string, object> Data { get; }
        IList<IExecution> Children { get; }
        INode CurrentNode { get; }
        bool IsActive { get; }
        void Proceed();
        void Proceed(string transitionName);
        void Resume();
        void Stop();
        void Start(INode startNode, IDictionary<string, object> data);
        void CreateChild(INode startNode);
        void Accept(IExecutionVisitor visitor);
    }
}