using System.ComponentModel.DataAnnotations;

namespace CarWashFinancialSystem.Models
{
    public class Service
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        public string Description { get; set; } = string.Empty;
        public int DurationMinutes { get; set; }
        public bool IsActive { get; set; } = true;
    }
}