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

        // Lấy danh sách các khoản nhắc của User
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

        // Thêm mới một khoản nhắc / hóa đơn
        public async Task AddItemAsync(int userId, DebtReminderItem item)
        {
            var entity = new DebtReminderEntity
            {
                UserId = userId,
                Title = item.Title ?? string.Empty,
                PartnerName = item.PartnerName ?? string.Empty,
                CategoryType = item.CategoryType ?? "Borrowing",
                PrincipalAmount = item.PrincipalAmount,
                InterestRate = item.InterestRate,
                PaidAmount = 0,
                DueDate = item.DueDate,
                RecurrenceCycle = item.RecurrenceCycle ?? "Monthly",
                Status = "Unpaid",
                Note = item.Note ?? string.Empty,
                CreatedAt = DateTime.Now
            };

            _context.DebtReminders.Add(entity);
            await _context.SaveChangesAsync();
        }

        // 1. Chức năng Xóa
        public async Task DeleteItemAsync(int id)
        {
            var entity = await _context.DebtReminders.FindAsync(id);
            if (entity != null)
            {
                _context.DebtReminders.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }

        // 2. Chức năng Thanh toán từng phần (Trả bớt tiền)
        public async Task RecordPaymentAsync(int id, decimal amount)
        {
            var entity = await _context.DebtReminders.FindAsync(id);
            if (entity == null || amount <= 0) return;

            entity.PaidAmount += amount;
            if (entity.PaidAmount >= entity.PrincipalAmount)
            {
                entity.PaidAmount = entity.PrincipalAmount;
                entity.Status = "Completed";
            }
            else
            {
                entity.Status = "Partial";
            }

            await _context.SaveChangesAsync();
        }

        // 3. Hoàn tất / Tất toán toàn bộ
        public async Task MarkAsCompletedAsync(int id)
        {
            var entity = await _context.DebtReminders.FindAsync(id);
            if (entity == null) return;

            entity.PaidAmount = entity.PrincipalAmount;
            entity.Status = "Completed";

            // CHỈ tự động gia hạn nếu là khoản "Chi phí định kỳ" (Recurring)
            if (entity.CategoryType == "Recurring")
            {
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
                        Status = "Unpaid",
                        CreatedAt = DateTime.Now
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
                        Status = "Unpaid",
                        CreatedAt = DateTime.Now
                    });
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}