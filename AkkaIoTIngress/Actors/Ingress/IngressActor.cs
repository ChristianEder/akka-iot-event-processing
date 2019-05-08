using System;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using AkkaIoTIngress.Actors.Device;
using AkkaIoTIngress.Services;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

namespace AkkaIoTIngress.Actors.Ingress
{
    public class IngressActor : ReceiveActor
    {
        private readonly ActorSystem _system;
        private readonly ITableStorage _tableStorage;
        private EventProcessorHost _eventProcessorHost;

        public class StartListening { }
        public class StopListening { }

        public class DispatchEventData
        {
            public string PartitionId { get; set; }
            public EventData EventData { get; set; }
        }

        public IngressActor(ActorSystem system, ITableStorage tableStorage)
        {
            _system = system;
            _tableStorage = tableStorage;
            ReceiveAsync<StartListening>(OnStart);
            ReceiveAsync<StopListening>(OnStop);
            ReceiveAsync<DispatchEventData>(OnDispatch);
        }

        private async Task<bool> OnStart(StartListening arg)
        {
            _eventProcessorHost = new EventProcessorHost(
                "akka-iot",
                PartitionReceiver.DefaultConsumerGroupName,
                Environment.GetEnvironmentVariable("akka-iot-hub-endpoint"),
                Environment.GetEnvironmentVariable("akka-iot-checkpoint-storage"),
                "akka-iot-checkpoints");
            await _eventProcessorHost.RegisterEventProcessorFactoryAsync(new EventProcessorFactory(_system, _tableStorage));
            return true;
        }

        private async Task<bool> OnStop(StopListening obj)
        {
            await _eventProcessorHost.UnregisterEventProcessorAsync();
            return true;
        }

        private async Task<bool> OnDispatch(DispatchEventData message)
        {
            var eventData = message.EventData;
            var data = Encoding.UTF8.GetString(eventData.Body.Array, eventData.Body.Offset, eventData.Body.Count);
            var deviceId = eventData.SystemProperties["iothub-connection-device-id"].ToString();
            var deviceActor = Get(deviceId);
            var success = await deviceActor.Ask<bool>(new DeviceActor.DeviceMessage { Message = data, PartitionId = message.PartitionId });
            Sender.Tell(success);
            return success;
        }

        private IActorRef Get(string deviceId)
        {
            var deviceActor = Context.Child(deviceId);

            return deviceActor is Nobody
                ? Context.ActorOf(DeviceActor.Props(deviceId, _tableStorage), deviceId)
                : deviceActor;
        }
    }
}
