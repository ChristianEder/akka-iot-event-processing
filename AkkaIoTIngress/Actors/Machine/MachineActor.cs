using System;
using Akka.Actor;

namespace AkkaIoTIngress.Actors.Machine
{
    public class MachineActor : ReceiveActor
    {
        public MachineActor()
        {
            Receive<WhatsTheMachine>(OnReceive);
        }

        private bool OnReceive(WhatsTheMachine message)
        {
            var name = new MachineName { Name = Environment.MachineName };
            Sender.Tell(name);
            return true;
        }

        public class WhatsTheMachine { }
        public class MachineName
        {
            public string Name { get; set; }
        }
    }
}
