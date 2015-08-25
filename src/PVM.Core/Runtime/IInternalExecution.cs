using System.Collections.Generic;

namespace PVM.Core.Runtime
{
    public interface IInternalExecution : IExecution
    {
        IDictionary<string, object> Data { get; }
        
    }
}