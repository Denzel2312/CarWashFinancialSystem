using CarWashFinancialSystem.Data;
using CarWashFinancialSystem.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CarWashFinancialSystem.Services
{
    public class ExpenseService
    {
        private readonly AppDbContext _context;

        public ExpenseService()
        {
            _context = new AppDbContext();
        }

        public List<Expense> GetAllExpenses()
        {
            return _context.Expenses
                .Include(e => e.Operator)
                .OrderByDescending(e => e.ExpenseDate)
                .ToList();
        }

        public List<Expense> GetExpensesForPeriod(DateTime startDate, DateTime endDate)
        {
            return _context.Expenses
                .Where(e => e.ExpenseDate >= startDate && e.ExpenseDate <= endDate)
                .Include(e => e.Operator)
                .OrderByDescending(e => e.ExpenseDate)
                .ToList();
        }

        public bool CreateExpense(Expense expense)
        {
            try
            {
                _context.Expenses.Add(expense);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateExpense(Expense expense)
        {
            try
            {
                _context.Expenses.Update(expense);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteExpense(int id)
        {
            try
            {
                var expense = _context.Expenses.Find(id);
                if (expense != null)
                {
                    _context.Expenses.Remove(expense);
                    _context.SaveChanges();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}