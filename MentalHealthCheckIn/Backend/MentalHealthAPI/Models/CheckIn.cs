using System.ComponentModel.DataAnnotations;

namespace MentalHealthAPI.Models
{
    public class CheckIn
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        public int Mood { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
