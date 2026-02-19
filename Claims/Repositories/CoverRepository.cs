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

        public async Task<Cover> GetCoverByIdAsync(string coverId)
        {
            return await this.claimsContext.Covers.Where(cover => cover.Id == coverId).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<Cover>> GetCoversAsync()
        {
            return await this.claimsContext.Covers.ToListAsync();
        }
        public async Task AddCoverAsync(Cover cover)
        {
            await this.claimsContext.Covers.AddAsync(cover);
            await this.claimsContext.SaveChangesAsync();
        }

        public async Task DeleteCoverAsync(Cover cover)
        {
            this.claimsContext.Covers.Remove(cover);
            await this.claimsContext.SaveChangesAsync();
        }
    }
}
