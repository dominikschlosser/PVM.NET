using PVM.Core.Inject;
using PVM.Core.Persistence;
using PVM.Core.Serialization;

namespace PVM.Core.Builder
{
    public class BasicServiceLocatorBuilder
    {
        private readonly BasicServiceLocator serviceLocator;
        private readonly WorkflowEngineBuilder workflowEngineBuilder;


        private IObjectSerializer objectSerializer = new JsonSerializer();
        private IPersistenceProvider persistenceProvider = new NullPersistenceProvider();

        public BasicServiceLocatorBuilder(BasicServiceLocator serviceLocator,
            WorkflowEngineBuilder workflowEngineBuilder)
        {
            this.serviceLocator = serviceLocator;
            this.workflowEngineBuilder = workflowEngineBuilder;
        }

        public BasicServiceLocatorBuilder WithPersistenceProvider(IPersistenceProvider persistenceProvider)
        {
            this.persistenceProvider = persistenceProvider;
            return this;
        }

        public BasicServiceLocatorBuilder WithObjectSerializer(IObjectSerializer objectSerializer)
        {
            this.objectSerializer = objectSerializer;
            return this;
        }


        public WorkflowEngineBuilder Build()
        {
            serviceLocator.Register(typeof (IPersistenceProvider), persistenceProvider);
            serviceLocator.Register(typeof (IObjectSerializer), objectSerializer);

            return workflowEngineBuilder;
        }
    }
}