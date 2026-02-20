namespace Claims.Services
{
    using Claims.Domain.Models;
    using Claims.DTOs;
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
        /// Creates new claim.
        /// </summary>
        /// <param name="claim">The input claim.</param>
        /// <returns>The created claim.</returns>
        public async Task<ClaimDto> CreateAsync(ClaimDto claimDTO)
        {
            var cover = await this.coverRepository.GetCoverByIdAsync(claimDTO.CoverId);

            if(cover == null)
            {
                throw new NotFoundException($"Cover with id={claimDTO.CoverId} not found.");
            }

            if(claimDTO.Created.Date < cover.StartDate.Date || claimDTO.Created.Date > cover.EndDate.Date)
            {
                throw new ValidationException("Claim date must be within the cover period.");
            }

            var claim = new Claim
            {
                Id = Guid.NewGuid().ToString(),
                CoverId = claimDTO.CoverId,
                Created = claimDTO.Created,
                Name = claimDTO.Name,
                Type = claimDTO.Type,
                DamageCost = claimDTO.DamageCost
            };
            claimDTO.Id = claim.Id;

            await this.claimRepository.AddClaimAsync(claim);
            await this.auditer.AuditClaimAsync(claim.Id, "POST");

            return claimDTO;
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
        public async Task<IEnumerable<ClaimDto>> GetAllAsync()
        {
            var claims = await this.claimRepository.GetClaimsAsync();
            var claimsDTOs = new List<ClaimDto>();
            foreach (var claim in claims)
            {
               claimsDTOs.Add(new ClaimDto
                {
                    Id = claim.Id,
                    CoverId = claim.CoverId,
                    Created = claim.Created,
                    Name = claim.Name,
                    Type = claim.Type,
                    DamageCost = claim.DamageCost
                });
            }
            return claimsDTOs;
        }

        /// <summary>
        /// Gets claim by id.
        /// </summary>
        /// <param name="claimId">The claim id.</param>
        /// <returns>The claim.</returns>
        public async Task<ClaimDto> GetByIdAsync(string claimId)
        {
            var claim =  await this.claimRepository.GetClaimByIdAsync(claimId);

            if (claim == null)
            {
                throw new NotFoundException($"Claim with id={claimId} not found.");
            }

            return new ClaimDto
            {
                Id = claim.Id,
                CoverId = claim.CoverId,
                Created = claim.Created,
                Name = claim.Name,
                Type = claim.Type,
                DamageCost = claim.DamageCost
            };
        }
    }
}
