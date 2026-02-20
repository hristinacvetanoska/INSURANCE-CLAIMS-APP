namespace Claims.Tests
{
    using Claims.Domain.Enums;
    using Claims.Domain.Models;
    using Claims.Exceptions;
    using Claims.Interfaces;
    using Claims.Services;
    using Moq;
    using System.ComponentModel.DataAnnotations;
    using Xunit;

    public class CoverServiceTests
    {
        private readonly Mock<IPremiumCalculator> mockPremiumCalculator;
        private readonly Mock<ICoverRepository> mockCoverRepository;
        private readonly Mock<IAuditer> mockAuditer;
        private readonly CoverService coverService;

        public CoverServiceTests()
        {
            mockPremiumCalculator = new Mock<IPremiumCalculator>();
            mockCoverRepository = new Mock<ICoverRepository>();
            mockAuditer = new Mock<IAuditer>();
            coverService = new CoverService(mockPremiumCalculator.Object, mockCoverRepository.Object, mockAuditer.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsCovers()
        {
            // Arrange
            var covers = new List<Cover> { new Cover { Id = "1", Type = CoverType.Yacht }, new Cover { Id = "2" , Type = CoverType.PassengerShip }, new Cover { Id = "3", Type = CoverType.Tanker } };
            this.mockCoverRepository.Setup(r => r.GetCoversAsync()).ReturnsAsync(covers);

            // Act
            var result = await coverService.GetAllAsync();

            // Assert
            Assert.Equal(3, ((List<Cover>)result).Count);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCover_WhenCoverExists()
        {
            // Arrange
            var cover = new Cover { Id = "1", Type = CoverType.Yacht };
            this.mockCoverRepository.Setup(r => r.GetCoverByIdAsync("1")).ReturnsAsync(cover);

            // Act
            var result = await coverService.GetByIdAsync("1");

            // Assert
            Assert.Equal("1", result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsNotFound_WhenCoverDoesNotExist()
        {
            // Arrange
            mockCoverRepository.Setup(r => r.GetCoverByIdAsync("1")).ReturnsAsync((Cover)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => coverService.GetByIdAsync("1"));
        }

        [Fact]
        public async Task CreateAsync_CreatesCover_WhenValid()
        {
            // Arrange
            var cover = new Cover
            {
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(30),
                Type = CoverType.Yacht
            };
            this.mockPremiumCalculator.Setup(p => p.Compute(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CoverType>())).Returns(1000m);

            // Act
            var result = await coverService.CreateAsync(cover);

            // Assert
            Assert.NotNull(result.Id);
            Assert.Equal(1000m, result.Premium);
            this.mockCoverRepository.Verify(r => r.AddCoverAsync(result), Times.Once);
            this.mockAuditer.Verify(a => a.AuditCoverAsync(result.Id, "POST"), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ThrowsValidationExcepton_WhenStartDateInPast()
        {
            // Arrange
            var cover = new Cover
            {
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(30),
                Type = CoverType.Yacht
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => coverService.CreateAsync(cover));
        }

        [Fact]
        public async Task CreateAsync_ThrowsValidationException_WhenInsurancePeriodExceedsOneYear()
        {
            // Arrange
            var cover = new Cover
            {
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(366),
                Type = CoverType.Yacht
            };

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() => coverService.CreateAsync(cover));
        }

        [Fact]
        public async Task DeleteAsync_DeletesCover_WhenExists()
        {
            // Arrange
            var cover = new Cover { Id = "1" };
            mockCoverRepository.Setup(r => r.GetCoverByIdAsync("1")).ReturnsAsync(cover);

            // Act
            await coverService.DeleteAsync("1");

            // Assert
            mockCoverRepository.Verify(r => r.DeleteCoverAsync(cover), Times.Once);
            mockAuditer.Verify(a => a.AuditCoverAsync("1", "DELETE"), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsNotFound_WhenCoverDoesNotExist()
        {
            // Arrange
            mockCoverRepository.Setup(r => r.GetCoverByIdAsync("1")).ReturnsAsync((Cover)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => coverService.DeleteAsync("1"));
        }

        [Fact]
        public void ComputePremium_CallsPremiumCalculator()
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var endDate = DateTime.UtcNow.AddDays(10);
            var coverType = CoverType.Yacht;
            mockPremiumCalculator.Setup(p => p.Compute(startDate, endDate, coverType)).Returns(500m);

            // Act
            var result = coverService.ComputePremium(startDate, endDate, coverType);

            // Assert
            Assert.Equal(500m, result);
            mockPremiumCalculator.Verify(p => p.Compute(startDate, endDate, coverType), Times.Once);
        }
    }
}
