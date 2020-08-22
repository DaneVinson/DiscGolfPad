namespace Dgp.Domain.Core

open System

type CourseDto = {
    PlayerId: string
    Id: string
    Name: string
    Location: string
    ImageUri: string
    HolePars: int[]
    HoleDistances: int[]
}

type ScorecardDto = {
    PlayerId: string
    Id: string
    CourseId: string
    Date: DateTimeOffset
    Notes: string
    HolePars: int[]
    HoleScores: int[]
}

type EntityId = {
    PlayerId: string
    Id: string
}

type PlayerMessage = {
    PlayerId: string
    Type: string
    Data: string
}

type CreateCourse = CreateCourse of CourseDto

type DeleteCourse = DeleteCourse of EntityId

type UpdateCourse = UpdateCourse of CourseDto

type CreateScorecard = CreateScorecard of ScorecardDto

type DeleteScorecard = DeleteScorecard of EntityId

type UpdateScorecard = UpdateScorecard of ScorecardDto

type CourseInfo = {
    PlayerId: string
    Id: string
    Name: string
    Par: int
    Holes: int
}

type ScorecardInfo = {
    PlayerId: string
    Id: string
    CourseId: string
    Date: DateTimeOffset
    HolePars: int[]
    HoleScores: int[]
}

type ScorecardQueryOptions = {
    PlayerId: string
    CourseId: string
}
