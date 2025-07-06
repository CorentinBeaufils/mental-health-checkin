using MentalHealthAPI.DTOs;

namespace MentalHealthAPI.Services
{
    public interface ICheckInService
    {
        Task<CheckInDto> CreateCheckInAsync(CreateCheckInDto createCheckInDto);
        Task<IEnumerable<CheckInDto>> GetAllCheckInsAsync(DateTime? fromDate = null, DateTime? toDate = null);
        Task<IEnumerable<CheckInDto>> GetCheckInsByUserAsync(string userId, DateTime? fromDate = null, DateTime? toDate = null);
        Task<CheckInDto?> GetCheckInByIdAsync(int id);
        Task<CheckInDto?> UpdateCheckInAsync(int id, UpdateCheckInDto updateCheckInDto);
        Task<bool> DeleteCheckInAsync(int id);
    }
}
