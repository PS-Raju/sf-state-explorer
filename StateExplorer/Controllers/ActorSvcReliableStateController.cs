namespace StateExplorer.Controllers
{
    using Microsoft.AspNetCore.Mvc;

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
        [HttpGet("{appName}/{actorSvcName}/{partitionKey}/{actorId}")]
        public ActionResult<string> GetByName(string appName, string actorSvcName, string partitionKey, string actorId)
        {
            return appName + "-" + actorSvcName + "-" + partitionKey + "-"+ actorId;
        }      
    }
}
