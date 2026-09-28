using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyTaiChinh.Data;
using QuanLyTaiChinh.Data.Entities;
using QuanLyTaiChinh.Models;

namespace QuanLyTaiChinh.Services
{
    public class SavingGoalService
    {
        public async Task<List<SavingGoalItem>> GetAllAsync()
        {
            using var context = new FinanceWiseDbContext();

            var entities = await context.SavingGoals
                .Where(g => g.UserId == UserSession.CurrentUserId)
                .ToListAsync();

            return entities.Select(ToModel).ToList();
        }

        public async Task AddAsync(SavingGoalItem model)
        {
            using var context = new FinanceWiseDbContext();

            var entity = ToEntity(model);
            entity.UserId = UserSession.CurrentUserId;

            context.SavingGoals.Add(entity);
            await context.SaveChangesAsync();

            model.Id = entity.GoalId;
        }

        public async Task UpdateAsync(SavingGoalItem model)
        {
            using var context = new FinanceWiseDbContext();

            var entity = await context.SavingGoals.FindAsync(model.Id);
            if (entity is null) return;

            entity.Icon = model.Icon;
            entity.Name = model.Name;
            entity.DeadlineLabel = model.DeadlineLabel;
            entity.AccentColor = model.AccentColor;
            entity.CurrentAmount = model.CurrentAmount;
            entity.TargetAmount = model.TargetAmount;

            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int goalId)
        {
            using var context = new FinanceWiseDbContext();

            var entity = await context.SavingGoals.FindAsync(goalId);
            if (entity is null) return;

            context.SavingGoals.Remove(entity);
            await context.SaveChangesAsync();
        }

        private static SavingGoalItem ToModel(SavingGoalEntity e) => new()
        {
            Id = e.GoalId,
            Icon = e.Icon,
            Name = e.Name,
            DeadlineLabel = e.DeadlineLabel,
            AccentColor = e.AccentColor,
            CurrentAmount = e.CurrentAmount,
            TargetAmount = e.TargetAmount
        };

        private static SavingGoalEntity ToEntity(SavingGoalItem m) => new()
        {
            Icon = m.Icon,
            Name = m.Name,
            DeadlineLabel = m.DeadlineLabel,
            AccentColor = m.AccentColor,
            CurrentAmount = m.CurrentAmount,
            TargetAmount = m.TargetAmount
        };
    }
}