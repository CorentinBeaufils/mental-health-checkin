using System.ComponentModel.DataAnnotations;

namespace MentalHealthAPI.DTOs
{
    public class CreateCheckInDto
    {
        [Required]
        [StringLength(100)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Range(1, 5, ErrorMessage = "Mood must be between 1 and 5")]
        public int Mood { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime? Date { get; set; }
    }

    public class UpdateCheckInDto
    {
        [Range(1, 5, ErrorMessage = "Mood must be between 1 and 5")]
        public int? Mood { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime? Date { get; set; }
    }

    public class CheckInDto
    {
        public int Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public int Mood { get; set; }
        public string? Notes { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
