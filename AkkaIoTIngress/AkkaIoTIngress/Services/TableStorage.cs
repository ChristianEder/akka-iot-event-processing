using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.RetryPolicies;
using Microsoft.WindowsAzure.Storage.Table;

namespace AkkaIoTIngress.Services
{
    public class TableStorage : ITableStorage
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly IRetryPolicy _retryPolicy;
        private readonly ConcurrentDictionary<string, Batch> _batches = new ConcurrentDictionary<string, Batch>();

        private class Batch
        {
            public List<ITableEntity> Inserts { get; } = new List<ITableEntity>();

            public void Insert<T>(T entity) where T : ITableEntity
            {
                Inserts.Add(entity);
            }
        }

        public TableStorage()
        {
            _storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("akka-iot-telemetry-storage"));
            _retryPolicy = new ExponentialRetry(TimeSpan.FromSeconds(5), 3);
        }

        public async Task CompleteBatch(string batchId)
        {
            if (_batches.TryRemove(batchId, out Batch batch))
            {
                await InsertBatch(batch.Inserts);
            }
        }

        public void Insert<T>(T entity, string batchId) where T : ITableEntity
        {
            var batch = GetBatch(batchId);
            batch.Insert(entity);
        }

        private Batch GetBatch(string batchId)
        {
            return _batches.GetOrAdd(batchId, _ => new Batch());
        }

        private Task InsertBatch(List<ITableEntity> batchInserts)
        {
            var insertByType = batchInserts.GroupBy(e => e.GetType());
            return Task.WhenAll(insertByType.Select(i => InsertBatch(GetTable(i.Key), i)));
        }

        private async Task InsertBatch(Task<CloudTable> tableTask, IEnumerable<ITableEntity> entities)
        {
            var table = await tableTask;

            var entitiesByPartition = entities.GroupBy(e => e.PartitionKey);
            foreach (var partition in entitiesByPartition)
            {
                var pagedBatch = Page(partition.ToList());
                foreach (var page in pagedBatch)
                {
                    var batch = new TableBatchOperation();
                    foreach (var insert in page.Select(TableOperation.Insert))
                    {
                        batch.Add(insert);
                    }

                    await table.ExecuteBatchAsync(batch);
                }
            }
        }

        private IEnumerable<IEnumerable<ITableEntity>> Page(List<ITableEntity> entities)
        {
            while (entities.Count > 100)
            {
                yield return entities.Take(100);
                entities = entities.Skip(100).ToList();
            }

            yield return entities;
        }

        private async Task<CloudTable> GetTable(Type entityType)
        {
            var tableClient = _storageAccount.CreateCloudTableClient();

            tableClient.DefaultRequestOptions.RetryPolicy = _retryPolicy;

            var table = tableClient.GetTableReference(entityType.Name);

            await table.CreateIfNotExistsAsync();
            return table;
        }
    }
}
