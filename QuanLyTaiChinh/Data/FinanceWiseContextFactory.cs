using Microsoft.EntityFrameworkCore;

namespace QuanLyTaiChinh.Data;

public static class FinanceWiseContextFactory
{
    public static FinanceWiseDbContext Create()
    {
        var options = new DbContextOptionsBuilder<FinanceWiseDbContext>()
            .UseSqlServer(
                @"Server=(localdb)\MSSQLLocalDB;Database=FinanceWiseDB;Trusted_Connection=True;TrustServerCertificate=True;")
            .Options;

        return new FinanceWiseDbContext(options);
    }
}