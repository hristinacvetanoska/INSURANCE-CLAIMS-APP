namespace Claims.Tests.ServicesTests
{
    using Claims.Domain.Enums;
    using Claims.Services;
    using Xunit;

    public class PremiumCalculatorTests
    {
        private readonly PremiumCalculator calculator;

        public PremiumCalculatorTests()
        {
            calculator = new PremiumCalculator();
        }

        [Fact]
        public void Compute_ReturnsPremium_ForYacht_30Days()
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(30);
            var coverType = CoverType.Yacht;

            // Act
            var premium = calculator.Compute(startDate, endDate, coverType);

            // Assert
            var expected = 30 * 1250m * 1.1m;
            Assert.Equal(expected, premium);
        }

        [Fact]
        public void Compute_ReturnsPremium_ForBulkCarrier_181Days()
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(181);
            var coverType = CoverType.BulkCarrier;

            // Act
            var premium = calculator.Compute(startDate, endDate, coverType);

            // Assert
            var premiumPerDay = 1250m * 1.3m; 
            var first30Days = 30 * premiumPerDay;          
            var next150Days = 150 * premiumPerDay * 0.98m; 
            var lastDay = 1 * premiumPerDay * 0.98m * 0.99m; 
            var expected = first30Days + next150Days + lastDay;

            Assert.Equal(expected, premium);
        }

        [Fact]
        public void Compute_ReturnsPremium_Yacht_180Days()
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var endDate = startDate.AddDays(180);
            var coverType = CoverType.Yacht;

            // Act
            var premium = calculator.Compute(startDate, endDate, coverType);

            // Assert
            var premiumPerDay = 1250m * 1.1m;

            var first30Days = 30 * premiumPerDay;
            var next150Days = 150 * premiumPerDay * 0.95m;

            var expected = first30Days + next150Days;

            Assert.Equal(expected, premium);
        }
    }
}
