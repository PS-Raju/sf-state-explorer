using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.FabricTransport;
using System.Threading.Tasks;

namespace SFQuerable.Interface
{
    public interface IStatefulActorService : IService
    {
        Task<string> QueryState(string actorId);
    }
}
