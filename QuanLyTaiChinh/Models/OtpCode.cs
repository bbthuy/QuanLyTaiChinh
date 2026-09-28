using System;
using System.Collections.Generic;

namespace QuanLyTaiChinh.Models;

public partial class OtpCode
{
    public int OtpId { get; set; }

    public int UserId { get; set; }

    public string CodeHash { get; set; } = null!;

    public string Type { get; set; } = null!;

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual User User { get; set; } = null!;
}
