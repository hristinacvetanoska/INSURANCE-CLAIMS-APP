namespace Claims.Tests.Controllers
{
    using Claims.Domain.Enums;
    using Claims.Domain.Models;
    using Claims.Exceptions;
    using Claims.Interfaces;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Net;
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using Xunit;

    public class CoversControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private static readonly JsonSerializerOptions _jsonOptions;

        static CoversControllerIntegrationTests()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public CoversControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private HttpClient GetClientWithMocks(Mock<ICoverService> mockCoverService)
        {
            return _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptors = services.Where(d => d.ServiceType == typeof(ICoverService)).ToList();
                    foreach (var d in descriptors)
                        services.Remove(d);

                    services.AddScoped<ICoverService>(_ => mockCoverService.Object);
                });
            }).CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost")
            });
        }

        [Fact]
        public async Task GetAll_ReturnsCovers()
        {
            // Arrange
            var mockService = new Mock<ICoverService>();
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<Cover>
            {
                new Cover { Id = "c1", StartDate = DateTime.UtcNow.AddDays(-10), EndDate = DateTime.UtcNow.AddDays(10), Type = CoverType.BulkCarrier },
                new Cover { Id = "c2", StartDate = DateTime.UtcNow.AddDays(-5), EndDate = DateTime.UtcNow.AddDays(5), Type = CoverType.Tanker }
            });

            var client = GetClientWithMocks(mockService);

            // Act
            var response = await client.GetAsync("/Covers");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var covers = await response.Content.ReadFromJsonAsync<List<Cover>>(_jsonOptions);
            Assert.NotNull(covers);
            Assert.Equal(2, covers.Count);
        }

        [Fact]
        public async Task GetById_ReturnsCover_WhenFound()
        {
            // Arrange
            var expected = new Cover
            {
                Id = "c1",
                StartDate = DateTime.UtcNow.AddDays(-1),
                EndDate = DateTime.UtcNow.AddDays(30),
                Type = CoverType.PassengerShip
            };

            var mockService = new Mock<ICoverService>();
            mockService.Setup(s => s.GetByIdAsync("c1")).ReturnsAsync(expected);

            var client = GetClientWithMocks(mockService);

            // Act
            var response = await client.GetAsync("/Covers/c1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var cover = await response.Content.ReadFromJsonAsync<Cover>(_jsonOptions);
            Assert.NotNull(cover);
            Assert.Equal("c1", cover.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            var mockService = new Mock<ICoverService>();
            mockService.Setup(s => s.GetByIdAsync("missing")).ThrowsAsync(new NotFoundException("not found"));

            var client = GetClientWithMocks(mockService);

            // Act
            var response = await client.GetAsync("/Covers/missing");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task ComputePremium_ReturnsOk_WithValue()
        {
            // Arrange
            var start = DateTime.UtcNow.Date;
            var end = start.AddDays(10);
            var coverType = CoverType.BulkCarrier;
            var expectedPremium = 123.45m;

            var mockService = new Mock<ICoverService>();
            mockService.Setup(s => s.ComputePremium(start, end, coverType)).Returns(expectedPremium);

            var client = GetClientWithMocks(mockService);

            // Act
            var url = $"/Covers/compute?startDate={Uri.EscapeDataString(start.ToString("o"))}&endDate={Uri.EscapeDataString(end.ToString("o"))}&coverType={coverType}";
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var value = await response.Content.ReadFromJsonAsync<decimal>(_jsonOptions);
            Assert.Equal(expectedPremium, value);
        }

        [Fact]
        public async Task ComputePremium_ReturnsBadRequest_OnInvalidInput()
        {
            // Arrange
            var start = DateTime.UtcNow.Date;
            var end = start.AddDays(-1);
            var coverType = CoverType.BulkCarrier;

            var mockService = new Mock<ICoverService>();
            mockService.Setup(s => s.ComputePremium(start, end, coverType)).Throws(new ValidationException("Invalid period"));

            var client = GetClientWithMocks(mockService);

            var url = $"/Covers/compute?startDate={Uri.EscapeDataString(start.ToString("o"))}&endDate={Uri.EscapeDataString(end.ToString("o"))}&coverType={coverType}";
            var response = await client.GetAsync(url);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            // Arrange
            var mockService = new Mock<ICoverService>();
            mockService.Setup(s => s.DeleteAsync("c1")).Returns(Task.CompletedTask);

            var client = GetClientWithMocks(mockService);

            // Act
            var response = await client.DeleteAsync("/Covers/c1");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}