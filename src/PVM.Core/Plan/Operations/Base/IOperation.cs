using PVM.Core.Runtime;

namespace PVM.Core.Plan.Operations.Base
{
    public interface IOperation<in TDataMappingDefinition> : IOperation where TDataMappingDefinition : class, new()
    {
        void Execute(IExecution execution, TDataMappingDefinition dataContext);
    }

    public interface IOperation
    {
        void Execute(IExecution execution);
    }
}