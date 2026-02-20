namespace Claims.Services
{
    using Claims.Domain.Models;
    using Claims.Exceptions;
    using Claims.Interfaces;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Provides operations for managing claims.
    /// </summary>
    public class ClaimService : IClaimService
    {
        private readonly IClaimRepository claimRepository;
        private readonly ICoverRepository coverRepository;
        private readonly IAuditer auditer;

        /// <summary>
        /// The constructor for Claim Service.
        /// </summary>
        /// <param name="claimRepository">The claim repository.</param>
        /// <param name="coverRepository">The cover repository.</param>
        /// <param name="auditer">The auditer.</param>
        public ClaimService(IClaimRepository claimRepository, ICoverRepository coverRepository, IAuditer auditer)
        { 
            this.claimRepository = claimRepository;
            this.coverRepository = coverRepository;
            this.auditer = auditer;
        }


        /// <summary>
        /// Creaates new claim.
        /// </summary>
        /// <param name="claim">The input claim.</param>
        /// <returns>The created claim.</returns>
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

        /// <summary>
        /// Deletes the claim by id.
        /// </summary>
        /// <param name="claimId">The claim id.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Get all claims.
        /// </summary>
        /// <returns>List of claims.</returns>
        public async Task<IEnumerable<Claim>> GetAllAsync()
        {
            return await this.claimRepository.GetClaimsAsync();
        }

        /// <summary>
        /// Gets claim by id.
        /// </summary>
        /// <param name="claimId">The claim id.</param>
        /// <returns>The claim.</returns>
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
