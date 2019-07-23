using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Query;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Data;
using Newtonsoft.Json;
using SFQuerable.Interface;
using SFQuerable.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SFQuerable
{
    public class ActorStateProviderExtensions : IActorStateProviderExtensions
    {
        public async Task<string> GetActorDetails(IActorStateProvider actorStateProvider, ActorId actorId, CancellationToken cancellationToken)
        {
            /*
            IEnumerable<string> stateKeys = await actorStateProvider.EnumerateStateNamesAsync(actorId, cancellationToken);
            List<string> keys = (List<string>)stateKeys;
            var actorDetails = new Dictionary<string, object>();

            foreach (string key in keys)
            {
                object value = await actorStateProvider.LoadStateAsync<object>(actorId, key, cancellationToken);
                actorDetails.Add(key, value);
            }

            return JsonConvert.SerializeObject(actorDetails);
            */
            await Task.CompletedTask;
            throw new NotImplementedException();
        }

        public async Task<string> GetActorsInPartition(IActorStateProvider actorStateProvider, CancellationToken cancellationToken)
        {
            List<ActorId> actorsInPartition = await GetAllActorsInCurrentPartitionAsync(actorStateProvider, cancellationToken);
            var actorPartitionMetadata = new ActorPartitionMetadata
            {
                count = actorsInPartition.Count,
                actors = actorsInPartition
            };
            return JsonConvert.SerializeObject(actorPartitionMetadata);
        }

        /// <summary>
        /// Returns all actors in the current partition.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task containing list of actorIds.</returns>
        private async Task<List<ActorId>> GetAllActorsInCurrentPartitionAsync(IActorStateProvider actorStateProvider, CancellationToken cancellationToken)
        {
            var actorsInPartition = new List<ActorId>();
            ContinuationToken continuationToken = null;
            do
            {
                PagedResult<ActorId> actors = null;
                actors = await actorStateProvider.GetActorsAsync(100, continuationToken, cancellationToken);
                actorsInPartition.AddRange(actors.Items);
                continuationToken = actors.ContinuationToken;
            }
            while (continuationToken != null);

            actorsInPartition = actorsInPartition.Distinct().ToList();
            return actorsInPartition;
        }
    }
}
