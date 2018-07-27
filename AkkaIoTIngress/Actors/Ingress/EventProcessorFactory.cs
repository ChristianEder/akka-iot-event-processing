using AkkaIoTIngress.Services;
using Microsoft.Azure.EventHubs.Processor;

namespace AkkaIoTIngress.Actors.Ingress
{
    public class EventProcessorFactory : IEventProcessorFactory
    {
        private readonly IngressActorProvider _provider;
        private readonly ITableStorage _tableStorage;

        public EventProcessorFactory(IngressActorProvider provider, ITableStorage tableStorage)
        {
            _provider = provider;
            _tableStorage = tableStorage;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return new EventProcessor(_provider, _tableStorage);
        }
    }
}