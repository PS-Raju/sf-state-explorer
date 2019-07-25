using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace TestStatefulService.Service
{
    public class ReliableQueueService : IReliableQueueService
    {
        private readonly IReliableStateManager stateManager;

        private readonly string queueName;
        public ReliableQueueService(string queueName, IReliableStateManager stateManager)
        {
            this.queueName = queueName;
            this.stateManager = stateManager;
        }

        public void PopulateReliableQueue(object cancellationToken)
        {
            this.Populate((CancellationToken)cancellationToken).Wait();
        }

        private async Task Populate(CancellationToken cancellationToken)
        {
            IReliableConcurrentQueue<string> list = await this.GetQueueAsync();
            using (ITransaction tx = this.stateManager.CreateTransaction())
            {
                await list.EnqueueAsync(tx, Guid.NewGuid().ToString(), cancellationToken);
                await tx.CommitAsync();
            }
            using (ITransaction tx = this.stateManager.CreateTransaction())
            {
                await list.EnqueueAsync(tx, Guid.NewGuid().ToString(), cancellationToken);
                await tx.CommitAsync();
            }
            using (ITransaction tx = this.stateManager.CreateTransaction())
            {
                await list.TryDequeueAsync(tx, cancellationToken);
                await tx.CommitAsync();
            }
        }

        private async Task<IReliableConcurrentQueue<string>> GetQueueAsync()
        {
            IReliableConcurrentQueue<string> queue = null;
            try
            {
                queue = await this.stateManager.GetOrAddAsync<IReliableConcurrentQueue<string>>(this.queueName);
            }
            catch (Exception)
            {
            }

            return queue;
        }
    }
}
