using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dgp.Data.AzureTables
{
    public static class Extensions
    {
        public static async Task<List<TElement>> ExecuteTableQueryAsync<TElement>(
            this CloudTable cloudTable,
            TableQuery<TElement> tableQuery)
            where TElement : TableEntity, new()
        {
            // Execute a segmentated query (1000 rows per request). This is the only
            // available async query mechanism for Azure Storage Tables (03/2018).
            TableContinuationToken continuationToken = null;
            List<TElement> list = new List<TElement>();
            do
            {
                var tableQueryResult = await cloudTable.ExecuteQuerySegmentedAsync(tableQuery, continuationToken);
                continuationToken = tableQueryResult.ContinuationToken;
                list.AddRange(tableQueryResult.Results);
            } while (continuationToken != null);

            // Return the list.
            return list;
        }

        public static async Task<TableResult> ExecuteWithTableAsync(this AzureStorageOptions options, string tableName, TableOperation operation)
        {
            return await options.GetCloudTable(tableName).ExecuteAsync(operation);
        }

        public static CloudTable GetCloudTable(this AzureStorageOptions options, string tableName)
        {
            var account = CloudStorageAccount.Parse($"DefaultEndpointsProtocol=https;AccountName={options.AccountName};AccountKey={options.Key}");
            var client = account?.CreateCloudTableClient();
            return client?.GetTableReference(tableName);
        }

        public static bool IsSuccessCode(this int httpStatusCode)
        {
            return httpStatusCode > 199 && httpStatusCode < 300;
        }
    }
}
