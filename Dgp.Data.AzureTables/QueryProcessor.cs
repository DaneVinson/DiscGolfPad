using Dgp.Domain.Core;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dgp.Data.AzureTables
{
    public class QueryProcessor : IQueryProcessor
    {
        public QueryProcessor(IOptions<AzureStorageOptions> options)
        {
            Options = options?.Value ?? throw new ArgumentNullException();
        }

        #region IQueryProcessor

        public async Task<Course> GetCourseAsync(IEntity entity)
        {
            return await GetEntityAsync<Course>(entity);
        }

        public async Task<IEnumerable<CourseInfo>> GetCoursesAsync(string playerId)
        {
            var courses = await GetPlayersEntitiesAsync<CourseInfo>(playerId);
            return courses.OrderBy(c => c.Name);
        }

        public async Task<Scorecard> GetScorecardAsync(IEntity entity)
        {
            return await GetEntityAsync<Scorecard>(entity);
        }

        public async Task<IEnumerable<ScorecardInfo>> GetScorecardsAsync(ScorecardsQueryOptions options)
        {
            // If there's no options get all the player's scorecards.
            if (options == null || (options.CourseId == null && options.From == null && options.To == null))
            {
                return await GetPlayersEntitiesAsync<ScorecardInfo>(options.PlayerId);
            }

            var filterConditions = new List<string>();

            filterConditions.Add(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, options.PlayerId));

            // Add CourseId filter if applicable.
            if (options.CourseId != null && options.CourseId != Guid.Empty)
            {
                filterConditions.Add(TableQuery.GenerateFilterConditionForGuid("CourseId", QueryComparisons.Equal, options.CourseId.Value));
            }

            // Add From filter if applicable.
            if (options.From != null)
            {
                filterConditions.Add(TableQuery.GenerateFilterConditionForDate("Date", QueryComparisons.GreaterThanOrEqual, options.From.Value));
            }

            // Add To filter if applicable.
            if (options.To != null)
            {
                filterConditions.Add(TableQuery.GenerateFilterConditionForDate("Date", QueryComparisons.LessThanOrEqual, options.To.Value));
            }

            // Build the query.
            var query = new TableQuery<ScorecardInfoTableEntity>();
            string filter = null;
            if (filterConditions.Count == 1) { filter = filterConditions.First(); }
            else
            {
                filter = TableQuery.CombineFilters(filterConditions[0], TableOperators.And, filterConditions[1]);
                for (int i = 2; i < filterConditions.Count; i++)
                {
                    filter = TableQuery.CombineFilters(filter, TableOperators.And, filterConditions[i]);
                }
            }
            query = query.Where(filter);

            // Execute the query and return ScorecardInfo data.
            var entities = await Options.GetCloudTable(nameof(ScorecardInfo)).ExecuteTableQueryAsync(query);
            return entities.Select(e => JsonConvert.DeserializeObject<ScorecardInfo>(e.Data)).OrderBy(s => s.Date);
        }

        #endregion

        private async Task<TEntity> GetEntityAsync<TEntity>(IEntity entity) where TEntity : class
        {
            var result = await Options.GetCloudTable(typeof(TEntity).Name)
                                        .ExecuteAsync(TableOperation.Retrieve<ModelTableEntity<TEntity>>(
                                                                        entity.PlayerId, 
                                                                        entity.Id.ToString()));
            if (!result.HttpStatusCode.IsSuccessCode()) { return null; }

            var dataEntity = result.Result as ModelTableEntity<TEntity>;
            return JsonConvert.DeserializeObject<TEntity>(dataEntity.Data);
        }

        private async Task<IEnumerable<TEntity>> GetPlayersEntitiesAsync<TEntity>(string playerId)
        {
            var entities = await Options.GetCloudTable(typeof(TEntity).Name)
                                        .ExecuteTableQueryAsync(new TableQuery<ModelTableEntity<TEntity>>()
                                                                        .Where(TableQuery.GenerateFilterCondition(
                                                                                            "PartitionKey",
                                                                                            QueryComparisons.Equal,
                                                                                            playerId)));
            return entities.Select(e => JsonConvert.DeserializeObject<TEntity>(e.Data));
        }

        private readonly AzureStorageOptions Options;
    }
}
