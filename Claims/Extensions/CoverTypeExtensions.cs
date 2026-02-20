namespace Claims.Extensions
{
    using Claims.Domain.Enums;

    /// <summary>
    /// Provides extension methods for <see cref="CoverType"/>.
    /// </summary>
    public static class CoverTypeExtensions
    {
        /// <summary>
        /// Returns the premium multiplier associated with a cover type.
        /// </summary>
        /// <param name="coverType">The cover type.</param>
        /// <returns>The multiplier used for premium calculation.</returns>
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
