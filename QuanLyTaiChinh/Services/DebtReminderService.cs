using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using QuanLyTaiChinh.Data;
using QuanLyTaiChinh.Data.Entities;
using QuanLyTaiChinh.Models;

namespace QuanLyTaiChinh.Services
{
    public class DebtReminderService
    {
        private readonly FinanceWiseDbContext _context;

        public DebtReminderService(FinanceWiseDbContext context)
        {
            _context = context;
        }

        public async Task<List<DebtReminderItem>> GetItemsAsync(int userId)
        {
            var entities = await _context.DebtReminders
                .Where(x => x.UserId == userId)
                .OrderBy(x => x.DueDate)
                .ToListAsync();

            return entities.Select(e => new DebtReminderItem
            {
                Id = e.Id,
                Title = e.Title,
                PartnerName = e.PartnerName ?? string.Empty,
                CategoryType = e.CategoryType,
                PrincipalAmount = e.PrincipalAmount,
                InterestRate = e.InterestRate,
                PaidAmount = e.PaidAmount,
                DueDate = e.DueDate,
                RecurrenceCycle = e.RecurrenceCycle,
                Status = e.Status,
                Note = e.Note
            }).ToList();
        }

        public async Task AddItemAsync(int userId, DebtReminderItem item)
        {
            var entity = new DebtReminderEntity
            {
                UserId = userId,
                Title = item.Title,
                PartnerName = item.PartnerName,
                CategoryType = item.CategoryType,
                PrincipalAmount = item.PrincipalAmount,
                InterestRate = item.InterestRate,
                PaidAmount = 0,
                DueDate = item.DueDate,
                RecurrenceCycle = item.RecurrenceCycle,
                Status = "Unpaid",
                Note = item.Note
            };

            _context.DebtReminders.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task MarkAsCompletedAsync(int id)
        {
            var entity = await _context.DebtReminders.FindAsync(id);
            if (entity == null) return;

            entity.PaidAmount = entity.PrincipalAmount;
            entity.Status = "Completed";

            // Tự động gia hạn kỳ tiếp theo cho khoản định kỳ
            if (entity.RecurrenceCycle == "Monthly")
            {
                _context.DebtReminders.Add(new DebtReminderEntity
                {
                    UserId = entity.UserId,
                    Title = entity.Title,
                    PartnerName = entity.PartnerName,
                    CategoryType = entity.CategoryType,
                    PrincipalAmount = entity.PrincipalAmount,
                    InterestRate = entity.InterestRate,
                    PaidAmount = 0,
                    DueDate = entity.DueDate.AddMonths(1),
                    RecurrenceCycle = "Monthly",
                    Status = "Unpaid"
                });
            }
            else if (entity.RecurrenceCycle == "Yearly")
            {
                _context.DebtReminders.Add(new DebtReminderEntity
                {
                    UserId = entity.UserId,
                    Title = entity.Title,
                    PartnerName = entity.PartnerName,
                    CategoryType = entity.CategoryType,
                    PrincipalAmount = entity.PrincipalAmount,
                    InterestRate = entity.InterestRate,
                    PaidAmount = 0,
                    DueDate = entity.DueDate.AddYears(1),
                    RecurrenceCycle = "Yearly",
                    Status = "Unpaid"
                });
            }

            await _context.SaveChangesAsync();
        }
    }
}