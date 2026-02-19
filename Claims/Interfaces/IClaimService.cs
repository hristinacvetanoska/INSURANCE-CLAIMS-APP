namespace Claims.Interfaces
{
    using Claims.Domain.Models;

    public interface IClaimService
    {
        Task<IEnumerable<Claim>> GetAllAsync();
        Task<Claim> GetByIdAsync(string claimId);
        Task<Claim> CreateAsync(Claim claim);
        Task DeleteAsync(string claimId);
    }
}
