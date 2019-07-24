using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Data;
using TestStatefulService.Service;
using Microsoft.ServiceFabric.Services.Remoting.Runtime;
using SFQuerable;

namespace TestStatefulService
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    internal sealed class TestStatefulService : StatefulService, ITestStatefulService
    {
        public TestStatefulService(StatefulServiceContext context, IReliableStateManagerReplica2 stateManager)
            : base(context, stateManager)
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
            await Task.Delay(120000);
            this.CreateTimerForReliableDictionary("sampleDictionary1", TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(2), cancellationToken);
            await Task.Delay(10000);
            this.CreateTimerForReliableDictionary("sampleDictionary2", TimeSpan.FromMilliseconds(4), TimeSpan.FromSeconds(10), cancellationToken);
            await Task.Delay(10000);
            this.CreateTimerForReliableQueue("sampleQueue1", TimeSpan.FromMilliseconds(2), TimeSpan.FromSeconds(15), cancellationToken);
            await Task.Delay(10000);
            this.CreateTimerForReliableQueue("sampleQueue2", TimeSpan.FromMilliseconds(3), TimeSpan.FromSeconds(30), cancellationToken);

            await Task.CompletedTask;
        }

        private void CreateTimerForReliableQueue(string queueName, TimeSpan dueTime, TimeSpan period, CancellationToken cancellationToken)
        {
            IReliableQueueService reliableDictionaryService = new ReliableQueueService(queueName, this.StateManager);
            Timer reliableDictionaryTimer = new Timer(
                reliableDictionaryService.PopulateReliableQueue,
                cancellationToken,
                dueTime,
                period);
            // Register callback to stop timer during the service shutdown or primary swap. 
            cancellationToken.Register(() => reliableDictionaryTimer.Dispose());
        }

        private void CreateTimerForReliableDictionary(string dictionaryName, TimeSpan dueTime, TimeSpan period, CancellationToken cancellationToken)
        {
            IReliableDictionaryService reliableDictionaryService = new ReliableDictionaryService(dictionaryName, this.StateManager);
            Timer reliableDictionaryTimer = new Timer(
                reliableDictionaryService.PopulateReliableDictionary,
                cancellationToken,
                dueTime,
                period);
            // Register callback to stop timer during the service shutdown or primary swap. 
            cancellationToken.Register(() => reliableDictionaryTimer.Dispose());
        }

        public async Task<List<string>> QueryState(string reliableCollectionsName)
        {
            if (reliableCollectionsName == null)
            {
                return await ReliableStateManagerExtensions.GetMetadataAsync(this.StateManager);
            }
            else
            {
                return await ReliableStateManagerExtensions.QueryAsync(this.StateManager, reliableCollectionsName);
            }

        }
    }
}
