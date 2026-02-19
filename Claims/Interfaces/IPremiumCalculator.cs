namespace Claims.Interfaces
{
    using Claims.Domain.Enums;

    public interface IPremiumCalculator
    {
        decimal Compute(DateTime startDate, DateTime endDate, CoverType coverType);
    }
}
