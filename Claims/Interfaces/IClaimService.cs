namespace Claims.Interfaces
{
    using Claims.Domain.Models;
    using Claims.DTOs;

    /// <summary>
    /// Provides operations for managing claims.
    /// </summary>
    public interface IClaimService
    {
        /// <summary>
        /// Get all claims.
        /// </summary>
        /// <returns>List of claims.</returns>
        Task<IEnumerable<ClaimDto>> GetAllAsync();

        /// <summary>
        /// Gets claim by id.
        /// </summary>
        /// <param name="claimId">The claim id.</param>
        /// <returns>The claim.</returns>
        Task<ClaimDto> GetByIdAsync(string claimId);

        /// <summary>
        /// Creates new claim.
        /// </summary>
        /// <param name="claim">The input claim.</param>
        /// <returns>The created claim.</returns>
        Task<ClaimDto> CreateAsync(ClaimDto claim);

        /// <summary>
        /// Deletes the claim by id.
        /// </summary>
        /// <param name="claimId">The claim id.</param>
        /// <returns></returns>
        Task DeleteAsync(string claimId);
    }
}
