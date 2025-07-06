using Microsoft.AspNetCore.Mvc;
using MentalHealthAPI.DTOs;
using MentalHealthAPI.Services;

namespace MentalHealthAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CheckInsController : ControllerBase
    {
        private readonly ICheckInService _checkInService;

        public CheckInsController(ICheckInService checkInService)
        {
            _checkInService = checkInService;
        }

        /// <summary>
        /// Create a new mental health check-in
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CheckInDto>> CreateCheckIn([FromBody] CreateCheckInDto createCheckInDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var checkIn = await _checkInService.CreateCheckInAsync(createCheckInDto);
                return CreatedAtAction(nameof(GetCheckIn), new { id = checkIn.Id }, checkIn);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all check-ins with optional date filtering
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CheckInDto>>> GetAllCheckIns(
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var checkIns = await _checkInService.GetAllCheckInsAsync(fromDate, toDate);
                return Ok(checkIns);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Get all check-ins for a specific user
        /// </summary>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<CheckInDto>>> GetCheckInsByUser(
            string userId,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var checkIns = await _checkInService.GetCheckInsByUserAsync(userId, fromDate, toDate);
                return Ok(checkIns);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Get a specific check-in by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CheckInDto>> GetCheckIn(int id)
        {
            try
            {
                var checkIn = await _checkInService.GetCheckInByIdAsync(id);
                if (checkIn == null)
                    return NotFound($"Check-in with ID {id} not found.");

                return Ok(checkIn);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Update an existing check-in
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<CheckInDto>> UpdateCheckIn(int id, [FromBody] UpdateCheckInDto updateCheckInDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var checkIn = await _checkInService.UpdateCheckInAsync(id, updateCheckInDto);
                if (checkIn == null)
                    return NotFound($"Check-in with ID {id} not found.");

                return Ok(checkIn);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Delete a check-in
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCheckIn(int id)
        {
            try
            {
                var result = await _checkInService.DeleteCheckInAsync(id);
                if (!result)
                    return NotFound($"Check-in with ID {id} not found.");

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
