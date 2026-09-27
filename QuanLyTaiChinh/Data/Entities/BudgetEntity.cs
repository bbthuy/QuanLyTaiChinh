using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTaiChinh.Data.Entities
{
    [Table("Budgets")]
    public class BudgetEntity
    {
        [Key]
        public int BudgetId { get; set; }

        public int UserId { get; set; }

        [MaxLength(10)]
        public string Icon { get; set; } = "💰";

        [MaxLength(100)]
        public string Category { get; set; } = "";

        [Column(TypeName = "decimal(18,0)")]
        public decimal LimitAmount { get; set; }

        // Luu y: han muc ap dung cho thang hien tai, khong tach rieng theo tung thang/nam.
        // Neu ve sau can luu lich su han muc theo tung thang, them cot Month/Year vao day.
    }
}