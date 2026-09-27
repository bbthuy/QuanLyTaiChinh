using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyTaiChinh.Data;
using QuanLyTaiChinh.Data.Entities;
using QuanLyTaiChinh.Models;

namespace QuanLyTaiChinh.Services
{
    public class TransactionService
    {
        public async Task<List<TransactionItem>> GetAllAsync()
        {
            using var context = new FinanceWiseDbContext();

            var entities = await context.Transactions
                .Where(t => t.UserId == UserSession.CurrentUserId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();

            return entities.Select(ToModel).ToList();
        }

        public async Task AddAsync(TransactionItem model)
        {
            using var context = new FinanceWiseDbContext();

            var entity = ToEntity(model);
            entity.UserId = UserSession.CurrentUserId;

            context.Transactions.Add(entity);
            await context.SaveChangesAsync();

            model.Id = entity.TransactionId; // lay lai Id vua duoc DB tu sinh
        }

        public async Task UpdateAsync(TransactionItem model)
        {
            using var context = new FinanceWiseDbContext();

            var entity = await context.Transactions.FindAsync(model.Id);
            if (entity is null) return;

            entity.Icon = model.Icon;
            entity.Name = model.Name;
            entity.Category = model.Category;
            entity.Date = model.Date;
            entity.Amount = model.Amount;
            entity.Type = model.Type;

            await context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int transactionId)
        {
            using var context = new FinanceWiseDbContext();

            var entity = await context.Transactions.FindAsync(transactionId);
            if (entity is null) return;

            context.Transactions.Remove(entity);
            await context.SaveChangesAsync();
        }

        private static TransactionItem ToModel(TransactionEntity e) => new()
        {
            Id = e.TransactionId,
            Icon = e.Icon,
            Name = e.Name,
            Category = e.Category,
            Date = e.Date,
            Amount = e.Amount,
            Type = e.Type
        };

        private static TransactionEntity ToEntity(TransactionItem m) => new()
        {
            Icon = m.Icon,
            Name = m.Name,
            Category = m.Category,
            Date = m.Date,
            Amount = m.Amount,
            Type = m.Type
        };
    }
}