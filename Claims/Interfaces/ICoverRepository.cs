namespace Claims.Interfaces
{
    using Claims.Domain.Models;

    public interface ICoverRepository
    {
        Task<IEnumerable<Cover>> GetCoversAsync();
        Task<Cover> GetCoverByIdAsync(string coverId);
        Task AddCoverAsync(Cover cover);
        Task DeleteCoverAsync(Cover cover);
    }
}
