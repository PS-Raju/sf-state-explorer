using System.Text;

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

        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append("{");
            stringBuilder.Append("\"Name\" :\"").Append(this.Name).Append("\"");
            stringBuilder.Append("\"Type\" :\"").Append(this.Type).Append("\"");
            stringBuilder.Append("\"Count\" :").Append(this.Count);
            stringBuilder.Append("}");
            return stringBuilder.ToString();
        }

    }
}
