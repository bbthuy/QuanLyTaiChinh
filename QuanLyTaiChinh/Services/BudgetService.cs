using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyTaiChinh.Data;
using QuanLyTaiChinh.Data.Entities;
using QuanLyTaiChinh.Models;

namespace QuanLyTaiChinh.Services
{
    public class BudgetService
    {
        public async Task<List<BudgetItem>> GetAllAsync()
        {
            using var context = new FinanceWiseDbContext();

            var entities = await context.Budgets
                .Where(b => b.UserId == UserSession.CurrentUserId)
                .ToListAsync();

            return entities.Select(ToModel).ToList();
        }

        public async Task AddAsync(BudgetItem model)
        {
            using var context = new FinanceWiseDbContext();

            var entity = ToEntity(model);
            entity.UserId = UserSession.CurrentUserId;

            context.Budgets.Add(entity);
            await context.SaveChangesAsync();

            model.Id = entity.BudgetId;
        }

        public async Task UpdateAsync(BudgetItem model)
        {
            using var context = new FinanceWiseDbContext();

            var entity = await context.Budgets.FindAsync(model.Id);
            if (entity is null) return;

            entity.Icon = model.Icon;
            entity.Category = model.Category;
            entity.LimitAmount = model.LimitAmount;

            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int budgetId)
        {
            using var context = new FinanceWiseDbContext();

            var entity = await context.Budgets.FindAsync(budgetId);
            if (entity is null) return;

            context.Budgets.Remove(entity);
            await context.SaveChangesAsync();
        }

        // Luu y: SpentAmount KHONG luu xuong DB, luon tinh song lai tu bang Transactions
        private static BudgetItem ToModel(BudgetEntity e) => new()
        {
            Id = e.BudgetId,
            Icon = e.Icon,
            Category = e.Category,
            LimitAmount = e.LimitAmount
        };

        private static BudgetEntity ToEntity(BudgetItem m) => new()
        {
            Icon = m.Icon,
            Category = m.Category,
            LimitAmount = m.LimitAmount
        };
    }
}