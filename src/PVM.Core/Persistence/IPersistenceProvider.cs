using PVM.Core.Runtime;

namespace PVM.Core.Persistence
{
    public interface IPersistenceProvider
    {
        void Persist(IExecution execution);
    }
}