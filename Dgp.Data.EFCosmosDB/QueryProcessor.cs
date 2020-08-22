using Dgp.Domain.Core;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dgp.Data.EFCosmosDB
{
    public class QueryProcessor : IQueryProcessor
    {
        public QueryProcessor(DgpDbContext context)
        {
            Context = context ?? throw new ArgumentNullException();
        }


        public async Task<Course> GetCourseAsync(IEntity entity)
        {
            var courseDocument = await Context.Courses.FirstOrDefaultAsync(c => c.Id == entity.Id);
            if (courseDocument == null) { return null; }
            else { return JsonConvert.DeserializeObject<Course>(courseDocument.CourseData); }
        }

        public async Task<IEnumerable<CourseInfo>> GetCoursesAsync(string playerId)
        {
            return await Context.Courses
                                .Where(c => c.PlayerId == playerId)
                                .Select(c => new CourseInfo(JsonConvert.DeserializeObject<Course>(c.CourseData)))
                                .ToArrayAsync();
        }

        public async Task<Scorecard> GetScorecardAsync(IEntity entity)
        {
            var scorecardDocument = await Context.Scorecards.FirstOrDefaultAsync(s => s.Id == entity.Id);
            if (scorecardDocument == null) { return null; }
            else { return JsonConvert.DeserializeObject<Scorecard>(scorecardDocument.ScorecardData); }
        }

        public async Task<IEnumerable<ScorecardInfo>> GetScorecardsAsync(ScorecardsQueryOptions options)
        {
            var query = Context.Scorecards.AsQueryable();
            if (options?.CourseId == null || options.CourseId == Guid.Empty) { query = query.Where(s => s.PlayerId == options.PlayerId); }
            else { query = query.Where(s => s.CourseId == options.CourseId); }
            if (options.From != null) { query = query.Where(s => s.Date >= options.From); }
            if (options.To != null) { query = query.Where(s => s.Date <= options.To); }

            return await query.Select(s => new ScorecardInfo(JsonConvert.DeserializeObject<Scorecard>(s.ScorecardData)))
                                .ToArrayAsync();
        }


        private readonly DgpDbContext Context;
    }
}
