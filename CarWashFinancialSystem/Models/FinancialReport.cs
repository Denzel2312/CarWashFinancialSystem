using System;
using System.ComponentModel.DataAnnotations;

namespace CarWashFinancialSystem.Models
{
    public class FinancialReport
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime ReportDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(50)]
        public string Period { get; set; } = string.Empty; // День, Неделя, Месяц

        // Доходы
        public decimal TotalRevenue { get; set; }
        public int ServicesCount { get; set; }
        public decimal AverageCheck { get; set; }

        // Расходы по категориям
        public decimal WaterExpenses { get; set; }
        public decimal ElectricityExpenses { get; set; }
        public decimal HeatingExpenses { get; set; }
        public decimal SalaryExpenses { get; set; }
        public decimal ChemicalExpenses { get; set; }
        public decimal RentExpenses { get; set; }
        public decimal MaintenanceExpenses { get; set; }
        public decimal OtherExpenses { get; set; }

        // Итоги
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit { get; set; }
        public decimal ProfitMargin { get; set; } // Рентабельность в %

        public string Notes { get; set; } = string.Empty;
    }
}