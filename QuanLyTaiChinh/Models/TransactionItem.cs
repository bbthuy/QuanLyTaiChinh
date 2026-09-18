using System;
using System.Collections.Generic;
using System.Text;

namespace QuanLyTaiChinh.Models
{
    public enum TransactionType { Income, Expense, Saving }
    public class TransactionItem
    {
        public int Id { get; set; }
        public string Icon { get; set; } = "💰";
        public string Name { get; set; } = "";
        public string Category { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.Today;
        public decimal Amount { get; set; }
        public TransactionType Type { get; set; }
    }
}
