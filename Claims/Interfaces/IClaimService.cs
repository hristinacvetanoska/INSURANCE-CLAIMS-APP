namespace Claims.Interfaces
{
    using Claims.Domain.Models;

    /// <summary>
    /// Provides operations for managing claims.
    /// </summary>
    public interface IClaimService
    {
        /// <summary>
        /// Get all claims.
        /// </summary>
        /// <returns>List of claims.</returns>
        Task<IEnumerable<Claim>> GetAllAsync();

        /// <summary>
        /// Gets claim by id.
        /// </summary>
        /// <param name="claimId">The claim id.</param>
        /// <returns>The claim.</returns>
        Task<Claim> GetByIdAsync(string claimId);

        /// <summary>
        /// Creaates new claim.
        /// </summary>
        /// <param name="claim">The input claim.</param>
        /// <returns>The created claim.</returns>
        Task<Claim> CreateAsync(Claim claim);

        /// <summary>
        /// Deletes the claim by id.
        /// </summary>
        /// <param name="claimId">The claim id.</param>
        /// <returns></returns>
        Task DeleteAsync(string claimId);
    }
}
