using CarWashFinancialSystem.Data;
using CarWashFinancialSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarWashFinancialSystem.Services
{
    public class ReportService
    {
        private readonly AppDbContext _context;

        public ReportService()
        {
            _context = new AppDbContext();
        }

        public class DailyReport
        {
            public DateTime Date { get; set; }
            public decimal Revenue { get; set; }
            public decimal Expenses { get; set; }
            public decimal Profit { get; set; }
            public int TransactionsCount { get; set; }
        }

        public class ServiceStats
        {
            public string ServiceName { get; set; }
            public int Count { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal Percentage { get; set; }
        }

        // Отчет по дням за период
        // Отчет по дням за период
        public List<DailyReport> GetDailyReport(DateTime startDate, DateTime endDate)
        {
            var reports = new List<DailyReport>();

            for (var date = startDate.Date; date <= endDate.Date; date = date.AddDays(1))
            {
                var dayRevenue = _context.Transactions
                    .Where(t => t.TransactionDate.Date == date)
                    .Sum(t => (decimal?)t.Amount) ?? 0;

                var dayExpenses = _context.Expenses
                    .Where(e => e.ExpenseDate.Date == date) // ← ИСПРАВЛЕНО: ExpenseDate вместо Date
                    .Sum(e => (decimal?)e.Amount) ?? 0;

                reports.Add(new DailyReport
                {
                    Date = date,
                    Revenue = dayRevenue,
                    Expenses = dayExpenses,
                    Profit = dayRevenue - dayExpenses,
                    TransactionsCount = _context.Transactions.Count(t => t.TransactionDate.Date == date)
                });
            }
            App.Logger.LogInfo($"Создан ежедневный отчет: {startDate:dd.MM.yyyy} - {endDate:dd.MM.yyyy}", component: "Система отчётов");
            return reports;
        }
        // Статистика по услугам за период
        public List<ServiceStats> GetServiceStatistics(DateTime startDate, DateTime endDate)
        {
            var stats = _context.Transactions
                .Include(t => t.Service)
                .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .GroupBy(t => new { t.ServiceId, t.Service.Name })
                .Select(g => new ServiceStats
                {
                    ServiceName = g.Key.Name,
                    Count = g.Count(),
                    TotalAmount = g.Sum(t => t.Amount)
                })
                .ToList();

            var totalAmount = stats.Sum(s => s.TotalAmount);

            foreach (var stat in stats)
            {
                stat.Percentage = totalAmount > 0 ? (stat.TotalAmount / totalAmount) * 100 : 0;
            }

            return stats.OrderByDescending(s => s.TotalAmount).ToList();
        }

        // Общая финансовая сводка
        public dynamic GetFinancialSummary(DateTime startDate, DateTime endDate) // ← ИЗМЕНИЛ возвращаемый тип на dynamic
        {
            var totalRevenue = _context.Transactions
                .Where(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate)
                .Sum(t => (decimal?)t.Amount) ?? 0;

            var totalExpenses = _context.Expenses
                .Where(e => e.ExpenseDate >= startDate && e.ExpenseDate <= endDate) // ← ИСПРАВЛЕНО: ExpenseDate вместо Date
                .Sum(e => (decimal?)e.Amount) ?? 0;

            var transactionCount = _context.Transactions
                .Count(t => t.TransactionDate >= startDate && t.TransactionDate <= endDate);

            var avgCheck = transactionCount > 0 ? totalRevenue / transactionCount : 0;

            return new
            {
                TotalRevenue = totalRevenue,
                TotalExpenses = totalExpenses,
                Profit = totalRevenue - totalExpenses,
                TransactionCount = transactionCount,
                AverageCheck = avgCheck,
                StartDate = startDate,
                EndDate = endDate
            };
        }
        // Популярные услуги
        public List<ServiceStats> GetPopularServices(DateTime startDate, DateTime endDate, int topCount = 5)
        {
            return GetServiceStatistics(startDate, endDate).Take(topCount).ToList();
        }
    }
}