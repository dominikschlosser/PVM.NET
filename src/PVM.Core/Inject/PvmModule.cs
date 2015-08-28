using Ninject.Modules;
using PVM.Core.Persistence;
using PVM.Core.Serialization;

namespace PVM.Core.Inject
{
    public class PvmModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPersistenceProvider>().To<NullPersistenceProvider>();
            Bind<IObjectSerializer>().To<JsonSerializer>();

           
        }
    }
}