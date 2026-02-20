namespace Claims.Interfaces
{
    using Claims.Auditing;
    using System.Threading.Channels;

    /// <summary>
    /// The interface for an audit queue that allows enqueuing audit messages and provides a reader for processing them.
    /// </summary>
    public interface IAuditQueue
    {
        /// <summary>
        /// Adds the specified audit message to the queue for processing.
        /// </summary>
        /// <param name="message">The audit message to enqueue. Cannot be null.</param>
        void Enqueue(AuditMessage message);

        /// <summary>
        /// Gets the channel reader used to receive audit messages as they become available.
        /// </summary>
        ChannelReader<AuditMessage> Reader { get; }
    }
}
