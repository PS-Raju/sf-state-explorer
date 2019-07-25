using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using SFQuerable;
using SFQuerable.Interface;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using TestActor.Interfaces;

namespace TestActor
{
    internal class TestActorService : ActorService, IStatefulActorService
    {
        public TestActorService(
            StatefulServiceContext context, 
            ActorTypeInformation actorTypeInfo, 
            Func<ActorService, ActorId, ActorBase> actorFactory = null, 
            Func<ActorBase, IActorStateProvider, IActorStateManager> stateManagerFactory = null, 
            IActorStateProvider stateProvider = null, 
            ActorServiceSettings settings = null) 
            : base(
                  context, 
                  actorTypeInfo, 
                  actorFactory, 
                  stateManagerFactory, 
                  stateProvider, 
                  settings)
        {
        }


        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return this.CreateServiceRemotingReplicaListeners();
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            this.CreateTimerForActorCreation(TimeSpan.FromMilliseconds(1), TimeSpan.FromSeconds(30), cancellationToken);
            
            await Task.CompletedTask;
        }

        private void CreateTimerForActorCreation(TimeSpan dueTime, TimeSpan period, CancellationToken cancellationToken)
        {
            Timer timer = new Timer(
                this.CreateActor,
                cancellationToken,
                dueTime,
                period);
            // Register callback to stop timer during the service shutdown or primary swap. 
            cancellationToken.Register(() => timer.Dispose());
        }

        private void CreateActor(object cancellationToken)
        {
            ActorId actorId = ActorId.CreateRandom();
            ITestActor testActor = ActorProxy.Create<ITestActor>(actorId);
            testActor.SetCountAsync(10, (CancellationToken) cancellationToken).Wait();
        }

        public async Task<string> QueryState(string actorId)
        {
            if (actorId == null)
            {
                return await ActorStateProviderExtensions.GetActorsInPartition(this.StateProvider, default(CancellationToken));
            }
            else
            {
                return await ActorStateProviderExtensions.GetActorDetails(this.StateProvider, new ActorId(actorId), default(CancellationToken));
            }
        }
    }
}
