using Microsoft.EntityFrameworkCore;
using MentalHealthAPI.Data;
using MentalHealthAPI.DTOs;
using MentalHealthAPI.Models;
using MentalHealthAPI.Services;
using Xunit;

namespace MentalHealthAPI.Tests.Services
{
    public class CheckInServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly CheckInService _checkInService;

        public CheckInServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _checkInService = new CheckInService(_context);
        }

        [Fact]
        public async Task CreateCheckInAsync_ValidData_ReturnsCheckInDto()
        {
            // Arrange
            var createDto = new CreateCheckInDto
            {
                UserId = "user123",
                Mood = 4,
                Notes = "Feeling good today",
                Date = DateTime.UtcNow
            };

            // Act
            var result = await _checkInService.CreateCheckInAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createDto.UserId, result.UserId);
            Assert.Equal(createDto.Mood, result.Mood);
            Assert.Equal(createDto.Notes, result.Notes);
            Assert.True(result.Id > 0);
        }

        [Fact]
        public async Task CreateCheckInAsync_InvalidMood_ShouldValidateInController()
        {
            // Arrange - This test verifies the service accepts any mood value
            // (validation should happen at the controller level)
            var createDto = new CreateCheckInDto
            {
                UserId = "user123",
                Mood = 10, // Invalid mood (should be 1-5)
                Notes = "Test notes"
            };

            // Act
            var result = await _checkInService.CreateCheckInAsync(createDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(10, result.Mood); // Service doesn't validate, controller should
        }

        [Fact]
        public async Task GetCheckInsByUserAsync_ExistingUser_ReturnsCheckIns()
        {
            // Arrange
            var userId = "user123";
            var checkIn1 = new CheckIn
            {
                UserId = userId,
                Mood = 3,
                Notes = "First check-in",
                Date = DateTime.UtcNow.AddDays(-1),
                CreatedAt = DateTime.UtcNow.AddDays(-1)
            };
            var checkIn2 = new CheckIn
            {
                UserId = userId,
                Mood = 4,
                Notes = "Second check-in",
                Date = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.CheckIns.AddRange(checkIn1, checkIn2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _checkInService.GetCheckInsByUserAsync(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            
            // Should be ordered by date descending
            var checkIns = result.ToList();
            Assert.Equal(checkIn2.Id, checkIns[0].Id);
            Assert.Equal(checkIn1.Id, checkIns[1].Id);
        }

        [Fact]
        public async Task GetCheckInsByUserAsync_WithDateFilter_ReturnsFilteredResults()
        {
            // Arrange
            var userId = "user123";
            var oldDate = DateTime.UtcNow.AddDays(-10);
            var recentDate = DateTime.UtcNow.AddDays(-1);
            
            var oldCheckIn = new CheckIn
            {
                UserId = userId,
                Mood = 3,
                Date = oldDate,
                CreatedAt = oldDate
            };
            var recentCheckIn = new CheckIn
            {
                UserId = userId,
                Mood = 4,
                Date = recentDate,
                CreatedAt = recentDate
            };

            _context.CheckIns.AddRange(oldCheckIn, recentCheckIn);
            await _context.SaveChangesAsync();

            // Act
            var result = await _checkInService.GetCheckInsByUserAsync(
                userId, 
                fromDate: DateTime.UtcNow.AddDays(-5)
            );

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(recentCheckIn.Id, result.First().Id);
        }

        [Fact]
        public async Task GetCheckInByIdAsync_ExistingId_ReturnsCheckIn()
        {
            // Arrange
            var checkIn = new CheckIn
            {
                UserId = "user123",
                Mood = 5,
                Notes = "Great day!",
                Date = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.CheckIns.Add(checkIn);
            await _context.SaveChangesAsync();

            // Act
            var result = await _checkInService.GetCheckInByIdAsync(checkIn.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(checkIn.Id, result.Id);
            Assert.Equal(checkIn.UserId, result.UserId);
            Assert.Equal(checkIn.Mood, result.Mood);
        }

        [Fact]
        public async Task GetCheckInByIdAsync_NonExistingId_ReturnsNull()
        {
            // Act
            var result = await _checkInService.GetCheckInByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateCheckInAsync_ValidUpdate_ReturnsUpdatedCheckIn()
        {
            // Arrange
            var checkIn = new CheckIn
            {
                UserId = "user123",
                Mood = 3,
                Notes = "Original notes",
                Date = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.CheckIns.Add(checkIn);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateCheckInDto
            {
                Mood = 5,
                Notes = "Updated notes"
            };

            // Act
            var result = await _checkInService.UpdateCheckInAsync(checkIn.Id, updateDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateDto.Mood, result.Mood);
            Assert.Equal(updateDto.Notes, result.Notes);
            Assert.NotNull(result.UpdatedAt);
        }

        [Fact]
        public async Task UpdateCheckInAsync_NonExistingId_ReturnsNull()
        {
            // Arrange
            var updateDto = new UpdateCheckInDto
            {
                Mood = 5,
                Notes = "Updated notes"
            };

            // Act
            var result = await _checkInService.UpdateCheckInAsync(999, updateDto);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteCheckInAsync_ExistingId_ReturnsTrue()
        {
            // Arrange
            var checkIn = new CheckIn
            {
                UserId = "user123",
                Mood = 3,
                Date = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.CheckIns.Add(checkIn);
            await _context.SaveChangesAsync();

            // Act
            var result = await _checkInService.DeleteCheckInAsync(checkIn.Id);

            // Assert
            Assert.True(result);
            
            // Verify deletion
            var deletedCheckIn = await _context.CheckIns.FindAsync(checkIn.Id);
            Assert.Null(deletedCheckIn);
        }

        [Fact]
        public async Task DeleteCheckInAsync_NonExistingId_ReturnsFalse()
        {
            // Act
            var result = await _checkInService.DeleteCheckInAsync(999);

            // Assert
            Assert.False(result);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
