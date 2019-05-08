using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AkkaIoTIngress.Controllers
{
    [Produces("application/json")]
    [Route("api/Cluster")]
    public class ClusterController : Controller
    {
        [HttpGet("machine")]
        public async Task<string> Machine()
        {
            return Environment.MachineName;
        }
    }
}