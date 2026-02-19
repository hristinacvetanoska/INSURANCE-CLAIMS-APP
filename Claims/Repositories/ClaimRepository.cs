namespace Claims.Repositories
{
    using Claims.Domain.Models;
    using Claims.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class ClaimRepository : IClaimRepository
    {
        private readonly ClaimsContext claimsContext;

        public ClaimRepository(ClaimsContext claimsContext)
        {
            this.claimsContext = claimsContext;
        }

        public async Task<IEnumerable<Claim>> GetClaimsAsync()
        {
            return await this.claimsContext.Claims.ToListAsync();
        }
        public async Task<Claim> GetClaimByIdAsync(string claimId)
        {
            return await this.claimsContext.Claims
                           .Where(claim => claim.Id == claimId)
                           .SingleOrDefaultAsync();
        }
        public async Task AddClaimAsync(Claim claim)
        {
            await this.claimsContext.Claims.AddAsync(claim);
            await this.claimsContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Claim claim)
        {
            this.claimsContext.Claims.Remove(claim);
            await this.claimsContext.SaveChangesAsync();
        }
    }
}
