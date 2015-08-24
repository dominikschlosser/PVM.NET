using System;
using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations.Base
{
    public abstract class DataAwareOperation<T> : IOperation<T> where T: class, new()
    {
        public abstract void Execute(IExecution execution, T dataContext);

        public void Execute(IExecution execution)
        {
            throw new InvalidOperationException("Use overload with datacontext");
        }
    }
}