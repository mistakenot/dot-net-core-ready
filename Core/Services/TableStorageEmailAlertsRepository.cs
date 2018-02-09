using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace DotNetCoreReady.Services
{
    public class TableStorageEmailAlertsRepository : IEmailAlertsRepository
    {
        private readonly CloudTable _table;

        public TableStorageEmailAlertsRepository(string connectionString, string tableName)
        {
            var storageAccount = CloudStorageAccount.Parse(connectionString);
            var tableClient = storageAccount.CreateCloudTableClient();
            _table = tableClient.GetTableReference(tableName);
        }

        public async Task<CreateResult> CreateIfNotExists(string email, string packageId, bool optedInToMarketing)
        {
            await _table.CreateIfNotExistsAsync();

            var newEntity = new EmailAlert()
            {
                PartitionKey = packageId,
                RowKey = email,
                CreatedAt = DateTime.UtcNow,
                OptedInToMarketing = optedInToMarketing
            };

            var op = TableOperation.InsertOrReplace(newEntity);
            var result = await _table.ExecuteAsync(op);
            
            if (result.HttpStatusCode != 204)
            {
                return new CreateResult()
                {
                    WasSuccessful = false,
                    ErrorCode = result.HttpStatusCode
                };
            }

            return new CreateResult();
        }

        private class EmailAlert : TableEntity
        {
            public DateTime CreatedAt { get; set; }

            public bool OptedInToMarketing { get; set; }
        }
    }
}