using Dgp.Domain.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dgp.Data.EFCosmosDB
{
    public class ProjectionManager<TEntity> : IProjectionManager<TEntity> where TEntity : IEntity
    {
        public ProjectionManager(DgpDbContext context)
        {
            Context = context ?? throw new ArgumentNullException();
        }


        public async Task CreateProjectionAsync(ICreatedEvent<TEntity> @event)
        {
            if (IgnoreEntityType()) { return; }

            switch (@event)
            {
                case CourseCreated c:
                    Context.Courses.Add(new CourseDocument(c));
                    break;
                case ScorecardCreated s:
                    Context.Scorecards.Add(new ScorecardDocument(s));
                    break;
            }

            await Context.SaveChangesAsync();
        }

        public async Task DeleteProjectionAsync(IDeletedEvent<TEntity> @event)
        {
            if (IgnoreEntityType()) { return; }

            switch (@event)
            {
                case Delete<Course> d:
                    var course = Context.Attach(new Course() { Id = d.Id, PlayerId = d.PlayerId });
                    Context.Remove(course);
                    break;
                case Delete<Scorecard> d:
                    var scorecard = Context.Attach(new Scorecard() { Id = d.Id, PlayerId = d.PlayerId });
                    Context.Remove(scorecard);
                    break;
            }

            await Context.SaveChangesAsync();
        }

        public async Task UpdateProjectionAsync(IUpdatedEvent<TEntity> @event)
        {
            if (IgnoreEntityType()) { return; }

            switch (@event)
            {
                case UpdateCourse u:
                    var courseDocument = await Context.Courses.FirstOrDefaultAsync(c => c.Id == u.Id);
                    if (courseDocument != null)
                    {
                        var course = JsonConvert.DeserializeObject<Course>(courseDocument.CourseData);
                        course.Holes = u.Holes;
                        course.ImageUri = u.ImageUri;
                        course.Location = u.Location;
                        course.Name = u.Name;
                        courseDocument.Name = u.Name;
                        courseDocument.CourseData = JsonConvert.SerializeObject(course);
                    }
                    break;
                case UpdateScorecard u:
                    var scorecardDocument = await Context.Scorecards.FirstOrDefaultAsync(s => s.Id == u.Id);
                    if (scorecardDocument != null)
                    {
                        var scorecard = JsonConvert.DeserializeObject<Scorecard>(scorecardDocument.ScorecardData);
                        scorecard.Date = u.Date;
                        scorecard.Scores = u.Scores;
                        scorecard.Notes = u.Notes;
                        scorecardDocument.Date = u.Date;
                        scorecardDocument.ScorecardData = JsonConvert.SerializeObject(scorecard);
                    }
                    break;
            }

            await Context.SaveChangesAsync();
        }


        private bool IgnoreEntityType()
        {
            // The Cosmos DB implementation only has Course and Scorecard projections
            return typeof(TEntity) != typeof(Course) && typeof(TEntity) != typeof(Scorecard); 
        }


        private readonly DgpDbContext Context;
    }
}
