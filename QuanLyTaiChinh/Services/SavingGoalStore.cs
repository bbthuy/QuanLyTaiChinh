using System.Collections.ObjectModel;
using System.Threading.Tasks;
using QuanLyTaiChinh.Models;

namespace QuanLyTaiChinh.Services
{
    public class SavingGoalStore
    {
        private static SavingGoalStore? _instance;
        public static SavingGoalStore Instance => _instance ??= new SavingGoalStore();

        private readonly SavingGoalService _service = new();

        public ObservableCollection<SavingGoalItem> Goals { get; } = new();

        private SavingGoalStore() { }

        public async Task LoadFromDatabaseAsync()
        {
            Goals.Clear();
            foreach (var g in await _service.GetAllAsync())
                Goals.Add(g);
        }

        public async Task AddAsync(SavingGoalItem item)
        {
            await _service.AddAsync(item);
            Goals.Add(item);
        }

        public async Task UpdateAsync(SavingGoalItem item)
        {
            await _service.UpdateAsync(item);
        }

        public async Task DeleteAsync(SavingGoalItem item)
        {
            await _service.DeleteAsync(item.Id);
            Goals.Remove(item);
        }
    }
}