namespace Claims.Services
{
    using Claims.Domain.Enums;
    using Claims.Domain.Models;
    using Claims.Exceptions;
    using Claims.Interfaces;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines operations for managing covers.
    /// </summary>
    public class CoverService : ICoverService
    {
        private readonly IPremiumCalculator premiumCalculator;
        private readonly ICoverRepository coverRepository;
        private readonly IAuditer auditer;
        public CoverService(IPremiumCalculator premiumCalculator, ICoverRepository coverRepository, IAuditer auditer)
        {
            this.premiumCalculator = premiumCalculator;
            this.coverRepository = coverRepository;
            this.auditer = auditer;
        }

        /// <summary>
        /// Retrieves all covers.
        /// </summary>
        public Task<IEnumerable<Cover>> GetAllAsync()
        {
           return this.coverRepository.GetCoversAsync();
        }

        /// <summary>
        /// Retrieves a cover by its identifier.
        /// </summary>
        /// <param name="coverId">The cover id.</param>
        /// <returns></returns>
        public async Task<Cover> GetByIdAsync(string coverId)
        {
            var cover = await this.coverRepository.GetCoverByIdAsync(coverId);

            if(cover == null)
            {
                throw new NotFoundException($"Cover with id={coverId} not found.");
            }

            return cover;
        }

        /// <summary>
        /// Creates coves.
        /// </summary>
        /// <param name="cover">The cover.</param>
        /// <returns>The newly created cover.</returns>
        public async Task<Cover> CreateAsync(Cover cover)
        {
            if (cover == null)
            {
                throw new ArgumentNullException(nameof(cover));
            }

            if (cover.StartDate.Date < DateTime.UtcNow.Date)
            {
                throw new ValidationException("Cover StartDate cannot be in the past.");
            }
            if ((cover.EndDate - cover.StartDate).TotalDays > 365)
            {
                throw new ValidationException("Insurance period cannot exceed 1 year.");
            }

            if (cover.EndDate <= cover.StartDate)
            {
                throw new ValidationException("Cover EndDate must be after StartDate.");
            }

            cover.Id = Guid.NewGuid().ToString();
            cover.Premium = ComputePremium(cover.StartDate, cover.EndDate, cover.Type);

            await this.coverRepository.AddCoverAsync(cover);
            await this.auditer.AuditCoverAsync(cover.Id, "POST");

            return cover;
        }

        /// <summary>
        /// Deletes the cover by id.
        /// </summary>
        /// <param name="coverId">The cover id.</param>
        /// <returns></returns>
        public async Task DeleteAsync(string coverId)
        {
            var cover = await this.coverRepository.GetCoverByIdAsync(coverId);

            if (cover == null)
            {
                throw new NotFoundException($"Cover with id={coverId} not found.");
            }

            await this.coverRepository.DeleteCoverAsync(cover);
            await this.auditer.AuditCoverAsync(coverId, "DELETE");
        }


        /// <summary>
        /// Computes the premium based on period and cover type.
        /// </summary>
        /// <param name="startDate">Cover start date.</param>
        /// <param name="endDate">Cover end date.</param>
        /// <param name="coverType">Type of cover.</param>
        /// <returns>The calculated premium amount.</returns>
        public decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            if (endDate <= startDate)
            {
                throw new ValidationException("Cover EndDate must be after StartDate.");
            }

            return this.premiumCalculator.Compute(startDate, endDate, coverType);
        }
    }
}
