namespace Dgp.Service.Rest.Giraffe

open System
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Microsoft.WindowsAzure.Storage
open Microsoft.WindowsAzure.Storage.Table
open FSharp.Control.Tasks.V2
open Newtonsoft.Json
open Giraffe
open Dgp.Domain.Core

module AzTableQueryProcessor =

    let private getCloudTable (tableName:string) (connectionString:string) =
        CloudStorageAccount.Parse(connectionString)
                            .CreateCloudTableClient()
                            .GetTableReference(tableName)

    let private getEntity<'e> (tableEntity:DynamicTableEntity) =
        (tableEntity.Properties.Item "Data").StringValue
            |> JsonConvert.DeserializeObject<'e>

    let private executeQueryAsync<'e> =
        fun (query:TableQuery) (tableName:string) (connectionString:string) ->
            task {
                let table = getCloudTable tableName connectionString

                // ExecuteQuerySegmentedAsync is still all that's available for Azure Storage Tables 6/2019.
                // Null continuation token fetches the first 1000.
                let! querySegment = table.ExecuteQuerySegmentedAsync(query, null)
                return querySegment.Results
                        |> Seq.map (fun e -> getEntity<'e> e)
            }

    let private getCourseEntitiesAsync = 
        fun (connection:string) (playerId:string) ->
            task {
                let query = TableQuery().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, playerId))
                let! courses = executeQueryAsync<CourseInfo> query "CourseInfo" connection
                return courses |> Seq.toList
            }

    let private getEntityAsync<'e> =
        fun (connection:string) (tableName:string) (playerId:string) (id:string) ->
            task {
                let table = getCloudTable tableName connection
                let operation = TableOperation.Retrieve(playerId, id)
                let! result = table.ExecuteAsync operation
                return getEntity<'e> (result.Result :?> DynamicTableEntity)
            }

    let private getScorecardEntitiesAsync = 
        fun (connection:string) (playerId:string) (courseId:string) ->
            task {
                let playerFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, playerId)
                let query = 
                    match String.IsNullOrEmpty courseId with
                    | true -> TableQuery().Where(playerFilter)
                    | false -> 
                            let courseFilter = TableQuery.GenerateFilterConditionForGuid("CourseId", QueryComparisons.Equal, Guid.Parse(courseId))
                            TableQuery().Where(TableQuery.CombineFilters(playerFilter, TableOperators.And, courseFilter))
                let! scorecards = executeQueryAsync<ScorecardInfo> query "ScorecardInfo" connection
                return scorecards |> Seq.toList
            }

    // HttpContext -> domain adapter functions
    let getCourseAsync (context:HttpContext) =
        fun((entityId:EntityId)) ->
            let connection = context.GetService<IConfiguration>()
                                    .GetValue("AzureStorageOptions:ConnectionString")
            getEntityAsync<CourseDto> connection "Course" entityId.PlayerId entityId.Id

    let getCoursesAsync (context:HttpContext) =
        fun((playerId:string)) ->
            let connection = context.GetService<IConfiguration>()
                                    .GetValue("AzureStorageOptions:ConnectionString")
            getCourseEntitiesAsync connection playerId

    let getScorecardAsync (context:HttpContext) =
        fun((entityId:EntityId)) ->
            let connection = context.GetService<IConfiguration>()
                                    .GetValue("AzureStorageOptions:ConnectionString")
            getEntityAsync<ScorecardDto> connection "Scorecard" entityId.PlayerId entityId.Id

    let getScorecardsAsync (context:HttpContext) =
        fun((options:ScorecardQueryOptions)) ->
            let connection = context.GetService<IConfiguration>()
                                    .GetValue("AzureStorageOptions:ConnectionString")
            getScorecardEntitiesAsync connection options.PlayerId options.CourseId
    