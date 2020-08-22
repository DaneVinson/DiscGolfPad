namespace Dgp.Domain.Core

open System
open Microsoft.FSharp.Collections

type CourseHole = private {
    Number: HoleNumber
    Par: Par
    Distance: HoleDistance
}

type HoleScore = private {
    Number: HoleNumber
    Score: Score
    Par: Par
}

type Course = {
    PlayerId: PlayerId
    Id: Id
    Name: EntityName
    Location: Location
    ImageUri: ImageUri
    Holes: CourseHole list
}

type Scorecard = {
    PlayerId: PlayerId
    CourseId: Id
    Id: Id
    Date: Date
    Notes: ScorecardNotes
    Scores: HoleScore list
}

module Utility =

    let getFailureMessage<'success> (result:Result<'success, string>) =
        match result with
            | Failure msg -> Some msg
            | Success _ -> None

    let getFailureMessages<'success> (results:Result<'success, string list>[]) =
        results |> Array.toList
                |> List.collect (fun result -> match result with    
                                                    | Failure errors -> errors
                                                    | Success _ -> List.empty)

    let getSuccessResults<'success, 'failure> (results:Result<'success, 'failure>[]) =
        results |> Array.toList
                |> List.choose (fun result -> match result with
                                                    | Success result -> Some result
                                                    | Failure _ -> None)

    let bilboId = "E593FEE6-BB9D-4118-8936-4F5A03820F85"


module CourseHole =

    let create (number:int, par:int, distance:int) = 
        let errors = List.choose (fun msg -> msg) [
            Utility.getFailureMessage (HoleNumber.create number)
            Utility.getFailureMessage (Par.create par)
            Utility.getFailureMessage (HoleDistance.create distance)
        ]

        if errors.IsEmpty then
            Success { 
                Number = HoleNumber number
                Par = Par par
                Distance = HoleDistance distance
            }
        else
            Failure errors

module HoleScore =

    let create (number:int, score:int, par:int) = 
        let errors = List.choose (fun msg -> msg) [
            Utility.getFailureMessage (HoleNumber.create number)
            Utility.getFailureMessage (Score.create score)
            Utility.getFailureMessage (Par.create par)
        ]

        if errors.IsEmpty then
            Success { 
                Number = HoleNumber number
                Score = Score score
                Par = Par par
            }
        else
            Failure errors

module Course =

    let create (dto:CourseDto) =

        let playerIdResult = PlayerId.create dto.PlayerId
        let idResult = Id.createFromString dto.Id
        let nameResult = EntityName.create dto.Name
        let locationResult = Location.create dto.Location
        let imageUriResult = ImageUri.createFromString dto.ImageUri
        let parsAndDistancesResults = Array.mapi2 (fun i par distance -> CourseHole.create(i+1, par, distance)) dto.HolePars dto.HoleDistances

        let errors = List.concat [ 
                            [
                                Utility.getFailureMessage playerIdResult
                                Utility.getFailureMessage idResult
                                Utility.getFailureMessage nameResult
                                Utility.getFailureMessage locationResult
                                Utility.getFailureMessage imageUriResult
                            ] |> List.choose (fun error -> error)
                            Utility.getFailureMessages parsAndDistancesResults 
        ]

        if not errors.IsEmpty then
            Failure errors
        else            
            Success {
                PlayerId = PlayerId dto.PlayerId
                Id = Id (Guid.Parse dto.Id)
                Name = EntityName dto.Name
                Location = Location dto.Location
                ImageUri = ImageUri dto.ImageUri
                Holes = Utility.getSuccessResults parsAndDistancesResults
            }

    let getCreateCourseCommand (playerId:string) (id:string) (dto:CourseDto) =
        let (e:CourseDto) = {
            Id = id
            HoleDistances = dto.HoleDistances
            HolePars = dto.HolePars
            ImageUri = dto.ImageUri
            Location = dto.Location
            Name = dto.Name
            PlayerId = playerId
        }
        match create e with
        | Success c -> Some (CreateCourse e)
        | Failure l -> None

    let getUpdateCourseCommand (dto:CourseDto) =
        match create dto with
        | Success c -> Some (UpdateCourse dto)
        | Failure l -> None


module Scorecard =

    let create (dto:ScorecardDto) =

        let playerIdResult = PlayerId.create dto.PlayerId
        let courseIdResult = Id.createFromString dto.CourseId
        let idResult = Id.createFromString dto.Id
        let dateResult = Date.create dto.Date
        let notesResult = ScorecardNotes.create dto.Notes
        let parsAndScoresResults = Array.mapi2 (fun i par score -> HoleScore.create(i+1, par, score)) dto.HolePars dto.HoleScores

        let errors = List.concat [ 
                            [
                                Utility.getFailureMessage playerIdResult
                                Utility.getFailureMessage courseIdResult
                                Utility.getFailureMessage idResult
                                Utility.getFailureMessage dateResult
                                Utility.getFailureMessage notesResult
                            ] |> List.choose (fun error -> error)
                            Utility.getFailureMessages parsAndScoresResults 
        ]

        if not errors.IsEmpty then
            Failure errors
        else            
            Success {
                PlayerId = PlayerId dto.PlayerId
                CourseId = Id (Guid.Parse dto.CourseId)
                Id = Id (Guid.Parse dto.Id)
                Date = Date dto.Date
                Notes = ScorecardNotes dto.Notes
                Scores = Utility.getSuccessResults parsAndScoresResults
            }

    let getCreateScorecardCommand (playerId:string) (id:string) (dto:ScorecardDto) =
        let (e:ScorecardDto) = {
            CourseId = dto.CourseId
            Date = dto.Date
            HolePars = dto.HolePars
            HoleScores = dto.HoleScores
            Id = id
            Notes = dto.Notes
            PlayerId = playerId
        }
        match create e with
        | Success c -> Some (CreateScorecard e)
        | Failure l -> None

    let getUpdateScorecardCommand (dto:ScorecardDto) =
        match create dto with
        | Success c -> Some (UpdateScorecard dto)
        | Failure l -> None
