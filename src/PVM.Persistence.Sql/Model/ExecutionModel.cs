using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using PVM.Core.Runtime;
using PVM.Core.Serialization;

namespace PVM.Persistence.Sql.Model
{
    public class ExecutionModel
    {
        [Key]
        public string Identifier { get; set; }

        public virtual ExecutionModel Parent { get; set; }
        public virtual IList<ExecutionModel> Children { get; set; }
        public virtual string CurrentNodeIdentifier { get; set; }
        public bool IsActive { get; set; }
        public virtual IList<ExecutionVariableModel> Variables { get; set; }
 
        public static ExecutionModel FromExecution(IExecution execution, IObjectSerializer serializer)
        {
            var variables = execution.Data.Select(entry => new ExecutionVariableModel() {Key = entry.Key, SerializedValue = serializer.Serialize(entry.Value), ValueType = entry.Value.GetType().FullName}).ToList();
            return new ExecutionModel()
            {
                Identifier = execution.Identifier,
                IsActive = execution.IsActive,
                CurrentNodeIdentifier = execution.CurrentNode.Identifier,
                Parent = execution.Parent == null ? null : FromExecution(execution.Parent, serializer),
                Children = execution.Children.Select(c => FromExecution(c, serializer)).ToList(),
                Variables = variables
            };
        }
    }
}