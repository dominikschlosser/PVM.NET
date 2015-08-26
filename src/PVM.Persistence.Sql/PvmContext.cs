using System.Data.Entity;
using PVM.Persistence.Sql.Model;

namespace PVM.Persistence.Sql
{
    public class PvmContext : DbContext
    {
        public DbSet<ExecutionModel> Executions { get; set; }
        public DbSet<WorkflowDefinitionModel> WorkflowDefinitions { get; set; }
    }
}