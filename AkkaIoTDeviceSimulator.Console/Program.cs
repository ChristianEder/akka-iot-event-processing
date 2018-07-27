using System;
using System.Linq;

namespace AkkaIoTDeviceSimulator.Console
{
    class Program
    {
        static void Main(string[] args)
        {
           var devices = Enumerable.Range(1, 50).Select(i => "akka-iot-device-" + i).Select(d => new DeviceSimulator(d, 100)).ToArray();
            foreach (var device in devices)
            {
                device.Start();
            }

            System.Console.ReadLine();
        }
    }
}
