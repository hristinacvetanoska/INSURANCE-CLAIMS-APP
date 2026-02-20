namespace Claims.Interfaces
{
    using Claims.Domain.Enums;
    using Claims.Domain.Models;
    using Claims.DTOs;

    /// <summary>
    /// Defines operations for managing covers.
    /// </summary>
    public interface ICoverService
    {
        /// <summary>
        /// Retrieves all covers.
        /// </summary>
        Task<IEnumerable<CoverDto>> GetAllAsync();

        /// <summary>
        /// Retrieves a cover by its identifier.
        /// </summary>
        /// <param name="coverId">The cover id.</param>
        /// <returns></returns>
        Task<CoverDto> GetByIdAsync(string coverId);

        /// <summary>
        /// Creates coves.
        /// </summary>
        /// <param name="cover">The cover.</param>
        /// <returns>The newly created cover.</returns>
        Task<CoverDto> CreateAsync(CoverDto cover);

        /// <summary>
        /// Deletes the cover by id.
        /// </summary>
        /// <param name="coverId">The cover id.</param>
        /// <returns></returns>
        Task DeleteAsync(string coverId);

        /// <summary>
        /// Computes the premium based on period and cover type.
        /// </summary>
        /// <param name="startDate">Cover start date.</param>
        /// <param name="endDate">Cover end date.</param>
        /// <param name="coverType">Type of cover.</param>
        /// <returns>The calculated premium amount.</returns>
        decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType);
    }
}
