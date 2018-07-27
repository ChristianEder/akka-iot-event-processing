using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace AkkaIoTIngress.Services
{
    public interface ITableStorage
    {
        Task CompleteBatch(string batchId);
        void Insert<T>(T entity, string batchId) where T : ITableEntity;
    }
}