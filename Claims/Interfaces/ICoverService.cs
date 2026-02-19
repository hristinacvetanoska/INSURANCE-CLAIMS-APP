namespace Claims.Interfaces
{
    using Claims.Domain.Enums;
    using Claims.Domain.Models;

    public interface ICoverService
    {
        Task<IEnumerable<Cover>> GetAllAsync();
        Task<Cover> GetByIdAsync(string coverId);
        Task<Cover> CreateAsync(Cover cover);
        Task DeleteAsync(string coverId);
        decimal ComputePremium(DateTime startDate, DateTime endDate, CoverType coverType);
    }
}
