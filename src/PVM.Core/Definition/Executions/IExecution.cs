using System.Collections.Generic;
using PVM.Core.Definition.Nodes;

namespace PVM.Core.Definition.Executions
{
    public interface IExecution
    {
        INode CurrentNode { get; }
        string Identifier { get; }
        IExecution Parent { get; }
        IEnumerable<IExecution> ChildExecutions { get; }
        bool IsActive { get; }
        void Proceed(string transitionName);
        void Proceed();
        void CreateChild(INode node);
        void Start();
        void Stop();
    }
}