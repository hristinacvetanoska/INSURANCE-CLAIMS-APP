namespace Claims.Auditing
{
    using Claims.Interfaces;
    using Microsoft.Extensions.Hosting;

    /// <summary>
    /// Background service that consumes audit messages from the in-memory queue and persists them to the AuditContext.
    /// This implementation is resilient: it catches and logs exceptions per-message so a single failing save does not stop the service,
    /// respects cancellation via the provided <see cref="CancellationToken"/>, and uses the DI scope factory per message.
    /// </summary>
    public class AuditBackgroundService : BackgroundService
    {
        private readonly IAuditQueue queue;
        private readonly IServiceScopeFactory scopeFactory;
        private readonly ILogger<AuditBackgroundService> logger;

        public AuditBackgroundService(IAuditQueue queue, IServiceScopeFactory scopeFactory, ILogger<AuditBackgroundService> logger)
        {
            this.queue = queue;
            this.scopeFactory = scopeFactory;
            this.logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var message in queue.Reader.ReadAllAsync(stoppingToken))
            {
                try
                {
                    using var scope = scopeFactory.CreateScope();
                    var auditContext = scope.ServiceProvider.GetRequiredService<AuditContext>();

                    if (message.EntityType == "Claim")
                    {
                        await auditContext.ClaimAudits.AddAsync(new ClaimAudit
                        {
                            ClaimId = message.EntityId,
                            Created = DateTime.UtcNow,
                            HttpRequestType = message.HttpRequestType
                        }, stoppingToken);
                    }
                    else
                    {
                        await auditContext.CoverAudits.AddAsync(new CoverAudit
                        {
                            CoverId = message.EntityId,
                            Created = DateTime.UtcNow,
                            HttpRequestType = message.HttpRequestType
                        }, stoppingToken);
                    }

                    await auditContext.SaveChangesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to persist audit message: {EntityType} {EntityId}", message.EntityType, message.EntityId);
                }

            }
        }
    }

}
