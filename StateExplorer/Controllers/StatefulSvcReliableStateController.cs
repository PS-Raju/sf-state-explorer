using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace StateExplorer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatefulSvcReliableStateController : ControllerBase
    {
        // launch url 
        [HttpGet]
        public ActionResult<string> Get()
        {
            return "State Explorer is up and running";
        }

        // Get all state information from stateful svc 
        [HttpGet("{appName}/{serviceName}/{partitionKey}")]
        public ActionResult<string> GetAll(string appName, string serviceName, string partitionKey)
        {
            return appName + "-" + serviceName + "-" + partitionKey;
        }

        // Get state information from stateful svc by name.
        [HttpGet("{appName}/{serviceName}/{partitionKey}/{name}")]
        public ActionResult<string> GetByName(string appName, string serviceName, string partitionKey, string name)
        {
            return appName + "-" + serviceName + "-" + partitionKey + "-"+name;
        }      
    }
}
