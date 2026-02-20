namespace Claims.Extensions
{
    using Claims.Domain.Enums;

    public static class CoverTypeExtensions
    {
        public static decimal GetMultiplier(this CoverType coverType)
        {
            return coverType switch
            {
                CoverType.Yacht => 1.1m,
                CoverType.PassengerShip => 1.2m,
                CoverType.Tanker => 1.5m,
                _ => 1.3m
            };
        }
    }

}
