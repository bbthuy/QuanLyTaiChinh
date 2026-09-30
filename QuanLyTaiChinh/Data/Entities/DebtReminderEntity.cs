using QuanLyTaiChinh.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTaiChinh.Data.Entities
{
    [Table("DebtReminders")]
    public class DebtReminderEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(150)]
        public string? PartnerName { get; set; }

        [Required]
        [MaxLength(50)]
        public string CategoryType { get; set; } = "Borrowing";

        [Column(TypeName = "decimal(18,2)")]
        public decimal PrincipalAmount { get; set; }

        public double InterestRate { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal PaidAmount { get; set; }

        public DateTime DueDate { get; set; }

        [MaxLength(30)]
        public string RecurrenceCycle { get; set; } = "Monthly";

        [MaxLength(30)]
        public string Status { get; set; } = "Unpaid";

        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
    }
}