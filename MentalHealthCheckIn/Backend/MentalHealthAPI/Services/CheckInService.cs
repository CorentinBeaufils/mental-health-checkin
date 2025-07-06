using Microsoft.EntityFrameworkCore;
using MentalHealthAPI.Data;
using MentalHealthAPI.DTOs;
using MentalHealthAPI.Models;

namespace MentalHealthAPI.Services
{
    public class CheckInService : ICheckInService
    {
        private readonly ApplicationDbContext _context;

        public CheckInService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CheckInDto> CreateCheckInAsync(CreateCheckInDto createCheckInDto)
        {
            var checkIn = new CheckIn
            {
                UserId = createCheckInDto.UserId,
                Mood = createCheckInDto.Mood,
                Notes = createCheckInDto.Notes,
                Date = createCheckInDto.Date ?? DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow
            };

            _context.CheckIns.Add(checkIn);
            await _context.SaveChangesAsync();

            return MapToDto(checkIn);
        }

        public async Task<IEnumerable<CheckInDto>> GetAllCheckInsAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.CheckIns.AsQueryable();

            if (fromDate.HasValue)
                query = query.Where(c => c.Date >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(c => c.Date <= toDate.Value);

            var checkIns = await query
                .OrderByDescending(c => c.Date)
                .ToListAsync();

            return checkIns.Select(MapToDto);
        }

        public async Task<IEnumerable<CheckInDto>> GetCheckInsByUserAsync(string userId, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _context.CheckIns.Where(c => c.UserId == userId);

            if (fromDate.HasValue)
                query = query.Where(c => c.Date >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(c => c.Date <= toDate.Value);

            var checkIns = await query
                .OrderByDescending(c => c.Date)
                .ToListAsync();

            return checkIns.Select(MapToDto);
        }

        public async Task<CheckInDto?> GetCheckInByIdAsync(int id)
        {
            var checkIn = await _context.CheckIns.FindAsync(id);
            return checkIn != null ? MapToDto(checkIn) : null;
        }

        public async Task<CheckInDto?> UpdateCheckInAsync(int id, UpdateCheckInDto updateCheckInDto)
        {
            var checkIn = await _context.CheckIns.FindAsync(id);
            if (checkIn == null)
                return null;

            if (updateCheckInDto.Mood.HasValue)
                checkIn.Mood = updateCheckInDto.Mood.Value;

            if (updateCheckInDto.Notes != null)
                checkIn.Notes = updateCheckInDto.Notes;

            if (updateCheckInDto.Date.HasValue)
                checkIn.Date = updateCheckInDto.Date.Value;

            checkIn.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return MapToDto(checkIn);
        }

        public async Task<bool> DeleteCheckInAsync(int id)
        {
            var checkIn = await _context.CheckIns.FindAsync(id);
            if (checkIn == null)
                return false;

            _context.CheckIns.Remove(checkIn);
            await _context.SaveChangesAsync();
            return true;
        }

        private static CheckInDto MapToDto(CheckIn checkIn)
        {
            return new CheckInDto
            {
                Id = checkIn.Id,
                UserId = checkIn.UserId,
                Mood = checkIn.Mood,
                Notes = checkIn.Notes,
                Date = checkIn.Date,
                CreatedAt = checkIn.CreatedAt,
                UpdatedAt = checkIn.UpdatedAt
            };
        }
    }
}
