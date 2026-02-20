namespace Claims.Interfaces
{
    using Claims.Auditing;
    using System.Threading.Channels;

    public interface IAuditQueue
    {
        void Enqueue(AuditMessage message);
        ChannelReader<AuditMessage> Reader { get; }
    }
}
