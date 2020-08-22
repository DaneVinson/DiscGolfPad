module Dgp.Service.Rest.Giraffe.App

open System
open System.IO
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Cors.Infrastructure
open Microsoft.AspNetCore.Hosting
open Microsoft.Extensions.Logging
open Microsoft.Extensions.DependencyInjection
open Giraffe
open Dgp.Service.Rest.Giraffe
open Microsoft.Extensions.Configuration

// Web app
let webApp =
    choose [
        GET >=>
            choose [
                route "/api/courses" >=> HttpHandlers.getCourses
                routef "/api/courses/%s" (fun id -> HttpHandlers.getCourse id)
                route "/api/scorecards" >=> HttpHandlers.getScorecards
                routef "/api/scorecards/%s" (fun id -> HttpHandlers.getScorecard id)
                route "/api/handshake" >=> text "true"
            ]
        POST >=>
            choose [
                route "/api/courses" >=> HttpHandlers.createCourse
                route "/api/scorecards" >=> HttpHandlers.createScorecard
            ]
        PUT >=>
            choose [
                routef "/api/courses/%s" (fun _ -> HttpHandlers.updateCourse)
                routef "/api/scorecards/%s" (fun _ -> HttpHandlers.updateScorecard)
            ]
        DELETE >=>
            choose [
                routef "/api/courses/%s" (fun id -> HttpHandlers.deleteCourse id)
                routef "/api/scorecards/%s" (fun id -> HttpHandlers.deleteScorecard id)
            ]
        setStatusCode 404 >=> text "Not Found" ]

// Error handler
let errorHandler (ex : Exception) (logger : ILogger) =
    logger.LogError(ex, "An unhandled exception has occurred while executing the request.")
    clearResponse >=> setStatusCode 500 >=> text ex.Message

// Config and Main
let configureCors (builder : CorsPolicyBuilder) =
    builder.WithOrigins("http://localhost:8080")
           .AllowAnyMethod()
           .AllowAnyHeader()
           |> ignore

let configureApp (app : IApplicationBuilder) =
    let env = app.ApplicationServices.GetService<IHostingEnvironment>()
    (match env.IsDevelopment() with
    | true  -> app.UseDeveloperExceptionPage()
    | false -> app.UseGiraffeErrorHandler errorHandler)
        .UseHttpsRedirection()
        .UseCors(configureCors)
        .UseStaticFiles()
        .UseGiraffe(webApp)

let configureServices (services : IServiceCollection) =
    services.AddCors()    |> ignore
    services.AddGiraffe() |> ignore

let configureLogging (builder : ILoggingBuilder) =
    builder.AddFilter(fun l -> l.Equals LogLevel.Error)
           .AddConsole()
           .AddDebug() |> ignore

let configureAppConfiguration  (context: WebHostBuilderContext) (config: IConfigurationBuilder) =  
    config
        .AddJsonFile("appsettings.json",false,true)
        .AddJsonFile(sprintf "appsettings.%s.json" context.HostingEnvironment.EnvironmentName ,true)
        .AddEnvironmentVariables() |> ignore

[<EntryPoint>]
let main _ =
    let contentRoot = Directory.GetCurrentDirectory()
    let webRoot     = Path.Combine(contentRoot, "WebRoot")
    WebHostBuilder()
        .UseKestrel()
        .UseContentRoot(contentRoot)
        .UseIISIntegration()
        .UseWebRoot(webRoot)
        .ConfigureAppConfiguration(configureAppConfiguration)
        .Configure(Action<IApplicationBuilder> configureApp)
        .ConfigureServices(configureServices)
        .ConfigureLogging(configureLogging)
        .Build()
        .Run()
    0