using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SFQuerable.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace SFQuerable
{
    public static class ReliableStateManagerExtensions
    {
        /// <summary>
		/// Get the names, ypes and count of the reliable collections that are queryable from the reliable state manager.
		/// </summary>
		/// <param name="stateManager">Reliable state manager for the replica.</param>
		/// <returns>Reliable.</returns>
        public static async Task<string> GetMetadataAsync(this IReliableStateManager stateManager)
        {
            ArrayList metadataList = new ArrayList();
            using (IAsyncEnumerator<IReliableState> reliableStatesEnumerator = stateManager.GetAsyncEnumerator())
            {
                while (await reliableStatesEnumerator.MoveNextAsync(CancellationToken.None))
                {
                    IReliableState reliableState = reliableStatesEnumerator.Current;
                    ReliableStateMetadata metadata = new ReliableStateMetadata
                    {
                        Name = reliableState.Name.AbsolutePath,
                        Type = reliableState.GetType().FullName
                    };
                    metadataList.Add(metadata);
                }
            }
            
            return JsonConvert.SerializeObject(metadataList);
        }

        /// <summary>
		/// Query the reliable collection with the given name from the reliable state manager using the given query parameters.
		/// </summary>
		/// <param name="stateManager">Reliable state manager for the replica.</param>
		/// <param name="reliableStateName">Name of the reliable state to be queried.</param>
		/// <returns>The json serialized results of the query.</returns>
		public static async Task<string> QueryAsync(this IReliableStateManager stateManager, string reliableStateName)
        {
            using (var tx = stateManager.CreateTransaction())
            {
                IReliableState reliableState = await stateManager.GetQueryableState(reliableStateName).ConfigureAwait(false);

                // Get the data from the reliable state
                var results = await reliableState.GetAsyncEnumerable(tx, stateManager).ConfigureAwait(false);

                // Convert to json
                var json = await results.SelectAsync(r => JObject.FromObject(r)).AsEnumerable().ConfigureAwait(false);

                await tx.CommitAsync().ConfigureAwait(false);

                // Return the data as json string
                return json.ToString();
            }
        }

        /// <summary>
		/// Gets the values from the reliable state as the <see cref="Entity{TKey, TValue}"/> of the collection.
		/// </summary>
		/// <param name="state">Reliable state (must implement <see cref="IReliableDictionary{TKey, TValue}"/>).</param>
		/// <param name="tx">Transaction to create the enumerable under.</param>
		/// <returns>Values from the reliable state as <see cref="Entity{TKey, TValue}"/> values.</returns>
		private static async Task<IAsyncEnumerable<object>> GetAsyncEnumerable(this IReliableState state,
            ITransaction tx, IReliableStateManager stateManager)
        {
            if (!state.ImplementsGenericType(typeof(IReliableDictionary<,>)))
                throw new ArgumentException(nameof(state));

            var entityType = state.GetEntityType();

            // Create the async enumerable.
            var dictionaryType = typeof(IReliableDictionary<,>).MakeGenericType(state.GetType().GetGenericArguments());
            var createEnumerableAsyncTask = state.CallMethod<Task>("CreateEnumerableAsync", new[] { typeof(ITransaction) }, tx);
            await createEnumerableAsyncTask.ConfigureAwait(false);

            // Get the AsEntity method to convert to an Entity enumerable.
            var asyncEnumerable = createEnumerableAsyncTask.GetPropertyValue<object>("Result");
            var asEntityMethod = typeof(ReliableStateManagerExtensions).GetMethod("AsEntity", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(entityType.GenericTypeArguments);
            return (IAsyncEnumerable<object>)asEntityMethod.Invoke(null, new object[] { asyncEnumerable });
        }

        /// <summary>
		/// Lazily convert the reliable state enumerable into a queryable <see cref="KeyValuePair{TKey, TValue}"/> enumerable.
		/// /// </summary>
		/// <param name="source">Reliable state enumerable.</param>
		/// <returns>The</returns>
		private static IAsyncEnumerable<KeyValuePair<TKey, TValue>> AsEntity<TKey, TValue>(this IAsyncEnumerable<KeyValuePair<TKey, TValue>> source)
        {
            return source.SelectAsync(kvp => new KeyValuePair<TKey, TValue>(kvp.Key, kvp.Value));
        }

        /// <summary>
		/// Get the Entity model type from the reliable dictionary.
		/// This is the full metadata type definition for the rows in the
		/// dictionary (key, value, partition, etag).
		/// </summary>
		/// <param name="state">Reliable dictionary instance.</param>
		/// <returns>The Entity model type for the dictionary.</returns>
		private static Type GetEntityType(this IReliableState state)
        {
            var keyType = state.GetKeyType();
            var valueType = state.GetValueType();
            return typeof(KeyValuePair<,>).MakeGenericType(keyType, valueType);
        }

        /// <summary>
        /// Get the key type from the reliable dictionary.
        /// </summary>
        /// <param name="state">Reliable dictionary instance.</param>
        /// <returns>The type of the dictionary's keys.</returns>
        private static Type GetKeyType(this IReliableState state)
        {
            if (!state.ImplementsGenericType(typeof(IReliableDictionary<,>)))
                throw new ArgumentException(nameof(state));

            return state.GetType().GetGenericArguments()[0];
        }

        /// <summary>
        /// Get the value type from the reliable dictionary.
        /// </summary>
        /// <param name="state">Reliable dictionary instance.</param>
        /// <returns>The type of the dictionary's values.</returns>
        private static Type GetValueType(this IReliableState state)
        {
            if (!state.ImplementsGenericType(typeof(IReliableDictionary<,>)))
                throw new ArgumentException(nameof(state));

            return state.GetType().GetGenericArguments()[1];
        }

        /// <summary>
		/// Get the queryable reliable collection by name.
		/// </summary>
		/// <param name="stateManager">Reliable state manager for the replica.</param>
		/// <param name="reliableStateName">Name of the reliable state.</param>
		/// <returns>The reliable collection that supports querying.</returns>
		private static async Task<IReliableState> GetQueryableState(this IReliableStateManager stateManager, string reliableStateName)
        {
            // Find the reliable state.
            var reliableStateResult = await stateManager.TryGetAsync<IReliableState>(reliableStateName).ConfigureAwait(false);
            if (!reliableStateResult.HasValue)
            {
                throw new ArgumentException($"IReliableState '{reliableStateName}' not found in this state manager.");
            }

            // Verify the state is a reliable dictionary.
            var reliableState = reliableStateResult.Value;
            if (!reliableState.ImplementsGenericType(typeof(IReliableDictionary<,>)))
            {
                throw new ArgumentException($"IReliableState '{reliableStateName}' must be an IReliableDictionary.");
            }

            return reliableState;
        }

    }
}
