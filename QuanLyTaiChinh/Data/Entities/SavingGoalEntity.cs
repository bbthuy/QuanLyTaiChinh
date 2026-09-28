using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyTaiChinh.Data.Entities
{
    [Table("SavingGoals")]
    public class SavingGoalEntity
    {
        [Key]
        public int GoalId { get; set; }

        public int UserId { get; set; }

        [MaxLength(10)]
        public string Icon { get; set; } = "🎯";

        [MaxLength(200)]
        public string Name { get; set; } = "";

        [MaxLength(100)]
        public string DeadlineLabel { get; set; } = "";

        [MaxLength(20)]
        public string AccentColor { get; set; } = "Blue";

        [Column(TypeName = "decimal(18,0)")]
        public decimal CurrentAmount { get; set; }

        [Column(TypeName = "decimal(18,0)")]
        public decimal TargetAmount { get; set; }
    }
}