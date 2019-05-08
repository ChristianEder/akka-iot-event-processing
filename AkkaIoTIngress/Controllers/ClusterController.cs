using System;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using AkkaIoTIngress.Actors.Machine;
using Microsoft.AspNetCore.Mvc;

namespace AkkaIoTIngress.Controllers
{
    [Produces("application/json")]
    [Route("api/Cluster")]
    public class ClusterController : Controller
    {
        private readonly ActorSystem _actorSystem;

        public ClusterController(ActorSystem actorSystem)
        {
            _actorSystem = actorSystem;
        }

        [HttpGet("machine")]
        public async Task<IActionResult> Machine()
        {
            var actor = _actorSystem.ActorOf(_actorSystem.DI().Props<MachineActor>(), "akka-iot-backend-machine");
            var name = await actor.Ask<MachineActor.MachineName>(new MachineActor.WhatsTheMachine());
            return Ok(name.Name);
        }
    }
}