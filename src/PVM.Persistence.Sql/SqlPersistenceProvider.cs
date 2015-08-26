using System.Linq;
using PVM.Core.Definition;
using PVM.Core.Infrastructure.Serialization;
using PVM.Core.Persistence;
using PVM.Core.Runtime;
using PVM.Persistence.Sql.Model;

namespace PVM.Persistence.Sql
{
    public class SqlPersistenceProvider : IPersistenceProvider
    {
        public void Persist(IExecution execution)
        {
            using (var db = new PvmContext())
            {
                var entity = ExecutionModel.FromExecution(execution, new JsonSerializer());

                if (db.Executions.Any(e => e.Identifier == execution.Identifier))
                {
                    db.Executions.Attach(entity);
                }
                else
                {
                    db.Executions.Add(entity);
                }

                db.SaveChanges();
            }
        }

        public void Persist(IWorkflowDefinition workflowDefinition)
        {
            using (var db = new PvmContext())
            {
                var entity = WorkflowDefinitionModel.FromWorkflowDefinition(workflowDefinition);

                if (db.WorkflowDefinitions.Any(e => e.Identifier == workflowDefinition.Identifier))
                {
                    db.WorkflowDefinitions.Attach(entity);
                }
                else
                {
                    db.WorkflowDefinitions.Add(entity);
                }

                db.SaveChanges();
            }
        }
    }
}