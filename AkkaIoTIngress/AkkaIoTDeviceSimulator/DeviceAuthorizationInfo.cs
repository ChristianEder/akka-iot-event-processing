using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client;

namespace AkkaIoTDeviceSimulator
{
    internal class DeviceAuthorizationInfo
    {
        public string DeviceId { get; set; }
        public string SharedAccessKeySignature { get; set; }

        private static readonly ConcurrentDictionary<string, DeviceAuthorizationInfo> DeviceIds = new ConcurrentDictionary<string, DeviceAuthorizationInfo>();

        public static IEnumerable<DeviceAuthorizationInfo> All => DeviceIds.Values;

        private static RegistryManager _registryManager;

        public static DeviceAuthorizationInfo For(string deviceId)
        {
            return DeviceIds.GetOrAdd(deviceId, Get);
        }

        private static DeviceAuthorizationInfo Get(string deviceId)
        {
            _registryManager = _registryManager ?? RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("akka-iot-hub-admin"));

            var device = _registryManager.GetDeviceAsync(deviceId).Result ??
                         _registryManager.AddDeviceAsync(new Device(deviceId)).Result;

            SharedAccessSignatureBuilder sasBuilder = new SharedAccessSignatureBuilder()
            {
                Key = device.Authentication.SymmetricKey.PrimaryKey,
                Target = $"akka-iot.azure-devices.net/devices/{System.Net.WebUtility.UrlEncode(deviceId)}",
                TimeToLive = TimeSpan.FromDays(Convert.ToDouble(30))
            };

            return new DeviceAuthorizationInfo
            {
                DeviceId = deviceId,
                SharedAccessKeySignature = sasBuilder.ToSignature()
            };
        }
    }
}