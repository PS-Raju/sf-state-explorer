namespace StateExplorer.Controllers
{
    using Microsoft.AspNetCore.Mvc;

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
            var service = RemotingHelper.GetStatefulServiceHandler(appName, serviceName, partitionKey);
            return service.QueryState(null).Result;
        }

        // Get state information from stateful svc by name.
        [HttpGet("{appName}/{serviceName}/{partitionKey}/{name}")]
        public ActionResult<string> GetByName(string appName, string serviceName, string partitionKey, string name)
        {
            var service = RemotingHelper.GetStatefulServiceHandler(appName, serviceName, partitionKey);
            return service.QueryState(null).Result;
        }      
    }
}
