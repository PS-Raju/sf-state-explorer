namespace StateExplorer.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.ServiceFabric.Actors;
    using System.Threading.Tasks;

    [Route("api/[controller]")]
    [ApiController]
    public class ActorSvcReliableStateController : ControllerBase
    {
        // Get all state information from actor svc 
        [HttpGet("{appName}/{actorSvcName}/{partitionKey}")]
        public async Task<ActionResult<string>> GetAll(string appName, string actorSvcName, string partitionKey)
        {
            var service = RemotingHelper.GetActorServiceHandler(appName, actorSvcName, partitionKey);
            var response = await service.QueryState(null);
            return response;
        }

        // Get state information from actor svc by name.
        [HttpGet("{appName}/{actorSvcName}/{partitionKey}/{actorId}")]
        public async Task<ActionResult<string>> GetByName(string appName, string actorSvcName, string partitionKey, string actorId)
        {
            var service = RemotingHelper.GetActorServiceHandler(appName, actorSvcName, partitionKey);
            var response = await service.QueryState(actorId);
            return response;
        }      
    }
}
