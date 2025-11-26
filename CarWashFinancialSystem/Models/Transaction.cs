using System;
using System.ComponentModel.DataAnnotations;

namespace CarWashFinancialSystem.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        [Required]
        public int ServiceId { get; set; }
        public Service Service { get; set; }

        [Required]
        public decimal Amount { get; set; }

        public string CarType { get; set; }
        public string LicensePlate { get; set; }
        public string PaymentMethod { get; set; } // "Cash", "Card"

        public int? OperatorId { get; set; }
        public User Operator { get; set; }

        public string Notes { get; set; }
    }
}