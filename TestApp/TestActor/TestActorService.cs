using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using Microsoft.ServiceFabric.Actors.Runtime;
using SFQuerable.Interface;
using System;
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

        public Task<string> QueryState(ActorId actorId)
        {
            return Task.FromResult("From Actor Test service");
        }
    }
}
