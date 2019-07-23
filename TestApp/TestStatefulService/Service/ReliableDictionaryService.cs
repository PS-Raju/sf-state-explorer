namespace TestStatefulService.Service
{
    using Microsoft.ServiceFabric.Data;
    using Microsoft.ServiceFabric.Data.Collections;
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class ReliableDictionaryService : IReliableDictionaryService
    {
        private readonly IReliableStateManager stateManager;

        private readonly string listName;

        public ReliableDictionaryService(string dictionaryName, IReliableStateManager stateManager)
        {
            this.listName = dictionaryName;
            this.stateManager = stateManager;
        }

        public void PopulateReliableDictionary(object cancellationToken)
        {
            this.Populate((CancellationToken) cancellationToken).Wait();
        }

        private async Task Populate(CancellationToken cancellationToken)
        {
            IReliableDictionary<string, string> list = await this.GetListAsync();
            using (ITransaction tx = this.stateManager.CreateTransaction())
            {
                await list.AddAsync(tx, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
                await tx.CommitAsync();
            }
        }

        public async Task<IReliableDictionary<string, string>> GetListAsync()
        {
            IReliableDictionary<string, string> dict = null;
            try
            {
                dict = await this.stateManager.GetOrAddAsync<IReliableDictionary<string, string>>(this.listName);
            }
            catch (Exception e)
            {
                string errorMessage = $"Get List failed.";

            }

            return dict;
        }
    }
}
