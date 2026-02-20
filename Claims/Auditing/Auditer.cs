namespace Claims.Auditing
{
    using Claims.Interfaces;

    public class Auditer : IAuditer
    {
        private readonly IAuditQueue queue;

        public Auditer(IAuditQueue queue)
        {
            this.queue = queue;
        }

        public Task AuditClaimAsync(string id, string httpRequestType)
        {
            queue.Enqueue(new AuditMessage
            {
                EntityId = id,
                HttpRequestType = httpRequestType,
                EntityType = "Claim"
            });

            return Task.CompletedTask;
        }
        
        public Task AuditCoverAsync(string id, string httpRequestType)
        {
            queue.Enqueue(new AuditMessage
            {
                EntityId = id,
                HttpRequestType = httpRequestType,
                EntityType = "Cover"
            });

            return Task.CompletedTask;
        }
    }
}
