using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;

namespace AkkaIoTIngress.Model
{
    public class SensorTelemetry
    {
        [JsonProperty("temperature")]
        public int Temperature { get; set; }

        [JsonProperty("humidity")]
        public int Humidity { get; set; }
    }

    public class SensorTelemetryEntity : TableEntity
    {
        public int Temperature { get; set; }
        public int Humidity { get; set; }
    }
}
