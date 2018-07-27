using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Event;
using AkkaIoTIngress.Model;
using AkkaIoTIngress.Services;
using Newtonsoft.Json;

namespace AkkaIoTIngress.Actors.Device
{
    public class DeviceActor : ReceiveActor
    {
        public class DeviceMessage
        {
            public string PartitionId { get; set; }
            public string Message { get; set; }
        }

        private string _deviceId;
        private readonly ITableStorage _tableStorage;
        private int _count = 0;

        public DeviceActor(string deviceId, ITableStorage tableStorage)
        {
            this._deviceId = deviceId;
            _tableStorage = tableStorage;
            Receive<DeviceMessage>(OnReceive);
        }

        private bool OnReceive(DeviceMessage data)
        {
            _count++;
            Console.WriteLine($"Device {_deviceId} received the {_count}th message");
            System.Diagnostics.Debug.WriteLine($"Device {_deviceId} received the {_count}th message");

            var message = JsonConvert.DeserializeObject<SensorTelemetry>(data.Message);
            var entity = new SensorTelemetryEntity
            {
                PartitionKey = _deviceId,
                RowKey = (long.MaxValue- DateTime.UtcNow.Ticks).ToString(),
                Humidity = message.Humidity,
                Temperature = message.Temperature
            };
            _tableStorage.Insert(entity, data.PartitionId);

            Sender.Tell(true);
            return true;
        }

        public static Props Props(string deviceId, ITableStorage tableStorage)
        {
            return Akka.Actor.Props.Create(() => new DeviceActor(deviceId, tableStorage));
        }


    }
}
