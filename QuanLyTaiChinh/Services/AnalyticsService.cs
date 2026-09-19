using System;
using System.Linq;
using System.Collections.Generic;

namespace QuanLyTaiChinh.Core.Services
{
    public class AnalyticsService
    {
        // Giả sử bạn có AppDbContext của Entity Framework
        // private readonly AppDbContext _context;
        // public AnalyticsService(AppDbContext context) { _context = context; }

        public (double[] Incomes, double[] Expenses, string[] Months) GetIncomeExpenseData(int year)
        {
            // Code thực tế gọi DB (Entity Framework):
            /*
            var transactions = _context.Transactions
                                       .Where(t => t.Date.Year == year)
                                       .ToList();
            */

            // Dữ liệu giả lập mô phỏng DB trả về
            var incomesFromDb = new double[] { 18, 21, 19, 23, 20, 26, 22, 28, 25 };
            var expensesFromDb = new double[] { 12, 15, 11, 16, 13, 15, 18, 15, 16 };
            var months = new string[] { "T1", "T2", "T3", "T4", "T5", "T6", "T7", "T8", "T9" };

            // Trả về tuple mảng double cho ViewModel
            return (incomesFromDb, expensesFromDb, months);
        }
    }
}