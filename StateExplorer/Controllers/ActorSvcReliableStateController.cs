using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StateExplorer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorSvcReliableStateController : ControllerBase
    {
        // Get all state information from actor svc 
        [HttpGet("{appName}/{actorSvcName}/{partitionKey}")]
        public ActionResult<string> GetAll(string appName, string actorSvcName, string partitionKey)
        {
            return appName + "-" + actorSvcName + "-" + partitionKey;
        }

        // Get state information from actor svc by name.
        [HttpGet("{appName}/{actorSvcName}/{partitionKey}/{name}")]
        public ActionResult<string> GetByName(string appName, string actorSvcName, string partitionKey, string name)
        {
            return appName + "-" + actorSvcName + "-" + partitionKey + "-"+name;
        }      
    }
}
