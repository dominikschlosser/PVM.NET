using System.Collections.Generic;
using PVM.Core.Definition;

namespace PVM.Core.Runtime
{
    public interface IExecution
    {
        string Identifier { get; }
        IExecution Parent { get; }
        IList<IExecution> Children { get; }
        INode CurrentNode { get; }
        bool IsActive { get; }
        void Proceed();
        void Proceed(string transitionName);
        void Stop();
        void Accept(IExecutionVisitor visitor);
        void Start(INode startNode);
        void CreateChild(INode startNode);
    }
}