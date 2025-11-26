using System;
using System.ComponentModel.DataAnnotations;

namespace CarWashFinancialSystem.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime ExpenseDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty; // Вода, Электричество, Отопление, Зарплата, Химия, Аренда

        [Required]
        [StringLength(200)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Amount { get; set; }

        public int? OperatorId { get; set; }
        public User Operator { get; set; }

        public string Notes { get; set; } = string.Empty;
    }

    public static class ExpenseCategories
    {
        public const string Water = "Водоснабжение";
        public const string Electricity = "Электричество";
        public const string Heating = "Отопление";
        public const string Salary = "Зарплаты";
        public const string Chemicals = "Химия и материалы";
        public const string Rent = "Аренда";
        public const string Maintenance = "Обслуживание";
        public const string Other = "Прочие расходы";

        public static string[] AllCategories = new[]
        {
            Water, Electricity, Heating, Salary, Chemicals, Rent, Maintenance, Other
        };
    }
}