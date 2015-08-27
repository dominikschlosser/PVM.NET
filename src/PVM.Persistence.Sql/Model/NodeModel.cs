using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using PVM.Core.Definition;

namespace PVM.Persistence.Sql.Model
{
    public class NodeModel
    {
        [Key]
        public string Identifier { get; set; }

        public string OperationType { get; set; }
        public virtual IList<TransitionModel> IncomingTransitions { get; set; }
        public virtual IList<TransitionModel> OutgoingTransitions { get; set; }

        public static NodeModel FromNode(INode node)
        {
            return new NodeModel
            {
                Identifier = node.Identifier,
                OperationType = node.OperationType,
                IncomingTransitions = node.IncomingTransitions.Select(TransitionModel.FromTransition).ToList(),
                OutgoingTransitions = node.OutgoingTransitions.Select(TransitionModel.FromTransition).ToList()
            };
        }
    }
}