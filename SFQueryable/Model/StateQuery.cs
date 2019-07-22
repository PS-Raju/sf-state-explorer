namespace SFQuerable.Model
{
    public class StateQuery
    {
        // Mandate parameter.
        public string ServiceName { get; set; }

        // Optional, if porvided will query this partition else all partitions.
        public string PartitionKey { get; set; }

        // Types could be "ReliableDictionary", "ReliableQueue", "ReliableConcurrentQueue"
        public string CollectionType { get; set; }

        // Name of collection
        public string ReliableStateName { get; set; }

        // In case if above state is a dictioanry, use this key to query the value.
        public string Key { get; set; }
    }
}
