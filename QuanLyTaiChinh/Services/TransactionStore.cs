using System.Collections.ObjectModel;
using System.Threading.Tasks;
using QuanLyTaiChinh.Models;

namespace QuanLyTaiChinh.Services
{
    public class TransactionStore
    {
        private static TransactionStore? _instance;
        public static TransactionStore Instance => _instance ??= new TransactionStore();

        private readonly TransactionService _service = new();

        public ObservableCollection<TransactionItem> Transactions { get; } = new();

        private TransactionStore() { }

        // Goi 1 lan duy nhat, ngay sau khi dang nhap thanh cong (trong SignIn cua MainWindow)
        public async Task LoadFromDatabaseAsync()
        {
            Transactions.Clear();
            foreach (var t in await _service.GetAllAsync())
                Transactions.Add(t);
        }

        public async Task AddAsync(TransactionItem item)
        {
            await _service.AddAsync(item); 
            Transactions.Add(item);
        }

        public async Task UpdateAsync(TransactionItem item)
        {
            await _service.UpdateAsync(item);
             }

        public async Task DeleteAsync(TransactionItem item)
        {
            await _service.DeleteAsync(item.Id);
            Transactions.Remove(item);
        }
    }
}