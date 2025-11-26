using CarWashFinancialSystem.Data;
using CarWashFinancialSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarWashFinancialSystem.Services
{
    public class FinancialService
    {
        private readonly AppDbContext _context;

        public FinancialService()
        {
            _context = new AppDbContext();
        }

        // === РАСЧЁТЫ ДОХОДОВ ===
        public decimal GetRevenueForPeriod(DateTime startDate, DateTime endDate)
        {
            return _context.Transactions
                .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .Sum(t => t.Amount);
        }

        public int GetServicesCountForPeriod(DateTime startDate, DateTime endDate)
        {
            return _context.Transactions
                .Count(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate);
        }

        public decimal GetAverageCheckForPeriod(DateTime startDate, DateTime endDate)
        {
            var transactions = _context.Transactions
                .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .ToList();

            return transactions.Any() ? transactions.Average(t => t.Amount) : 0;
        }

        // === РАСЧЁТЫ РАСХОДОВ ===
        public Dictionary<string, decimal> GetExpensesByCategoryForPeriod(DateTime startDate, DateTime endDate)
        {
            var expenses = _context.Expenses
                .Where(e => e.ExpenseDate >= startDate && e.ExpenseDate <= endDate)
                .GroupBy(e => e.Category)
                .Select(g => new { Category = g.Key, Total = g.Sum(e => e.Amount) })
                .ToDictionary(x => x.Category, x => x.Total);

            // Гарантируем, что все категории присутствуют
            foreach (var category in ExpenseCategories.AllCategories)
            {
                if (!expenses.ContainsKey(category))
                    expenses[category] = 0;
            }

            return expenses;
        }

        public decimal GetTotalExpensesForPeriod(DateTime startDate, DateTime endDate)
        {
            return _context.Expenses
                .Where(e => e.ExpenseDate >= startDate && e.ExpenseDate <= endDate)
                .Sum(e => e.Amount);
        }

        // === ФИНАНСОВЫЕ ПОКАЗАТЕЛИ ===
        public FinancialReport GenerateDailyReport(DateTime date)
        {
            var startDate = date.Date;
            var endDate = date.Date.AddDays(1).AddSeconds(-1);

            var revenue = GetRevenueForPeriod(startDate, endDate);
            var servicesCount = GetServicesCountForPeriod(startDate, endDate);
            var averageCheck = GetAverageCheckForPeriod(startDate, endDate);
            var expensesByCategory = GetExpensesByCategoryForPeriod(startDate, endDate);
            var totalExpenses = GetTotalExpensesForPeriod(startDate, endDate);
            var netProfit = revenue - totalExpenses;
            var profitMargin = revenue > 0 ? (netProfit / revenue) * 100 : 0;

            return new FinancialReport
            {
                ReportDate = DateTime.Now,
                Period = "День",
                TotalRevenue = revenue,
                ServicesCount = servicesCount,
                AverageCheck = averageCheck,
                WaterExpenses = expensesByCategory[ExpenseCategories.Water],
                ElectricityExpenses = expensesByCategory[ExpenseCategories.Electricity],
                HeatingExpenses = expensesByCategory[ExpenseCategories.Heating],
                SalaryExpenses = expensesByCategory[ExpenseCategories.Salary],
                ChemicalExpenses = expensesByCategory[ExpenseCategories.Chemicals],
                RentExpenses = expensesByCategory[ExpenseCategories.Rent],
                MaintenanceExpenses = expensesByCategory[ExpenseCategories.Maintenance],
                OtherExpenses = expensesByCategory[ExpenseCategories.Other],
                TotalExpenses = totalExpenses,
                NetProfit = netProfit,
                ProfitMargin = profitMargin
            };
        }

        public FinancialReport GenerateMonthlyReport(int year, int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1).AddSeconds(-1);

            var revenue = GetRevenueForPeriod(startDate, endDate);
            var servicesCount = GetServicesCountForPeriod(startDate, endDate);
            var averageCheck = GetAverageCheckForPeriod(startDate, endDate);
            var expensesByCategory = GetExpensesByCategoryForPeriod(startDate, endDate);
            var totalExpenses = GetTotalExpensesForPeriod(startDate, endDate);
            var netProfit = revenue - totalExpenses;
            var profitMargin = revenue > 0 ? (netProfit / revenue) * 100 : 0;

            return new FinancialReport
            {
                ReportDate = DateTime.Now,
                Period = $"Месяц {month}.{year}",
                TotalRevenue = revenue,
                ServicesCount = servicesCount,
                AverageCheck = averageCheck,
                WaterExpenses = expensesByCategory[ExpenseCategories.Water],
                ElectricityExpenses = expensesByCategory[ExpenseCategories.Electricity],
                HeatingExpenses = expensesByCategory[ExpenseCategories.Heating],
                SalaryExpenses = expensesByCategory[ExpenseCategories.Salary],
                ChemicalExpenses = expensesByCategory[ExpenseCategories.Chemicals],
                RentExpenses = expensesByCategory[ExpenseCategories.Rent],
                MaintenanceExpenses = expensesByCategory[ExpenseCategories.Maintenance],
                OtherExpenses = expensesByCategory[ExpenseCategories.Other],
                TotalExpenses = totalExpenses,
                NetProfit = netProfit,
                ProfitMargin = profitMargin
            };
        }

        // === АНАЛИТИКА ===
        public Dictionary<string, decimal> GetServicePopularityForPeriod(DateTime startDate, DateTime endDate)
        {
            var transactions = _context.Transactions
                .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .Include(t => t.Service)
                .ToList();

            return transactions
                .GroupBy(t => t.Service.Name)
                .ToDictionary(g => g.Key, g => (decimal)g.Count());
        }

        public decimal CalculateWaterCostPerCar(DateTime startDate, DateTime endDate)
        {
            var waterExpenses = GetExpensesByCategoryForPeriod(startDate, endDate)[ExpenseCategories.Water];
            var carsCount = GetServicesCountForPeriod(startDate, endDate);

            return carsCount > 0 ? waterExpenses / carsCount : 0;
        }

        public decimal CalculateElectricityCostPerCar(DateTime startDate, DateTime endDate)
        {
            var electricityExpenses = GetExpensesByCategoryForPeriod(startDate, endDate)[ExpenseCategories.Electricity];
            var carsCount = GetServicesCountForPeriod(startDate, endDate);

            return carsCount > 0 ? electricityExpenses / carsCount : 0;
        }
    }
}