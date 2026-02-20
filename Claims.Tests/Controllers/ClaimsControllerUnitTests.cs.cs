namespace Claims.Tests.Controllers
{
    using Claims.Controllers;
    using Claims.Domain.Models;
    using Claims.Exceptions;
    using Claims.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System.ComponentModel.DataAnnotations;
    using Xunit;

    public class ClaimsControllerUnitTests
    {
        private ClaimsController CreateController(Mock<IClaimService> mockService)
        {
            var loggerMock = new Mock<ILogger<ClaimsController>>();
            return new ClaimsController(mockService.Object, loggerMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkWithClaims()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim { Id = "1", CoverId = "c1" },
                new Claim { Id = "2", CoverId = "c2" }
            };

            var mockService = new Mock<IClaimService>();
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(claims);

            var controller = CreateController(mockService);

            // Act
            var result = await controller.GetAllAsync();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsAssignableFrom<IEnumerable<Claim>>(ok.Value);
            Assert.Equal(2, new List<Claim>(returned).Count);
        }

        [Fact]
        public async Task GetByIdAsync_WhenFound_ReturnsOkWithClaim()
        {
            // Arrange
            var expected = new Claim { Id = "1", CoverId = "c1" };
            var mockService = new Mock<IClaimService>();
            mockService.Setup(s => s.GetByIdAsync("1")).ReturnsAsync(expected);

            var controller = CreateController(mockService);

            // Act
            var result = await controller.GetByIdAsync("1");

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsType<Claim>(ok.Value);
            Assert.Equal(expected.Id, returned.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            var mockService = new Mock<IClaimService>();
            mockService.Setup(s => s.GetByIdAsync("missing")).ThrowsAsync(new NotFoundException("not found"));

            var controller = CreateController(mockService);

            // Act
            var result = await controller.GetByIdAsync("missing");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateAsync_OnSuccess_ReturnsCreated()
        {
            // Arrange
            var input = new Claim { CoverId = "c1" };
            var created = new Claim { Id = "1", CoverId = "c1" };

            var mockService = new Mock<IClaimService>();
            mockService.Setup(s => s.CreateAsync(input)).ReturnsAsync(created);

            var controller = CreateController(mockService);

            // Act
            var result = await controller.CreateAsync(input);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            var returned = Assert.IsType<Claim>(createdResult.Value);
            Assert.Equal(created.Id, returned.Id);
        }

        [Fact]
        public async Task CreateAsync_OnValidationException_ReturnsBadRequest()
        {
            // Arrange
            var input = new Claim { CoverId = "" }; // invalid for example
            var mockService = new Mock<IClaimService>();
            mockService.Setup(s => s.CreateAsync(input)).ThrowsAsync(new ValidationException("Invalid"));

            var controller = CreateController(mockService);

            // Act
            var result = await controller.CreateAsync(input);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid", bad.Value);
        }

        [Fact]
        public async Task DeleteAsync_OnSuccess_ReturnsNoContent()
        {
            // Arrange
            var mockService = new Mock<IClaimService>();
            mockService.Setup(s => s.DeleteAsync("1")).Returns(Task.CompletedTask);

            var controller = CreateController(mockService);

            // Act
            var result = await controller.DeleteAsync("1");

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAsync_WhenNotFound_ReturnsNotFound()
        {
            // Arrange
            var mockService = new Mock<IClaimService>();
            mockService.Setup(s => s.DeleteAsync("missing")).ThrowsAsync(new NotFoundException("not found"));

            var controller = CreateController(mockService);

            // Act
            var result = await controller.DeleteAsync("missing");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}