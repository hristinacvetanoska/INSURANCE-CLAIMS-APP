namespace Claims.Services
{
    using Claims.Domain.Enums;
    using Claims.Interfaces;
    using System;

    public class PremiumCalculator : IPremiumCalculator
    {
        public decimal Compute(DateTime startDate, DateTime endDate, CoverType coverType)
        {
            var multiplier = 1.3m;
            if (coverType == CoverType.Yacht)
            {
                multiplier = 1.1m;
            }

            if (coverType == CoverType.PassengerShip)
            {
                multiplier = 1.2m;
            }

            if (coverType == CoverType.Tanker)
            {
                multiplier = 1.5m;
            }

            var premiumPerDay = 1250 * multiplier;
            var insuranceLength = (endDate - startDate).TotalDays;
            var totalPremium = 0m;

            for (var i = 0; i < insuranceLength; i++)
            {
                if (i < 30)
                {
                    totalPremium += premiumPerDay;
                }
                if (i < 180)
                {
                    if(coverType == CoverType.Yacht)
                    {
                        totalPremium += premiumPerDay - premiumPerDay * 0.05m;
                    }
                    else
                    {
                        totalPremium += premiumPerDay - premiumPerDay * 0.02m;
                    }
                }
                if (i < 365)
                {
                    if(coverType != CoverType.Yacht)
                    {
                        totalPremium += premiumPerDay - premiumPerDay * 0.03m;
                    }
                    else
                    {
                        totalPremium += premiumPerDay - premiumPerDay * 0.08m;
                    }
                }
            }

            return totalPremium;
        }
    }
}
