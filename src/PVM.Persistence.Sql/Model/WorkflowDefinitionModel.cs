using System.Collections.Generic;
using System.Linq;
using PVM.Core.Definition;

namespace PVM.Persistence.Sql.Model
{
    public class WorkflowDefinitionModel : NodeModel
    {
        public virtual IList<NodeModel> Nodes { get; set; }
        public virtual IList<NodeModel> EndNodes { get; set; }

        public static WorkflowDefinitionModel FromWorkflowDefinition(IWorkflowDefinition workflowDefinition)
        {
            return new WorkflowDefinitionModel
            {
                Identifier = workflowDefinition.Identifier,
                OperationType = workflowDefinition.Operation.GetType().AssemblyQualifiedName,
                IncomingTransitions = workflowDefinition.IncomingTransitions.Select(TransitionModel.FromTransition).ToList(),
                OutgoingTransitions = workflowDefinition.OutgoingTransitions.Select(TransitionModel.FromTransition).ToList(),
                Nodes = workflowDefinition.Nodes.Select(FromNode).ToList(),
                EndNodes = workflowDefinition.Nodes.Select(FromNode).ToList(),

            };
        }
    }
}