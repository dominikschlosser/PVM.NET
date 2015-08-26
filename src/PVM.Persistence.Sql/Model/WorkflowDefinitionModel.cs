using System.Collections.Generic;

namespace PVM.Persistence.Sql.Model
{
    public class WorkflowDefinitionModel : NodeModel
    {
        public virtual IList<NodeModel> Nodes { get; set; }
        public virtual IList<NodeModel> EndNodes { get; set; }
    }
}