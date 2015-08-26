using System.Linq;
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
                var entity = ExecutionModel.FromExecution(execution);
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
    }
}