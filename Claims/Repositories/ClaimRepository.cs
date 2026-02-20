namespace Claims.Repositories
{
    using Claims.Domain.Models;
    using Claims.Interfaces;
    using Microsoft.EntityFrameworkCore;

    /// <summary>
    /// The class for Claim repository
    /// </summary>
    public class ClaimRepository : IClaimRepository
    {
        private readonly ClaimsContext claimsContext;

        /// <summary>
        /// ClaimRepository constructor
        /// </summary>
        /// <param name="claimsContext">The claims context.</param>
        public ClaimRepository(ClaimsContext claimsContext)
        {
            this.claimsContext = claimsContext;
        }

        /// <summary>
        /// Gets claims asynchronously from the data store.
        /// </summary>
        /// <returns>List of claims.</returns>
        public async Task<IEnumerable<Claim>> GetClaimsAsync()
        {
            return await this.claimsContext.Claims.ToListAsync();
        }

        /// <summary>
        /// Gets thw claim by id.
        /// </summary>
        /// <param name="claimId">The claim id.</param>
        /// <returns>The claim.</returns>
        public async Task<Claim?> GetClaimByIdAsync(string claimId)
        {
            return await this.claimsContext.Claims
                           .Where(claim => claim.Id == claimId)
                           .SingleOrDefaultAsync();
        }

        /// <summary>
        /// Adds a new claim to the data base asynchronously.
        /// </summary>
        /// <param name="claim">The claim.</param>
        /// <returns></returns>
        public async Task AddClaimAsync(Claim claim)
        {
            await this.claimsContext.Claims.AddAsync(claim);
            await this.claimsContext.SaveChangesAsync();
        }


        /// <summary>
        /// Deletes the claim by id.
        /// </summary>
        /// <param name="claim">The claim.</param>
        /// <returns></returns>
        public async Task DeleteAsync(Claim claim)
        {
            this.claimsContext.Claims.Remove(claim);
            await this.claimsContext.SaveChangesAsync();
        }
    }
}
