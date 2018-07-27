using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using AkkaIoTDeviceSimulator;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;

namespace AkkaIoTLoadTest
{
    public static class LoadTest
    {
        private static ConcurrentDictionary<string, DeviceSimulator> _devices = new ConcurrentDictionary<string, DeviceSimulator>();
        private static Random Random = new Random();

        [FunctionName("Load01")]
        public static Task Run01([TimerTrigger("* * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Simulate(1);
        }

        [FunctionName("Load02")]
        public static Task Run02([TimerTrigger("* * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Simulate(2);
        }

        [FunctionName("Load03")]
        public static Task Run03([TimerTrigger("* * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Simulate(3);
        }

        [FunctionName("Load04")]
        public static Task Run04([TimerTrigger("* * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Simulate(4);
        }

        [FunctionName("Load05")]
        public static Task Run05([TimerTrigger("* * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Simulate(5);
        }

        [FunctionName("Load06")]
        public static Task Run06([TimerTrigger("* * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Simulate(6);
        }

        [FunctionName("Load07")]
        public static Task Run07([TimerTrigger("* * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Simulate(7);
        }

        [FunctionName("Load08")]
        public static Task Run08([TimerTrigger("* * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Simulate(8);
        }

        [FunctionName("Load09")]
        public static Task Run09([TimerTrigger("* * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Simulate(9);
        }

        [FunctionName("Load10")]
        public static Task Run10([TimerTrigger("* * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Simulate(10);
        }

        [FunctionName("Load11")]
        public static Task Run11([TimerTrigger("* * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Simulate(11);
        }

        [FunctionName("Load12")]
        public static Task Run12([TimerTrigger("* * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Simulate(12);
        }

        [FunctionName("Load13")]
        public static Task Run13([TimerTrigger("* * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Simulate(13);
        }

        [FunctionName("Load14")]
        public static Task Run14([TimerTrigger("* * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Simulate(14);
        }

        [FunctionName("Load15")]
        public static Task Run15([TimerTrigger("* * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Simulate(15);
        }

        [FunctionName("Load16")]
        public static Task Run16([TimerTrigger("* * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Simulate(16);
        }

        [FunctionName("Load17")]
        public static Task Run17([TimerTrigger("* * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Simulate(17);
        }

        [FunctionName("Load18")]
        public static Task Run18([TimerTrigger("* * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Simulate(18);
        }

        [FunctionName("Load19")]
        public static Task Run19([TimerTrigger("* * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Simulate(19);
        }

        [FunctionName("Load20")]
        public static Task Run20([TimerTrigger("* * * * * *")]TimerInfo myTimer, TraceWriter log)
        {
            return Simulate(20);
        }

        private static async Task Simulate(int partition)
        {
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
            await SendMessage(partition);
        }

        private static Task SendMessage(int partition)
        {
            var deviceId = "akka-iot-device-" + ((partition - 1) * 50 + Random.Next(1, 51));
            var simulator = _devices.GetOrAdd(deviceId, d => new DeviceSimulator(d));
            return simulator.Send();
        }
    }
}
