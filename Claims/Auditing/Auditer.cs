namespace Claims.Auditing
{
    using Claims.Interfaces;

    /// <summary>
    /// Provides functionality for auditing operations on claims and covers.
    /// </summary>
    public class Auditer : IAuditer
    {
        private readonly IAuditQueue queue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Auditer"/> class with a specified audit queue.
        /// </summary>
        /// <param name="queue">The audit queue used to enqueue audit messages.</param>
        public Auditer(IAuditQueue queue)
        {
            this.queue = queue;
        }
        /// <summary>
        /// Audits an operation performed on a claim.
        /// </summary>
        /// <param name="id">The unique identifier of the claim.</param>
        /// <param name="httpRequestType">The type of HTTP request (e.g., GET, POST, PUT, DELETE) that triggered the audit.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns></param>
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


        /// <summary>
        /// Audits an operation performed on a cover.
        /// </summary>
        /// <param name="id">The unique identifier of the cover.</param>
        /// <param name="httpRequestType">The type of HTTP request (e.g., GET, POST, PUT, DELETE) that triggered the audit.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
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
