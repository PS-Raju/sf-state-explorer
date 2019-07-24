namespace StateExplorer.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Collections.Generic;
    using System.Threading.Tasks;

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
        public async Task<ActionResult<List<string>>> GetAll(string appName, string serviceName, string partitionKey)
        {
            var service = RemotingHelper.GetStatefulServiceHandler(appName, serviceName, partitionKey);
            return await service.QueryState(null);
        }

        // Get state information from stateful svc by name.
        [HttpGet("{appName}/{serviceName}/{partitionKey}/{name}")]
        public async Task<ActionResult<List<string>>> GetByName(string appName, string serviceName, string partitionKey, string name)
        {
            var service = RemotingHelper.GetStatefulServiceHandler(appName, serviceName, partitionKey);
            var response = await service.QueryState(name);
            return response;
        }      
    }
}
