namespace Dgp.Service.Rest.Giraffe

open System
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open FSharp.Control.Tasks.V2
open Giraffe
open Dgp.Domain.Core
open Dgp.Service.Rest.Giraffe

module HttpHandlers =

    //-------------------------
    // Command
    //-------------------------
    let createCourse = 
        fun (next:HttpFunc) (context:HttpContext) ->
            task {
                let! command = context.BindJsonAsync<CourseDto>()
                let result = Course.getCreateCourseCommand (ServiceUtility.getPlayerId context) (Guid.NewGuid().ToString()) command                
                if result.IsSome then
                    let! _ = AzServiceBusMessenger.sendCommandMessageAsync context command (typeof<CreateCourse>.ToString())
                    return! (setStatusCode 202 >=> ServiceUtility.serializeAsJson command) next context
                else
                    return! setStatusCode 400 next context
            }

    let createScorecard = 
        fun (next:HttpFunc) (context:HttpContext) ->
            task {
                let! command = context.BindJsonAsync<ScorecardDto>()
                let result = Scorecard.getCreateScorecardCommand (ServiceUtility.getPlayerId context) (Guid.NewGuid().ToString()) command                
                if result.IsSome then
                    let! _ = AzServiceBusMessenger.sendCommandMessageAsync context command (typeof<CreateScorecard>.ToString())
                    return! (setStatusCode 202 >=> ServiceUtility.serializeAsJson command) next context
                else
                    return! setStatusCode 400 next context
            }
            
    let deleteCourse id = 
        fun (next:HttpFunc) (context:HttpContext) ->
            task {
                let command = DeleteCourse {
                                PlayerId = ServiceUtility.getPlayerId context
                                Id = id 
                }
                let! _ = AzServiceBusMessenger.sendCommandMessageAsync context command (typeof<DeleteCourse>.ToString())
                return! setStatusCode 202 next context
            }

    let deleteScorecard id = 
        fun (next:HttpFunc) (context:HttpContext) ->
            task {
                let command = DeleteScorecard {
                                PlayerId = ServiceUtility.getPlayerId context
                                Id = id 
                }
                let! _ = AzServiceBusMessenger.sendCommandMessageAsync context command (typeof<DeleteScorecard>.ToString())
                return! setStatusCode 202 next context
            }

    let updateCourse = 
        fun (next:HttpFunc) (context:HttpContext) ->
            task {
                let! command = context.BindJsonAsync<CourseDto>()
                let result = Course.getUpdateCourseCommand command
                if result.IsSome then
                    let! _ = AzServiceBusMessenger.sendCommandMessageAsync context command (typeof<UpdateCourse>.ToString())
                    return! (setStatusCode 202 >=> ServiceUtility.serializeAsJson command) next context
                else
                    return! setStatusCode 400 next context
            }

    let updateScorecard = 
        fun (next:HttpFunc) (context:HttpContext) ->
            task {
                let! command = context.BindJsonAsync<ScorecardDto>()
                let result = Scorecard.getUpdateScorecardCommand command                
                if result.IsSome then
                    let! _ = AzServiceBusMessenger.sendCommandMessageAsync context command (typeof<UpdateScorecard>.ToString())
                    return! (setStatusCode 202 >=> ServiceUtility.serializeAsJson command) next context
                else
                    return! setStatusCode 400 next context
            }

    //-------------------------
    // Query
    //-------------------------
    let getCourse id =
        fun (next:HttpFunc) (context:HttpContext) ->
            task {
                let playerId = ServiceUtility.getPlayerId context
                let entityId = { Id = id; PlayerId = playerId; }
                let (f:GetCourseAsync) = AzTableQueryProcessor.getCourseAsync context
                let! course = f entityId
                return! ServiceUtility.serializeAsJson course next context
            }

    let getCourses =
        fun (next:HttpFunc) (context:HttpContext) ->
            task {
                let playerId = ServiceUtility.getPlayerId context
                let (f:GetCoursesAsync) = AzTableQueryProcessor.getCoursesAsync context
                let! courses = f playerId
                return! ServiceUtility.serializeAsJson courses next context
            }

    let getScorecard id =
        fun (next:HttpFunc) (context:HttpContext) ->
            task {
                let playerId = ServiceUtility.getPlayerId context
                let entityId = { Id = id; PlayerId = playerId; }
                let (f:GetScorecardAsync) = AzTableQueryProcessor.getScorecardAsync context
                let! scorecard = f entityId
                return! ServiceUtility.serializeAsJson scorecard next context
            }

    let getScorecards =
        fun (next:HttpFunc) (context:HttpContext) ->
            task {
                let playerId = ServiceUtility.getPlayerId context
                let courseId = 
                     match context.TryGetQueryStringValue "courseId" with
                     | None -> null
                     | Some id -> id
                let options = { PlayerId = playerId; CourseId = courseId; }
                let (f:GetScorecardsAsync) = AzTableQueryProcessor.getScorecardsAsync context
                let! scorecards = f options
                return! ServiceUtility.serializeAsJson scorecards next context
            }
