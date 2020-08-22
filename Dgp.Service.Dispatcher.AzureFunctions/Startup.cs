using Dgp.Data.AzureServiceBusQueue;
using Dgp.Data.AzureTables;
using Dgp.Domain.Core;
using FluentValidation;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

[assembly: FunctionsStartup(typeof(Dgp.Service.CommandDispatcher.AzureFunctions.Startup))]

namespace Dgp.Service.CommandDispatcher.AzureFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.Configure<AzureStorageOptions>(Configuration.GetSection(nameof(AzureStorageOptions)))
                            .AddSingleton(Configuration.GetSection(nameof(AzureServiceBusOptions)).Get<AzureServiceBusOptions>())
                            .AddSingleton<IValidator<Course>, CourseValidator>()
                            .AddSingleton<IValidator<Delete<Course>>, DeleteValidator<Course>>()
                            .AddSingleton<IValidator<Scorecard>, ScorecardValidator>()
                            .AddSingleton<IValidator<Delete<Scorecard>>, DeleteValidator<Scorecard>>()
                            .AddSingleton<IValidator<HoleScore>, HoleScoreValidator>()
                            .AddSingleton<IValidator<Hole>, HoleValidator>()
                            .AddTransient<IMessenger<IEvent>, EventMessenger<IEvent>>()
                            .AddTransient<IDispatcher<ICommand<Course>>, CommandDispatcher<Course>>()
                            .AddTransient<IDispatcher<ICommand<Scorecard>>, CommandDispatcher<Scorecard>>()
                            .AddTransient<IDispatcher<IEvent>, EventDispatcher>()
                            .AddTransient<IEventStore, EventStore>()
                            .AddTransient<IProjectionManager<Course>, ProjectionManager<Course>>()
                            .AddTransient<IProjectionManager<CourseInfo>, ProjectionManager<CourseInfo>>()
                            .AddTransient<IProjectionManager<Scorecard>, ProjectionManager<Scorecard>>()
                            .AddTransient<IProjectionManager<ScorecardInfo>, ProjectionManager<ScorecardInfo>>()
                            .AddTransient<IAggregate<Course>, CourseAggregate>()
                            .AddTransient<IAggregate<Scorecard>, ScorecardAggregate>();
        }

        static Startup()
        {
            Configuration = new ConfigurationBuilder()
                                    .SetBasePath(Directory.GetCurrentDirectory())
                                    .AddJsonFile("appsettings.json", false, true)
                                    .AddJsonFile("appsettings.Development.json", true, true)
                                    .AddEnvironmentVariables()
                                    .Build();
        }

        private static readonly IConfiguration Configuration;
    }
}
