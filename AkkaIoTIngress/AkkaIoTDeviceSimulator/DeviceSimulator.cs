using System;
using System.Configuration;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AkkaIoTDeviceSimulator
{
    public class DeviceSimulator
    {
        private readonly string _deviceId;
        private bool _stopped = false;

        public DeviceSimulator(string deviceId)
        {
            _deviceId = deviceId;
        }

        public async void Start()
        {
           SendData();
        }

        public async Task Send()
        {
            await SendMessage();
        }
        

        private async void SendData()
        {
            await SendMessage();

            await Task.Delay(5000);

            if (!_stopped)
            {
                SendData();
            }
        }

        private async Task SendMessage()
        {
            try
            {
                var random = new Random();
                var message = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new
                {
                    temperature = random.Next(50, 80),
                    humidity = random.Next(40, 70)
                }));
                var now = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;

                var request = CreateRequest(DeviceAuthorizationInfo.For(_deviceId), now, message);
                using (var response = (HttpWebResponse)await request.GetResponseAsync())
                {
                    Console.WriteLine($"Message sent from {_deviceId}, result: {response.StatusCode}");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Stop()
        {
            _stopped = true;
        }

        private static HttpWebRequest CreateRequest(DeviceAuthorizationInfo device, long sentDate, byte[] message)
        {
            var url = $"https://akka-iot.azure-devices.net/devices/{device.DeviceId}/messages/events?api-version=2016-11-14";

            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Headers.Add("Authorization", device.SharedAccessKeySignature);
            request.Headers.Add("iothub-app-SentAt", sentDate.ToString());
            request.Headers.Add("iothub-app-PayloadVersion", "1");


            request.Method = "POST";
            request.ContentType = "application/json";
            request.ContentLength = message.Length;

            using (var stream = request.GetRequestStream())
            {
                stream.Write(message, 0, message.Length);
            }
            return request;
        }
    }
}