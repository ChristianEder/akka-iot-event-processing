using Akka.Actor;
using AkkaIoTIngress.Services;
using Microsoft.Azure.EventHubs.Processor;

namespace AkkaIoTIngress.Actors.Ingress
{
    public class EventProcessorFactory : IEventProcessorFactory
    {
        private readonly ActorSystem _actorSystem;
        private readonly ITableStorage _tableStorage;

        public EventProcessorFactory(ActorSystem actorSystem, ITableStorage tableStorage)
        {
            _actorSystem = actorSystem;
            _tableStorage = tableStorage;
        }

        public IEventProcessor CreateEventProcessor(PartitionContext context)
        {
            return new EventProcessor(_actorSystem, _tableStorage);
        }
    }
}