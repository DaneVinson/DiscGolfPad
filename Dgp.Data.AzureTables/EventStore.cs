using Dgp.Domain.Core;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Dgp.Data.AzureTables
{
    public class EventStore : IEventStore
    {
        public EventStore(IOptions<AzureStorageOptions> options)
        {
            Options = options?.Value ?? throw new ArgumentNullException();
        }


        public async Task<IEnumerable<IEvent>> GetEventsAsync(Guid entityId)
        {
            var records = await Options.GetCloudTable(EventTable)
                                        .ExecuteTableQueryAsync(new TableQuery<EventTableEntity>()
                                                                        .Where(TableQuery.GenerateFilterCondition(
                                                                                            "PartitionKey",
                                                                                            QueryComparisons.Equal,
                                                                                            entityId.ToString())));
            return records.OrderBy(r => r.Timestamp)
                            .Select(r => JsonConvert.DeserializeObject(r.Data, DomainAssembly.GetType(r.Type)) as IEvent);
        }

        public async Task<bool> PersistEventsAsync(IEnumerable<IEvent> events)
        {
            var tasks = new List<Task<IList<TableResult>>>();
            var cloudTable = Options.GetCloudTable(EventTable);

            foreach (var group in events.Select(e => new EventTableEntity(e)).GroupBy(e => e.PartitionKey))
            {
                var operation = new TableBatchOperation();
                foreach(var tableEntity in group)
                {
                    operation.Add(TableOperation.Insert(tableEntity));
                }
                tasks.Add(cloudTable.ExecuteBatchAsync(operation));
            }

            var results = await Task.WhenAll(tasks);

            // TODO: I don't like this but batch interaction with Azure Storeage Tables isn't ideal
            return results.SelectMany(r => r)
                            .Select(r => r.HttpStatusCode.IsSuccessCode())
                            .Any(r => r);
        }


        private const string EventTable = "Events";
        private readonly AzureStorageOptions Options;
        private static Assembly DomainAssembly => typeof(Course).Assembly;
    }
}
