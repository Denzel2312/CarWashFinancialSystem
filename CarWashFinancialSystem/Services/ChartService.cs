using CarWashFinancialSystem.Data;
using CarWashFinancialSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarWashFinancialSystem.Services
{
    public class ChartService
    {
        private readonly AppDbContext _context;

        public ChartService()
        {
            _context = new AppDbContext();
        }

        // Данные для графика выручки по дням (последние 7 дней)
        public Dictionary<DateTime, decimal> GetRevenueLast7Days()
        {
            var endDate = DateTime.Today;
            var startDate = endDate.AddDays(-6);

            var revenueData = new Dictionary<DateTime, decimal>();

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var dailyRevenue = _context.Transactions
                    .Where(t => t.TransactionDate.Date == date)
                    .Sum(t => (decimal?)t.Amount) ?? 0;

                revenueData[date] = dailyRevenue;
            }

            return revenueData;
        }

        // Данные для круговой диаграммы по услугам (текущий месяц)
        public Dictionary<string, decimal> GetServiceDistributionCurrentMonth()
        {
            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var serviceData = _context.Transactions
                .Include(t => t.Service)
                .Where(t => t.TransactionDate >= firstDayOfMonth && t.TransactionDate <= lastDayOfMonth)
                .GroupBy(t => t.Service.Name)
                .Select(g => new { ServiceName = g.Key, TotalAmount = g.Sum(t => t.Amount) })
                .ToDictionary(x => x.ServiceName, x => x.TotalAmount);

            return serviceData;
        }

        // Данные для графика доходы vs расходы (последние 30 дней)
        public (Dictionary<DateTime, decimal> Revenue, Dictionary<DateTime, decimal> Expenses) GetRevenueVsExpensesLast30Days()
        {
            var endDate = DateTime.Today;
            var startDate = endDate.AddDays(-29);

            var revenueData = new Dictionary<DateTime, decimal>();
            var expensesData = new Dictionary<DateTime, decimal>();

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var dailyRevenue = _context.Transactions
                    .Where(t => t.TransactionDate.Date == date)
                    .Sum(t => (decimal?)t.Amount) ?? 0;

                var dailyExpenses = _context.Expenses
                    .Where(e => e.ExpenseDate.Date == date)
                    .Sum(e => (decimal?)e.Amount) ?? 0;

                revenueData[date] = dailyRevenue;
                expensesData[date] = dailyExpenses;
            }

            return (revenueData, expensesData);
        }

        // Статистика по способам оплаты
        public Dictionary<string, int> GetPaymentMethodStats()
        {
            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var paymentStats = _context.Transactions
                .Where(t => t.TransactionDate >= firstDayOfMonth && t.TransactionDate <= lastDayOfMonth)
                .GroupBy(t => t.PaymentMethod)
                .Select(g => new { Method = g.Key, Count = g.Count() })
                .ToDictionary(x => x.Method ?? "Не указан", x => x.Count);

            return paymentStats;
        }

        // Топ-5 самых популярных услуг
        public Dictionary<string, int> GetTopServicesByCount()
        {
            var firstDayOfMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            var topServices = _context.Transactions
                .Include(t => t.Service)
                .Where(t => t.TransactionDate >= firstDayOfMonth && t.TransactionDate <= lastDayOfMonth)
                .GroupBy(t => t.Service.Name)
                .Select(g => new { ServiceName = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count)
                .Take(5)
                .ToDictionary(x => x.ServiceName, x => x.Count);

            return topServices;
        }
    }
}