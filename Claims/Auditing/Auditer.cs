namespace Claims.Auditing
{
    using Claims.Interfaces;

    public class Auditer : IAuditer
    {
        private readonly AuditContext auditContext;

        public Auditer(AuditContext auditContext)
        {
            this.auditContext = auditContext;
        }

        public async Task AuditClaimAsync(string id, string httpRequestType)
        {
            var claimAudit = new ClaimAudit()
            {
                Created = DateTime.UtcNow,
                HttpRequestType = httpRequestType,
                ClaimId = id
            };

            await this.auditContext.AddAsync(claimAudit);
            await this.auditContext.SaveChangesAsync();
        }
        
        public async Task AuditCoverAsync(string id, string httpRequestType)
        {
            var coverAudit = new CoverAudit()
            {
                Created = DateTime.UtcNow,
                HttpRequestType = httpRequestType,
                CoverId = id
            };

            await this.auditContext.AddAsync(coverAudit);
            await this.auditContext.SaveChangesAsync();
        }
    }
}
