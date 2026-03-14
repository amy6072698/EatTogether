using EatTogether.Models.DTOs;
using EatTogether.Models.EfModels;
using EatTogether.Models.Extensions;
using Microsoft.EntityFrameworkCore;

namespace EatTogether.Models.Repositories
{
    public class TableRepository : ITableRepository
    {
        private readonly EatTogetherDBContext _context;

        public TableRepository(EatTogetherDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TableDto>> GetAllAsync()
        {
            return await _context.Tables
                .OrderBy(t => t.TableName)
                .Select(t => t.ToDto())
                .ToListAsync();
        }

        public async Task<TableDto?> GetByIdAsync(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            return table?.ToDto();
        }

        public async Task CreateAsync(TableDto dto)
        {
            var table = new Table
            {
                TableName = dto.TableName,
                SeatCount = dto.SeatCount,
                Status = 0   // 新增桌位預設為空桌
            };
            _context.Tables.Add(table);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateStatusAsync(int id, int newStatus)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table == null) return;

            table.Status = newStatus;
            if (newStatus == 0 || newStatus == 1) table.Remark = null;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var table = await _context.Tables.FindAsync(id);
            if (table == null) return;

            _context.Tables.Remove(table);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsNameExistsAsync(string tableName, int excludeId = 0)
        {
            return await _context.Tables
                .AnyAsync(t => t.TableName == tableName && t.Id != excludeId);
        }

        public async Task UpdateRemarkAsync(int id, string? remark)
        {
            var t = await _context.Tables.FindAsync(id);
            if (t == null) return;
            t.Remark = remark;
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TableDto dto)
        {
            var table = await _context.Tables.FindAsync(dto.Id);
            if (table == null) return;

            table.TableName = dto.TableName;
            table.SeatCount = dto.SeatCount;
            table.Remark = dto.Remark;
            await _context.SaveChangesAsync();
        }
    }
}
