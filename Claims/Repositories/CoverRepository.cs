namespace Claims.Repositories
{
    using Claims.Domain.Models;
    using Claims.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class CoverRepository : ICoverRepository
    {
        private readonly ClaimsContext claimsContext;
        public CoverRepository(ClaimsContext claimsContext)
        {
            this.claimsContext = claimsContext;
        }

        /// <summary>
        /// Retrieves a cover by its identifier.
        /// </summary>
        /// <param name="coverId">The cover identifier.</param>
        public async Task<Cover?> GetCoverByIdAsync(string coverId)
        {
            return await this.claimsContext.Covers.Where(cover => cover.Id == coverId).SingleOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves all covers.
        /// </summary>
        public async Task<IEnumerable<Cover>> GetCoversAsync()
        {
            return await this.claimsContext.Covers.ToListAsync();
        }

        /// <summary>
        /// Adds a new cover.
        /// </summary>
        /// <param name="cover">The cover to add.</param>
        public async Task AddCoverAsync(Cover cover)
        {
            await this.claimsContext.Covers.AddAsync(cover);
            await this.claimsContext.SaveChangesAsync();
        }

        /// <summary>
        /// Deletes an existing cover.
        /// </summary>
        /// <param name="cover">The cover to delete.</param>
        public async Task DeleteCoverAsync(Cover cover)
        {
            this.claimsContext.Covers.Remove(cover);
            await this.claimsContext.SaveChangesAsync();
        }
    }
}
