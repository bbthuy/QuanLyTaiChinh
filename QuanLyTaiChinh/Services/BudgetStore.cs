using System.Collections.ObjectModel;
using System.Threading.Tasks;
using QuanLyTaiChinh.Models;

namespace QuanLyTaiChinh.Services
{
    public class BudgetStore
    {
        private static BudgetStore? _instance;
        public static BudgetStore Instance => _instance ??= new BudgetStore();

        private readonly BudgetService _service = new();

        public ObservableCollection<BudgetItem> Budgets { get; } = new();

        private BudgetStore() { }

        public async Task LoadFromDatabaseAsync()
        {
            Budgets.Clear();
            foreach (var b in await _service.GetAllAsync())
                Budgets.Add(b);
        }

        public async Task AddAsync(BudgetItem item)
        {
            await _service.AddAsync(item);
            Budgets.Add(item);
        }

        public async Task UpdateAsync(BudgetItem item)
        {
            await _service.UpdateAsync(item);
        }

        public async Task DeleteAsync(BudgetItem item)
        {
            await _service.DeleteAsync(item.Id);
            Budgets.Remove(item);
        }
    }
}