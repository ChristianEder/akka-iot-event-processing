using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.Actor.Dsl;
using AkkaIoTIngress.Services;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;

namespace AkkaIoTIngress.Actors.Ingress
{
    public class EventProcessor : IEventProcessor
    {
        private readonly ActorSystem _actorSystem;
        private readonly ITableStorage _tableStorage;

        public EventProcessor(ActorSystem actorSystem, ITableStorage tableStorage)
        {
            _actorSystem = actorSystem;
            _tableStorage = tableStorage;
        }

        public Task CloseAsync(PartitionContext context, CloseReason reason)
        {
            Console.WriteLine($"Processor Shutting Down. Partition '{context.PartitionId}', Reason: '{reason}'.");
            return Task.CompletedTask;
        }

        public Task OpenAsync(PartitionContext context)
        {
            Console.WriteLine($"SimpleEventProcessor initialized. Partition: '{context.PartitionId}'");
            return Task.CompletedTask;
        }

        public Task ProcessErrorAsync(PartitionContext context, Exception error)
        {
            Console.WriteLine($"Error on Partition: {context.PartitionId}, Error: {error.Message}");
            return Task.CompletedTask;
        }

        public Task ProcessEventsAsync(PartitionContext context, IEnumerable<EventData> messages)
        {
            return ProcessEventsAsync(context, messages.ToArray(), 3);
        }

        private async Task ProcessEventsAsync(PartitionContext context, EventData[] messages, int retries)
        {
            try
            {
                var tasks = new List<Task<bool>>();

                var actorRef = _actorSystem.ActorSelection("akka://user/akka-iot-ingress");
                foreach (var eventData in messages)
                {
                    tasks.Add(actorRef.Ask<bool>(new IngressActor.DispatchEventData { EventData = eventData, PartitionId = context.PartitionId }));
                }

                var success = await Task.WhenAll(tasks);
                if (!success.All(s => s))
                {
                    throw new Exception("At least one message could not be processed");
                }

                await _tableStorage.CompleteBatch(context.PartitionId);

                await Checkpoint(context);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                if (retries > 0)
                {
                    await ProcessEventsAsync(context, messages, retries - 1);
                }
                else
                {
                    throw;
                }

            }
        }

        private static async Task Checkpoint(PartitionContext context)
        {
            try
            {
                await context.CheckpointAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}