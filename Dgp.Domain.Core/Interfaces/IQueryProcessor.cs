using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Dgp.Domain.Core
{
    public interface IQueryProcessor
    {
        Task<Course> GetCourseAsync(IEntity entity);
        Task<IEnumerable<CourseInfo>> GetCoursesAsync(string playerId);
        Task<Scorecard> GetScorecardAsync(IEntity entity);
        Task<IEnumerable<ScorecardInfo>> GetScorecardsAsync(ScorecardsQueryOptions options);
    }
}
