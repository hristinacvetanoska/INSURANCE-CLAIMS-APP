namespace Claims.Services
{
    using Claims.Domain.Models;
    using Claims.Exceptions;
    using Claims.Interfaces;
    using System.ComponentModel.DataAnnotations;

    public class ClaimService : IClaimService
    {
        private readonly IClaimRepository claimRepository;
        private readonly ICoverRepository coverRepository;
        private readonly IAuditer auditer;
        public ClaimService(IClaimRepository claimRepository, ICoverRepository coverRepository, IAuditer auditer)
        { 
            this.claimRepository = claimRepository;
            this.coverRepository = coverRepository;
            this.auditer = auditer;
        }

        public async Task<Claim> CreateAsync(Claim claim)
        {
            claim.Id = Guid.NewGuid().ToString();
            var cover = await this.coverRepository.GetCoverByIdAsync(claim.CoverId);

            if(cover == null)
            {
                throw new NotFoundException($"Cover with id={claim.CoverId} not found.");
            }

            if(claim.Created < cover.StartDate || claim.Created > cover.EndDate)
            {
                throw new ValidationException("Claim date must be within the cover period.");
            }

            await this.claimRepository.AddClaimAsync(claim);
            await this.auditer.AuditClaimAsync(claim.Id, "POST");

            return claim;
        }

        public async Task DeleteAsync(string claimId)
        {
            var claim = await this.claimRepository.GetClaimByIdAsync(claimId);

            if (claim == null)
            {
                throw new NotFoundException($"Claim with id={claimId} not found.");
            }

            await this.claimRepository.DeleteAsync(claim);
            await this.auditer.AuditClaimAsync(claimId, "DELETE");
        }

        public async Task<IEnumerable<Claim>> GetAllAsync()
        {
            return await this.claimRepository.GetClaimsAsync();
        }

        public async Task<Claim> GetByIdAsync(string claimId)
        {
            var claim =  await this.claimRepository.GetClaimByIdAsync(claimId);

            if (claim == null)
            {
                throw new NotFoundException($"Claim with id={claimId} not found.");
            }

            return claim;
        }
    }
}
