using System;

namespace QuanLyTaiChinh.Models
{
    public class DebtReminderItem
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string PartnerName { get; set; } = string.Empty;
        public string CategoryType { get; set; } = "Borrowing";
        public decimal PrincipalAmount { get; set; }
        public double InterestRate { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime DueDate { get; set; }
        public string RecurrenceCycle { get; set; } = "Monthly";
        public string Status { get; set; } = "Unpaid";
        public string? Note { get; set; }

        public decimal RemainingAmount => Math.Max(0, PrincipalAmount - PaidAmount);
        public bool IsOverdue => Status != "Completed" && DueDate.Date < DateTime.Today;
        public bool IsDueSoon => Status != "Completed" && !IsOverdue && (DueDate.Date - DateTime.Today).TotalDays <= 3;

        public string CategoryDisplay => CategoryType switch
        {
            "Borrowing" => "Khoản vay / Thẻ TD",
            "Lending" => "Người khác nợ",
            "Recurring" => "Chi phí định kỳ",
            _ => CategoryType
        };

        public string StatusDisplay => Status switch
        {
            "Completed" => "Đã hoàn tất",
            _ => IsOverdue ? "ĐÃ QUÁ HẠN" : (IsDueSoon ? "SẮP ĐẾN HẠN" : "Chưa thanh toán")
        };
    }
}