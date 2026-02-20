namespace Claims.Tests.ServicesTests
{
    using Claims.Domain.Models;
    using Claims.Exceptions;
    using Claims.Interfaces;
    using Claims.Services;
    using Moq;
    using System.ComponentModel.DataAnnotations;
    using Xunit;

    public class ClaimServiceTests
    {
        private readonly Mock<IClaimRepository> mockClaimRepository;
        private readonly Mock<ICoverRepository> mockCoverRepository;
        private readonly Mock<IAuditer> mockAuditer;
        private readonly ClaimService claimService;

        public ClaimServiceTests()
        {
            this.mockClaimRepository = new Mock<IClaimRepository>();
            this.mockCoverRepository = new Mock<ICoverRepository>();
            this.mockAuditer = new Mock<IAuditer>();

            claimService = new ClaimService(
                this.mockClaimRepository.Object,
                this.mockCoverRepository.Object,
                this.mockAuditer.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsClaim()
        {
            // Arrange
            var claim = new Claim { Id = "1" };

            this.mockClaimRepository
                .Setup(r => r.GetClaimByIdAsync("1"))
                .ReturnsAsync(claim);

            // Act
            var result = await claimService.GetByIdAsync("1");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("1", result.Id);
        }

        [Fact]
        public async Task GetByIdAsync_ThrowsNotFound_WhenClaimDoesNotExist()
        {
            // Arrange
            this.mockClaimRepository
                .Setup(r => r.GetClaimByIdAsync("1"))
                .ReturnsAsync((Claim)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                claimService.GetByIdAsync("1"));
        }

        [Fact]
        public async Task DeleteAsync_SuccessfullyDeletesClaim()
        {
            // Arrange
            var claim = new Claim { Id = "1" };

            this.mockClaimRepository
                .Setup(r => r.GetClaimByIdAsync("1"))
                .ReturnsAsync(claim);

            // Act
            await claimService.DeleteAsync("1");

            // Assert
            this.mockClaimRepository.Verify(r => r.DeleteAsync(claim), Times.Once);
            this.mockAuditer.Verify(a => a.AuditClaimAsync("1", "DELETE"), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsNotFound_WhenClaimDoesNotExist()
        {
            // Arrange
            this.mockClaimRepository
                .Setup(r => r.GetClaimByIdAsync("1"))
                .ReturnsAsync((Claim)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                claimService.DeleteAsync("1"));
        }

        [Fact]
        public async Task CreateAsync_SuccessfullyCreatesClaim_WhenValid()
        {
            // Arrange
            var cover = new Cover
            {
                Id = "cover1",
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(10)
            };

            var claim = new Claim
            {
                CoverId = "cover1",
                Created = DateTime.UtcNow
            };

            this.mockCoverRepository
                .Setup(r => r.GetCoverByIdAsync("cover1"))
                .ReturnsAsync(cover);

            // Act
            var result = await claimService.CreateAsync(claim);

            // Assert.
            Assert.NotNull(result.Id);
            this.mockClaimRepository.Verify(r => r.AddClaimAsync(claim), Times.Once);
            this.mockAuditer.Verify(a => a.AuditClaimAsync(result.Id, "POST"), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ThrowsNotFound_WhenCoverDoesNotExist()
        {
            // Assert
            var claim = new Claim
            {
                CoverId = "invalid",
                Created = DateTime.UtcNow
            };

            this.mockCoverRepository
                .Setup(r => r.GetCoverByIdAsync("invalid"))
                .ReturnsAsync((Cover)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() =>
                claimService.CreateAsync(claim));
        }


        [Fact]
        public async Task CreateAsync_ThrowsValidationException_WhenDateOutsideCoverPeriod()
        {
            // Assert
            var cover = new Cover
            {
                Id = "cover1",
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(10)
            };

            var claim = new Claim
            {
                CoverId = "cover1",
                Created = DateTime.UtcNow
            };

            this.mockCoverRepository
                .Setup(r => r.GetCoverByIdAsync("cover1"))
                .ReturnsAsync(cover);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                claimService.CreateAsync(claim));
        }

        [Fact]
        public async Task CreateAsync_ThrowsValidationException_WhenDamageCostTooHigh()
        {
            // Arrange
            var cover = new Cover
            {
                Id = "cover1",
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(10)
            };

            var claim = new Claim
            {
                CoverId = "cover1",
                Created = DateTime.UtcNow,
                DamageCost = 200000
            };

            this.mockCoverRepository
                .Setup(r => r.GetCoverByIdAsync("cover1"))
                .ReturnsAsync(cover);

            // Act & Assert
            await Assert.ThrowsAsync<ValidationException>(() =>
                claimService.CreateAsync(claim));
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllClaims()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim { Id = "1" },
                new Claim { Id = "2" }
            };

            this.mockClaimRepository
                .Setup(r => r.GetClaimsAsync())
                .ReturnsAsync(claims);

            // Act
            var result = await claimService.GetAllAsync();

            // Assert
            Assert.Equal(2, result.Count());

        }

        [Fact]
        public async Task CreateAsync_AllowsCreatedOnCoverStartDate()
        {
            // Arrange
            var cover = new Cover
            {
                Id = "cover1",
                StartDate = DateTime.UtcNow.Date,
                EndDate = DateTime.UtcNow.AddDays(10).Date
            };

            var claim = new Claim
            {
                CoverId = "cover1",
                Created = cover.StartDate,
                DamageCost = 5000
            };

            mockCoverRepository
                .Setup(r => r.GetCoverByIdAsync("cover1"))
                .ReturnsAsync(cover);

            // Act
            var result = await claimService.CreateAsync(claim);

            // Assert
            Assert.NotNull(result.Id);
            mockClaimRepository.Verify(r => r.AddClaimAsync(claim), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_AllowsCreatedOnCoverEndDate()
        {
            // Arrange
            var cover = new Cover
            {
                Id = "cover1",
                StartDate = DateTime.UtcNow.AddDays(-10).Date,
                EndDate = DateTime.UtcNow.Date
            };

            var claim = new Claim
            {
                CoverId = "cover1",
                Created = cover.EndDate, // edge case
                DamageCost = 5000
            };

            mockCoverRepository
                .Setup(r => r.GetCoverByIdAsync("cover1"))
                .ReturnsAsync(cover);

            // Act
            var result = await claimService.CreateAsync(claim);

            // Assert
            Assert.NotNull(result.Id);
            mockClaimRepository.Verify(r => r.AddClaimAsync(claim), Times.Once);
        }
    }
}
