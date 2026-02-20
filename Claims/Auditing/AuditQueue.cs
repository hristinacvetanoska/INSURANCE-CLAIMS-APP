using Claims.Interfaces;
using System.Threading.Channels;

namespace Claims.Auditing
{
    public class AuditQueue : IAuditQueue
    {
        private readonly Channel<AuditMessage> channel = Channel.CreateUnbounded<AuditMessage>();

        public void Enqueue(AuditMessage message)
        {
            channel.Writer.TryWrite(message);
        }

        public ChannelReader<AuditMessage> Reader => channel.Reader;
    }

}
