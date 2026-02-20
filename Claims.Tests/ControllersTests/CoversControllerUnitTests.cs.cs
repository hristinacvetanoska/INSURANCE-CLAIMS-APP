namespace Claims.Tests.Controllers
{
    using Claims.Controllers;
    using Claims.Domain.Enums;
    using Claims.Domain.Models;
    using Claims.Exceptions;
    using Claims.Interfaces;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Threading.Tasks;
    using Xunit;

    public class CoversControllerUnitTests
    {
        private CoversController CreateController(Mock<ICoverService> mockService)
        {
            var loggerMock = new Mock<ILogger<CoversController>>();
            return new CoversController(mockService.Object, loggerMock.Object);
        }

        [Fact]
        public void ComputePremium_ReturnsOk_WhenValid()
        {
            // Arrange
            var start = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc);
            var end = start.AddDays(10);
            var coverType = CoverType.Yacht;
            var expected = 123.45m;

            var mockService = new Mock<ICoverService>();
            mockService.Setup(s => s.ComputePremium(start, end, coverType)).Returns(expected);

            var controller = CreateController(mockService);

            // Act
            var result = controller.ComputePremium(start, end, coverType);

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(expected, Assert.IsType<decimal>(ok.Value));
        }

        [Fact]
        public void ComputePremium_ReturnsBadRequest_OnArgumentException()
        {
            // Arrange
            var start = DateTime.UtcNow;
            var end = start.AddDays(-2);
            var coverType = CoverType.Yacht;

            var mockService = new Mock<ICoverService>();
            mockService.Setup(s => s.ComputePremium(start, end, coverType)).Throws(new ValidationException("Invalid period"));

            var controller = CreateController(mockService);

            // Act
            var result = controller.ComputePremium(start, end, coverType);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal("Invalid period", bad.Value);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsOkWithCovers()
        {
            // Arrange
            var covers = new List<Cover>
            {
                new Cover { Id = "c1", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(10), Type = CoverType.Yacht },
                new Cover { Id = "c2", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(5), Type = CoverType.Tanker }
            };

            var mockService = new Mock<ICoverService>();
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(covers);

            var controller = CreateController(mockService);

            // Act
            var result = await controller.GetAllAsync();

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var returned = Assert.IsAssignableFrom<IEnumerable<Cover>>(ok.Value);
            Assert.Equal(2, new List<Cover>(returned).Count);
        }

        [Fact]
        public async Task GetById_ReturnsCover_WhenFound()
        {
            // Arrange
            var expected = new Cover { Id = "c1", StartDate = DateTime.UtcNow, EndDate = DateTime.UtcNow.AddDays(30), Type = CoverType.PassengerShip };
            var mockService = new Mock<ICoverService>();
            mockService.Setup(s => s.GetByIdAsync("c1")).ReturnsAsync(expected);

            var controller = CreateController(mockService);

            // Act
            var result = await controller.GetByIdAsync("c1");

            // Assert
            var ok = Assert.IsType<OkObjectResult>(result.Result);
            var cover = Assert.IsType<Cover>(ok.Value);
            Assert.Equal("c1", cover.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            var mockService = new Mock<ICoverService>();
            mockService.Setup(s => s.GetByIdAsync("missing")).ThrowsAsync(new NotFoundException("not found"));

            var controller = CreateController(mockService);

            // Act
            var result = await controller.GetByIdAsync("missing");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async Task CreateAsync_ReturnsCreated_WhenValid()
        {
            // Arrange
            var input = new Cover
            {
                StartDate = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc),
                EndDate = new DateTime(2025, 01, 31, 0, 0, 0, DateTimeKind.Utc),
                Type = CoverType.Yacht
            };

            var created = new Cover
            {
                Id = "newId",
                StartDate = input.StartDate,
                EndDate = input.EndDate,
                Type = input.Type
            };

            var mockService = new Mock<ICoverService>();
            mockService.Setup(s => s.CreateAsync(It.IsAny<Cover>())).ReturnsAsync(created);

            var controller = CreateController(mockService);

            // Act
            var result = await controller.CreateAsync(input);

            // Assert
            var createdResult = Assert.IsType<CreatedResult>(result);
            var returned = Assert.IsType<Cover>(createdResult.Value);
            Assert.Equal(created.Id, returned.Id);

            mockService.Verify(s => s.CreateAsync(It.Is<Cover>(c => c.Type == input.Type && c.StartDate == input.StartDate)), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ReturnsBadRequest_OnValidationException()
        {
            // Arrange
            var input = new Cover
            {
                StartDate = DateTime.UtcNow.AddDays(-10),
                EndDate = DateTime.UtcNow.AddDays(5),
                Type = CoverType.Yacht
            };

            var mockService = new Mock<ICoverService>();
            mockService.Setup(s => s.CreateAsync(It.IsAny<Cover>())).ThrowsAsync(new ValidationException("Invalid"));

            var controller = CreateController(mockService);

            // Act
            var result = await controller.CreateAsync(input);

            // Assert
            var bad = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid", bad.Value);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNoContent_WhenSuccess()
        {
            // Arrange
            var mockService = new Mock<ICoverService>();
            mockService.Setup(s => s.DeleteAsync("c1")).Returns(Task.CompletedTask);

            var controller = CreateController(mockService);

            // Act
            var result = await controller.DeleteAsync("c1");

            // Assert
            Assert.IsType<NoContentResult>(result);
            mockService.Verify(s => s.DeleteAsync("c1"), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            var mockService = new Mock<ICoverService>();
            mockService.Setup(s => s.DeleteAsync("missing")).ThrowsAsync(new NotFoundException("not found"));

            var controller = CreateController(mockService);

            // Act
            var result = await controller.DeleteAsync("missing");

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}