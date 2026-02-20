namespace Claims.Interfaces
{
    using Claims.Domain.Models;

    /// <summary>
    /// Provides data access operations for covers.
    /// </summary>
    public interface ICoverRepository
    {
        /// <summary>
        /// Retrieves all covers.
        /// </summary>
        Task<IEnumerable<Cover>> GetCoversAsync();

        /// <summary>
        /// Retrieves a cover by its identifier.
        /// </summary>
        /// <param name="coverId">The cover identifier.</param>
        Task<Cover?> GetCoverByIdAsync(string coverId);

        /// <summary>
        /// Adds a new cover.
        /// </summary>
        /// <param name="cover">The cover to add.</param>
        Task AddCoverAsync(Cover cover);

        /// <summary>
        /// Deletes an existing cover.
        /// </summary>
        /// <param name="cover">The cover to delete.</param>
        Task DeleteCoverAsync(Cover cover);
    }
}
