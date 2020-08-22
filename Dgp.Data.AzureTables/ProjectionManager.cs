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
    public class ProjectionManager<TEntity> : IProjectionManager<TEntity> where TEntity : IEntity
    {
        public ProjectionManager(IOptions<AzureStorageOptions> options)
        {
            Options = options?.Value ?? throw new ArgumentNullException();
            UpdatedEventHandlers = new Dictionary<Type, Func<IUpdatedEvent<TEntity>, TEntity, TableEntity>>()
            {
                { typeof(CourseUpdated), (e, t) => UpdateCourse((CourseUpdated)e, t) },
                { typeof(CourseInfoUpdated), (e, t) => UpdateCourseInfo((CourseInfoUpdated)e, t) },
                { typeof(ScorecardUpdated), (e, t) => UpdateScorecard((ScorecardUpdated)e, t) },
                { typeof(ScorecardInfoUpdated), (e, t) => UpdateScorecardInfo((ScorecardInfoUpdated)e, t) }
            };
        }

        #region IProjectionManager

        public async Task CreateProjectionAsync(ICreatedEvent<TEntity> @event)
        {
            TableEntity tableEntity;
            if (@event.GetType() == typeof(ScorecardInfoCreated))
            {
                var scorecardInfo = new ScorecardInfo(@event as ScorecardInfoCreated);
                tableEntity = new ScorecardInfoTableEntity(
                                    scorecardInfo.PlayerId, 
                                    scorecardInfo.Id)
                {
                    CourseId = scorecardInfo.CourseId,
                    Data = JsonConvert.SerializeObject(scorecardInfo),
                    Date = scorecardInfo.Date.DateTime
                };                
            }
            else
            {
                var entity = (TEntity)Activator.CreateInstance(typeof(TEntity), @event);
                tableEntity = new ModelTableEntity<TEntity>(entity.PlayerId, entity.Id)
                {
                    Data = JsonConvert.SerializeObject(entity),
                };
            }

            var result = await Options.ExecuteWithTableAsync(
                                            typeof(TEntity).Name, 
                                            TableOperation.Insert(tableEntity));
            if (!result.HttpStatusCode.IsSuccessCode())
            {
                // TODO: log insert failure
            }
        }

        public async Task DeleteProjectionAsync(IDeletedEvent<TEntity> @event)
        {
            var result = await Options.ExecuteWithTableAsync(
                                        typeof(TEntity).Name,
                                        TableOperation.Delete(new DynamicTableEntity(
                                                                    @event.PlayerId,
                                                                    @event.Id.ToString()) { ETag = "*" }));
            if (!result.HttpStatusCode.IsSuccessCode())
            {
                // TODO: log delete failure
            }
        }

        public async Task UpdateProjectionAsync(IUpdatedEvent<TEntity> @event)
        {
            // Get the current projection.
            var result = await Options.ExecuteWithTableAsync(
                                            typeof(TEntity).Name, 
                                            TableOperation.Retrieve(
                                                            @event.PlayerId, 
                                                            @event.Id.ToString()));
            if (result?.Result == null || !result.HttpStatusCode.IsSuccessCode()) { return; }

            string json = (result.Result as DynamicTableEntity)?
                                .Properties
                                .FirstOrDefault(p => p.Key == "Data")
                                .Value?
                                .StringValue;
            var entity = JsonConvert.DeserializeObject<TEntity>(json);
            var tableEntity = UpdatedEventHandlers[@event.GetType()].Invoke(@event, entity);
            result = await Options.ExecuteWithTableAsync(typeof(TEntity).Name, TableOperation.Replace(tableEntity));
            if (!result.HttpStatusCode.IsSuccessCode())
            {
                // TODO: log update failure
            }
        }

        #endregion

        #region Updated Event Handlers

        public TableEntity UpdateCourse(CourseUpdated @event, TEntity entity)
        {
            var model = entity as Course;
            model.Holes = @event.Holes;
            model.ImageUri = @event.ImageUri;
            model.Location = @event.Location;
            model.Name = @event.Name;
            return new ModelTableEntity<Course>(
                            model.PlayerId, 
                            model.Id, 
                            JsonConvert.SerializeObject(model));
        }

        public TableEntity UpdateCourseInfo(CourseInfoUpdated @event, TEntity entity)
        {
            var model = entity as CourseInfo;
            model.Holes = @event.Holes;
            model.Name = @event.Name;
            model.Par = @event.Par;
            return new ModelTableEntity<Course>(
                            model.PlayerId,
                            model.Id,
                            JsonConvert.SerializeObject(model));
        }

        public TableEntity UpdateScorecard(ScorecardUpdated @event, TEntity entity)
        {
            var model = entity as Scorecard;
            model.Date = @event.Date;
            model.Scores = @event.Scores;
            model.Notes = @event.Notes;
            return new ModelTableEntity<Scorecard>(
                            model.PlayerId,
                            model.Id,
                            JsonConvert.SerializeObject(model));
        }

        public TableEntity UpdateScorecardInfo(ScorecardInfo @event, TEntity entity)
        {
            var model = entity as ScorecardInfo;
            model.Date = @event.Date;
            model.Scores = @event.Scores;
            return new ScorecardInfoTableEntity(
                            model.PlayerId,
                            model.Id,
                            JsonConvert.SerializeObject(model))
            {
                CourseId = model.CourseId,
                Date = model.Date.DateTime
            };
        }

        #endregion

        private readonly AzureStorageOptions Options;
        private readonly Dictionary<Type, Func<IUpdatedEvent<TEntity>, TEntity, TableEntity>> UpdatedEventHandlers;
    }
}
