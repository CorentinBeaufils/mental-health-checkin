using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using MentalHealthAPI.Data;
using MentalHealthAPI.DTOs;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace MentalHealthAPI.Tests.Controllers
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _databaseName = "TestDb_" + Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove the real database context registration
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (dbContextDescriptor != null)
                    services.Remove(dbContextDescriptor);

                var contextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ApplicationDbContext));
                if (contextDescriptor != null)
                    services.Remove(contextDescriptor);

                // Also remove any database provider registrations
                var sqliteDescriptor = services.Where(d => d.ServiceType.FullName != null && 
                    (d.ServiceType.FullName.Contains("Sqlite") || d.ServiceType.FullName.Contains("EntityFramework")))
                    .ToList();
                
                foreach (var descriptor in sqliteDescriptor)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database for testing with shared database name
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseName);
                });
            });

            builder.UseEnvironment("Testing");
        }
    }

    public class CheckInsControllerTests : IClassFixture<TestWebApplicationFactory>
    {
        private readonly TestWebApplicationFactory _factory;
        private readonly HttpClient _client;

        public CheckInsControllerTests(TestWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task CreateCheckIn_ValidData_ReturnsCreated()
        {
            // Arrange
            var createDto = new CreateCheckInDto
            {
                UserId = "testuser",
                Mood = 4,
                Notes = "Feeling good today"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/checkins", createDto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var checkIn = JsonSerializer.Deserialize<CheckInDto>(content, options);
            
            Assert.NotNull(checkIn);
            Assert.Equal(createDto.UserId, checkIn.UserId);
            Assert.Equal(createDto.Mood, checkIn.Mood);
            Assert.Equal(createDto.Notes, checkIn.Notes);
        }

        [Fact]
        public async Task CreateCheckIn_InvalidMood_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CreateCheckInDto
            {
                UserId = "testuser",
                Mood = 10, // Invalid mood (should be 1-5)
                Notes = "Test notes"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/checkins", createDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateCheckIn_MissingUserId_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CreateCheckInDto
            {
                UserId = "", // Missing required field
                Mood = 3,
                Notes = "Test notes"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/checkins", createDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetCheckInsByUser_ExistingUser_ReturnsOk()
        {
            // Arrange
            var userId = "testuser123";
            
            // First, create a check-in
            var createDto = new CreateCheckInDto
            {
                UserId = userId,
                Mood = 3,
                Notes = "Test check-in"
            };
            
            await _client.PostAsJsonAsync("/api/checkins", createDto);

            // Act
            var response = await _client.GetAsync($"/api/checkins/user/{userId}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var checkIns = JsonSerializer.Deserialize<CheckInDto[]>(content, options);
            
            Assert.NotNull(checkIns);
            Assert.Single(checkIns);
            Assert.Equal(userId, checkIns[0].UserId);
        }

        [Fact]
        public async Task GetCheckInsByUser_NonExistingUser_ReturnsEmptyList()
        {
            // Act
            var response = await _client.GetAsync("/api/checkins/user/nonexistentuser");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var checkIns = JsonSerializer.Deserialize<CheckInDto[]>(content, options);
            
            Assert.NotNull(checkIns);
            Assert.Empty(checkIns);
        }

        [Fact]
        public async Task GetCheckIn_ExistingId_ReturnsOk()
        {
            // Arrange
            var createDto = new CreateCheckInDto
            {
                UserId = "testuser",
                Mood = 5,
                Notes = "Excellent day!"
            };
            
            var createResponse = await _client.PostAsJsonAsync("/api/checkins", createDto);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var createdCheckIn = JsonSerializer.Deserialize<CheckInDto>(createContent, options);

            // Act
            var response = await _client.GetAsync($"/api/checkins/{createdCheckIn!.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            var checkIn = JsonSerializer.Deserialize<CheckInDto>(content, options);
            
            Assert.NotNull(checkIn);
            Assert.Equal(createdCheckIn.Id, checkIn.Id);
            Assert.Equal(createDto.UserId, checkIn.UserId);
        }

        [Fact]
        public async Task GetCheckIn_NonExistingId_ReturnsNotFound()
        {
            // Act
            var response = await _client.GetAsync("/api/checkins/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task UpdateCheckIn_ValidData_ReturnsOk()
        {
            // Arrange
            var createDto = new CreateCheckInDto
            {
                UserId = "testuser",
                Mood = 3,
                Notes = "Original notes"
            };
            
            var createResponse = await _client.PostAsJsonAsync("/api/checkins", createDto);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var createdCheckIn = JsonSerializer.Deserialize<CheckInDto>(createContent, options);

            var updateDto = new UpdateCheckInDto
            {
                Mood = 5,
                Notes = "Updated notes"
            };

            // Act
            var response = await _client.PutAsJsonAsync($"/api/checkins/{createdCheckIn!.Id}", updateDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            
            var content = await response.Content.ReadAsStringAsync();
            var updatedCheckIn = JsonSerializer.Deserialize<CheckInDto>(content, options);
            
            Assert.NotNull(updatedCheckIn);
            Assert.Equal(updateDto.Mood, updatedCheckIn.Mood);
            Assert.Equal(updateDto.Notes, updatedCheckIn.Notes);
        }

        [Fact]
        public async Task UpdateCheckIn_NonExistingId_ReturnsNotFound()
        {
            // Arrange
            var updateDto = new UpdateCheckInDto
            {
                Mood = 5,
                Notes = "Updated notes"
            };

            // Act
            var response = await _client.PutAsJsonAsync("/api/checkins/999", updateDto);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task DeleteCheckIn_ExistingId_ReturnsNoContent()
        {
            // Arrange
            var createDto = new CreateCheckInDto
            {
                UserId = "testuser",
                Mood = 3,
                Notes = "To be deleted"
            };
            
            var createResponse = await _client.PostAsJsonAsync("/api/checkins", createDto);
            var createContent = await createResponse.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var createdCheckIn = JsonSerializer.Deserialize<CheckInDto>(createContent, options);

            // Act
            var response = await _client.DeleteAsync($"/api/checkins/{createdCheckIn!.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            
            // Verify deletion
            var getResponse = await _client.GetAsync($"/api/checkins/{createdCheckIn.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteCheckIn_NonExistingId_ReturnsNotFound()
        {
            // Act
            var response = await _client.DeleteAsync("/api/checkins/999");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
