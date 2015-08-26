using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PVM.Persistence.Sql.Model
{
    public class ExecutionVariableModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Identifier { get; set; }

        public string Key { get; set; }
        public string SerializedValue { get; set; }
        public string ValueType { get; set; }
    }
}