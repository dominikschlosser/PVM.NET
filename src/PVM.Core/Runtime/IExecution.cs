using System.Collections.Generic;
using PVM.Core.Data;
using PVM.Core.Definition;
using PVM.Core.Definition.Nodes;

namespace PVM.Core.Runtime
{
    public interface IExecution<T> where T : ICopyable<T>
    {
        string Identifier { get; }
        IExecution <T> Parent { get; }
        T Data { get; }
        IList<IExecution<T>> Children { get; }
        INode<T> CurrentNode { get; }
        bool IsActive { get; }
        void Proceed();
        void Proceed(string transitionName);
        void Stop();
        void Start(INode<T> startNode, T data);
        void CreateChild(INode<T> startNode);
        void Accept(IExecutionVisitor<T> visitor);
    }
}