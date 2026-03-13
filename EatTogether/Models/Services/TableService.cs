using EatTogether.Models.DTOs;
using EatTogether.Models.Infra;
using EatTogether.Models.Repositories;

namespace EatTogether.Models.Services
{
    public class TableService
    {
        private readonly ITableRepository _repo;
        public TableService(ITableRepository repo) { _repo = repo; }

        public async Task<IEnumerable<TableDto>> GetAllAsync() => await _repo.GetAllAsync();


        public async Task<TableDto?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

        public async Task<Result> UpdateAsync(TableDto dto)
        {
            var existing = await _repo.GetByIdAsync(dto.Id);
            if (existing == null)
                return Result.Fail("找不到此桌位");
            if (existing.TableName != dto.TableName &&
                await _repo.IsNameExistsAsync(dto.TableName, dto.Id))
                return Result.Fail($"桌位名稱「{dto.TableName}」已存在，請更換其他名稱");
            await _repo.UpdateAsync(dto);
            return Result.Success();
        }

        public async Task<Result> CreateAsync(TableDto dto)
        {
            if (await _repo.IsNameExistsAsync(dto.TableName))
                return Result.Fail($"桌位名稱「{dto.TableName}」已存在，請更換其他名稱");
            await _repo.CreateAsync(dto);
            return Result.Success();
        }

        public async Task<Result> UpdateStatusAsync(int id, int newStatus)
        {
            var table = await _repo.GetByIdAsync(id);
            if (table == null) return Result.Fail("找不到此桌位");
            if (table.Status == 1 && newStatus == 2)
                return Result.Fail("用餐中的桌位無法直接標記為保留，請先結帳恢復空桌");
            await _repo.UpdateStatusAsync(id, newStatus);
            return Result.Success();
        }

        public async Task<Result> DeleteAsync(int id)
        {
            var table = await _repo.GetByIdAsync(id);
            if (table == null) return Result.Fail("找不到此桌位");
            if (table.Status == 1) return Result.Fail("用餐中的桌位無法刪除");
            await _repo.DeleteAsync(id);
            return Result.Success();
        }
    }

}
