namespace Claims.Services
{
    using Claims.Domain.Enums;
    using Claims.Extensions;
    using Claims.Interfaces;
    using System;

    public class PremiumCalculator : IPremiumCalculator
    {
        public decimal Compute(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            var multiplier = coverType.GetMultiplier();
            var premiumPerDay = 1250m * multiplier;
            var insuranceLength = (endDate - startDate).Days;
            var totalPremium = 0m;

            for (var i = 0; i < insuranceLength; i++)
            {
                if (i < 30)
                {
                    totalPremium += premiumPerDay;
                }
                else if (i>=30 && i < 180)
                {
                    if(coverType == CoverType.Yacht)
                    {
                        totalPremium += premiumPerDay * 0.95m;
                    }
                    else
                    {
                        totalPremium += premiumPerDay * 0.98m;
                    }
                }
                else if (i >= 180)
                {
                    if(coverType != CoverType.Yacht)
                    {
                        totalPremium += premiumPerDay * 0.98m * 0.99m;
                    }
                    else
                    {
                        totalPremium += premiumPerDay * 0.95m * 0.97m;
                    }
                }
            }

            return totalPremium;
        }
    }
}
