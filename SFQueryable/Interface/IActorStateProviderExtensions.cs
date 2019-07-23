using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using System.Threading;
using System.Threading.Tasks;

namespace SFQuerable.Interface
{
    public interface IActorStateProviderExtensions
    {
        Task<string> GetActorDetails(IActorStateProvider actorStateProvider, ActorId actorId, CancellationToken cancellationToken);

        Task<string> GetActorsInPartition(IActorStateProvider actorStateProvider, CancellationToken cancellationToken);
    }
}
