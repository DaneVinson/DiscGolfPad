namespace Dgp.Domain.Core

open System
open Microsoft.FSharp.Collections
open System.Threading.Tasks

type GetCourseAsync = EntityId -> Task<CourseDto>

type GetCoursesAsync = string -> Task<CourseInfo list>

type GetScorecardAsync = EntityId -> Task<ScorecardDto>

type GetScorecardsAsync = ScorecardQueryOptions -> Task<ScorecardInfo list>

type SendMessageAsync<'m> = 'm -> Task<unit>
