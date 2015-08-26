using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using PVM.Core.Runtime;

namespace PVM.Persistence.Sql.Model
{
    public class ExecutionModel
    {
        [Key]
        public string Identifier { get; set; }

        public virtual ExecutionModel Parent { get; set; }
        public virtual IList<ExecutionModel> Children { get; set; }
        public virtual NodeModel CurrentNode { get; set; }
        public bool IsActive { get; set; }

        public static ExecutionModel FromExecution(IExecution execution)
        {
            return new ExecutionModel()
            {
                Identifier = execution.Identifier,
                IsActive = execution.IsActive,
                CurrentNode = NodeModel.FromNode(execution.CurrentNode),
                Parent = execution.Parent == null ? null : FromExecution(execution.Parent),
                Children = execution.Children.Select(FromExecution).ToList()
            };
        }
    }
}