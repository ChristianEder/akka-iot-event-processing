using Akka.Actor;
using AkkaIoTIngress.Services;

namespace AkkaIoTIngress.Actors.Ingress
{
    public class IngressActorProvider
    {
        private readonly IActorRef _ingress;

        public IngressActorProvider(ActorSystem actorSystem, ITableStorage tableStorage)
        {
            _ingress = actorSystem.ActorOf(IngressActor.Props(this, tableStorage), "akka-iot-ingress");
        }

        public IActorRef Get()
        {
            return _ingress;
        }
    }
}