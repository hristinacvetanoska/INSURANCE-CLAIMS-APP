namespace Claims.Tests.Controllers
{
    using Claims.Domain.Models;
    using Claims.DTOs;
    using Claims.Exceptions;
    using Claims.Interfaces;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http.Json;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using Xunit;

    public class ClaimsControllerIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;
        private static readonly JsonSerializerOptions _jsonOptions;

        static ClaimsControllerIntegrationTests()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            _jsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public ClaimsControllerIntegrationTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private HttpClient GetClientWithMocks(Mock<IClaimService> mockClaimService)
        {
            return _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptors = services.Where(d => d.ServiceType == typeof(IClaimService)).ToList();
                    foreach (var d in descriptors)
                        services.Remove(d);

                    services.AddScoped<IClaimService>(_ => mockClaimService.Object);
                });
            }).CreateClient(new WebApplicationFactoryClientOptions
            {
                BaseAddress = new Uri("https://localhost")
            });
        }

        [Fact]
        public async Task GetAll_ReturnsClaims()
        {
            // Arrange
            var mockService = new Mock<IClaimService>();
            mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(new List<ClaimDto>
            {
                new ClaimDto { Id = "1", CoverId = "c1", Type = Domain.Enums.ClaimType.Collision },
                new ClaimDto { Id = "2", CoverId = "c2", Type = Domain.Enums.ClaimType.Fire }
            });

            var client = GetClientWithMocks(mockService);

            // Act
            var response = await client.GetAsync("/Claims");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var claims = await response.Content.ReadFromJsonAsync<List<Claim>>(_jsonOptions);
            Assert.NotNull(claims);
            Assert.Equal(2, claims.Count);
        }

        [Fact]
        public async Task GetById_ReturnsClaim_WhenFound()
        {
            // Arrange
            var expected = new ClaimDto { Id = "1", CoverId = "c1", Type = Domain.Enums.ClaimType.Collision };
            var mockService = new Mock<IClaimService>();
            mockService.Setup(s => s.GetByIdAsync("1")).ReturnsAsync(expected);

            var client = GetClientWithMocks(mockService);

            // Act
            var response = await client.GetAsync("/Claims/1");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var claim = await response.Content.ReadFromJsonAsync<Claim>(_jsonOptions);
            Assert.NotNull(claim);
            Assert.Equal("1", claim.Id);
        }

        [Fact]
        public async Task GetById_ReturnsNotFound_WhenMissing()
        {
            // Arrange
            var mockService = new Mock<IClaimService>();
            mockService.Setup(s => s.GetByIdAsync("missing")).ThrowsAsync(new NotFoundException("not found"));

            var client = GetClientWithMocks(mockService);

            // Act
            var response = await client.GetAsync("/Claims/missing");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Delete_ReturnsNoContent()
        {
            // Arrange
            var mockService = new Mock<IClaimService>();
            mockService.Setup(s => s.DeleteAsync("1")).Returns(Task.CompletedTask);

            var client = GetClientWithMocks(mockService);

            // Act
            var response = await client.DeleteAsync("/Claims/1");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }
}