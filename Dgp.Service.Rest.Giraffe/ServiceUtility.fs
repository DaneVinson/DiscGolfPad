namespace Dgp.Service.Rest.Giraffe

open System
open Microsoft.AspNetCore.Http
open Microsoft.Extensions.Configuration
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open Giraffe
open Dgp.Domain.Core

module ServiceUtility = 

    // Camel case serialization with Newtonsoft. Default for ASP.NET Core
    let serializeToJson entity = 
        JsonConvert.SerializeObject(
            entity,
            JsonSerializerSettings(ContractResolver = DefaultContractResolver(NamingStrategy = CamelCaseNamingStrategy())))

    let serializeAsJson entity : HttpHandler =
        setHttpHeader "Content-Type" "application/json" 
            >=> setBodyFromString (serializeToJson entity)

    // TODO: replace with auth claims
    let getPlayerId (context:HttpContext) = Utility.bilboId

    let isHttpSuccess (code:int) = code > 199 && code < 300