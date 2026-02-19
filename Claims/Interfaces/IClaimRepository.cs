namespace Claims.Interfaces
{
    using Claims.Domain.Models;

    public interface IClaimRepository
    {
        Task<IEnumerable<Claim>> GetClaimsAsync();
        Task<Claim> GetClaimByIdAsync(string claimId);
        Task AddClaimAsync(Claim claim);
        Task DeleteAsync(Claim claim);
    }
}
