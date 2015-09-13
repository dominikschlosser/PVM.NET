using PVM.Core.Definition;
using PVM.Persistence.Sql.Model;

namespace PVM.Persistence.Sql.Transform
{
    public interface IWorkflowDefinitionTransformer
    {
        WorkflowDefinitionModel Transform(IWorkflowDefinition workflowDefinition);
        IWorkflowDefinition TransformBack(WorkflowDefinitionModel model);
    }
}