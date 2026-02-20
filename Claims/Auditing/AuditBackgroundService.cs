namespace Claims.Auditing
{
    using Claims.Interfaces;
    using Microsoft.Extensions.Hosting;

    public class AuditBackgroundService : BackgroundService
    {
        private readonly IAuditQueue queue;
        private readonly IServiceScopeFactory scopeFactory;

        public AuditBackgroundService(IAuditQueue queue, IServiceScopeFactory scopeFactory)
        {
            this.queue = queue;
            this.scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await foreach (var message in queue.Reader.ReadAllAsync(stoppingToken))
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
                    });
                }
                else
                {
                    await auditContext.CoverAudits.AddAsync(new CoverAudit
                    {
                        CoverId = message.EntityId,
                        Created = DateTime.UtcNow,
                        HttpRequestType = message.HttpRequestType
                    });
                }

                await auditContext.SaveChangesAsync();
            }
        }
    }

}
