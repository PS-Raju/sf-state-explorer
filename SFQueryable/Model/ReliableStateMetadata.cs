namespace SFQuerable.Model
{
    /// <summary>
    /// Contians the metadata for the IReliableState instances.
    /// </summary>
    public class ReliableStateMetadata
    {
        /// <summary>
        /// Gets the unique name for the IReliableState.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the type of the IReliableState (like IReliableConcurrentQueue,  IReliableDictionary, etc).
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets the number of elements contained in the IReliableState.
        /// </summary>
        public long Count { get; set; }
    }
}
