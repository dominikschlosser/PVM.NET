using PVM.Core.Definition;
using System.Collections.Generic;
using PVM.Core.Definition.Nodes;

namespace PVM.Core.Runtime
{
    public interface IExecution
    {
        string Identifier { get; }
        IExecution  Parent { get; }
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

    // TODO: kinda sucks but dont know a better way just yet. Activiti does it the same way
    // but at least come up with better names...
    public interface IInternalExecution : IExecution
    {
        IDictionary<string, object> Data { get; }  
    }
}