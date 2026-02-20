namespace Claims.Services
{
    using Claims.Domain.Enums;
    using Claims.Domain.Models;
    using Claims.Exceptions;
    using Claims.Interfaces;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;

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
        public Task<IEnumerable<Cover>> GetAllAsync()
        {
           return this.coverRepository.GetCoversAsync();
        }

        public async Task<Cover> GetByIdAsync(string coverId)
        {
            var cover = await this.coverRepository.GetCoverByIdAsync(coverId);

            if(cover == null)
            {
                throw new NotFoundException($"Cover with id={coverId} not found.");
            }

            return cover;
        }
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
