namespace Claims.Interfaces
{
    using Claims.Domain.Models;

    /// <summary>
    /// Interface for Claim repository, providing methods to manage Claim entities in the data store.
    /// </summary>
    public interface IClaimRepository
    {
        /// <summary>
        /// Gets claims asynchronously from the data store.
        /// </summary>
        /// <returns>List of claims.</returns>
        Task<IEnumerable<Claim>> GetClaimsAsync();

        /// <summary>
        /// Gets thw claim by id.
        /// </summary>
        /// <param name="claimId">The claim id.</param>
        /// <returns>The claim.</returns>
        Task<Claim?> GetClaimByIdAsync(string claimId);

        /// <summary>
        /// Adds a new claim to the data base asynchronously.
        /// </summary>
        /// <param name="claim">The claim.</param>
        /// <returns></returns>
        Task AddClaimAsync(Claim claim);

        /// <summary>
        /// Deletes the claim by id.
        /// </summary>
        /// <param name="claim">The claim.</param>
        /// <returns></returns>
        Task DeleteAsync(Claim claim);
    }
}
