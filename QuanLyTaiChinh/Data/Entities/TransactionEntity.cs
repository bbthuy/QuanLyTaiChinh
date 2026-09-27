using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using QuanLyTaiChinh.Models;

namespace QuanLyTaiChinh.Data.Entities
{
    [Table("Transactions")]
    public class TransactionEntity
    {
        [Key]
        public int TransactionId { get; set; }

        public int UserId { get; set; }

        [MaxLength(10)]
        public string Icon { get; set; } = "💰";

        [MaxLength(200)]
        public string Name { get; set; } = "";

        [MaxLength(100)]
        public string Category { get; set; } = "";

        public DateTime Date { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal Amount { get; set; }

        public TransactionType Type { get; set; }
    }
}