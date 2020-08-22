using Dgp.Data.AzureTables;
using Dgp.Data.EFCosmosDB;
using Dgp.Data.Seed;
using Dgp.Domain.Core;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dgp.AdminConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var id = Guid.Empty.ToString();
                var serviceProvider = NewServiceProvider(DataRepository.AzureTables);

                await DeleteAndAddTables(serviceProvider);
                await SeedDataAsync(serviceProvider);

                var validator = serviceProvider.GetService<IValidator<Hole>>();
                var results = validator.Validate(new Hole(1, 0));
                results.Errors.Add(new ValidationFailure("Par", "some bs"));
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} - {1}", ex.GetType(), ex.Message);
                Console.WriteLine(ex.StackTrace ?? String.Empty);
            }
            finally
            {
                Console.WriteLine();
                Console.WriteLine("...");
                Console.ReadKey();
            }
            await Task.CompletedTask;
        }


        private static async Task DeleteAndAddTables(ServiceProvider serviceProvider)
        {
            var tableNames = new string[]
            {
                "Events",
                nameof(Course),
                nameof(CourseInfo),
                nameof(Scorecard),
                nameof(ScorecardInfo)
            };
            var options = serviceProvider.GetService<IOptions<AzureStorageOptions>>()?.Value;
            var cloudTables = tableNames.ToDictionary(n => n, n => options.GetCloudTable(n));

            // Delete tables if they exist.
            var deleteTasks = tableNames.ToDictionary<string, string, Task<bool>>(n => n, n => null);
            foreach (var tableName in cloudTables.Keys)
            {
                deleteTasks[tableName] = cloudTables[tableName].DeleteIfExistsAsync();
            }
            await Task.WhenAll(deleteTasks.Values);
            foreach (var tableName in deleteTasks.Keys)
            {
                Console.WriteLine($"Existing {tableName} table deleted? {deleteTasks[tableName].Result}");
            }

            Console.WriteLine();

            // Wait for delete requests to finish executing. Azure Storeage Table deletes are not
            // necessarily complete when the async task returns.
            if (deleteTasks.Values.Any(v => v.Result))
            {
                var pauseSeconds = 120;
                Console.WriteLine($"pausing {pauseSeconds} seconds for Azure to finish deletes...hopefully.");
                await Task.Delay(TimeSpan.FromSeconds(pauseSeconds));
                Console.WriteLine();
            }

            // Create tables.
            var createTasks = tableNames.ToDictionary<string, string, Task>((n) => n, n => null);
            foreach (var tableName in cloudTables.Keys)
            {
                createTasks[tableName] = cloudTables[tableName].CreateAsync();
            }
            await Task.WhenAll(createTasks.Values);
            foreach (var tableName in createTasks.Keys)
            {
                Console.WriteLine($"Created table {tableName}.");
            }

            Console.WriteLine();
        }

        private static async Task CosmosDBEfProvider()
        {
            var dataRepository = DataRepository.EfCosmosDb;

            using (var serviceProvider = NewServiceProvider(dataRepository))
            {
                // TODO: This is because of an EF -> Cosmos DB issue with the current preview bits
                if (dataRepository == DataRepository.EfCosmosDb)
                {
                    using (var context = serviceProvider.GetService<DgpDbContext>())
                    {
                        await context.Database.EnsureCreatedAsync();
                    }
                }

                await SeedDataAsync(serviceProvider);
                //await DeleteAndAddTables(serviceProvider);
            }
        }

        private static ServiceProvider NewServiceProvider(DataRepository dataRepository)
        {
            var configuration = new ConfigurationBuilder()
                                        .SetBasePath(Directory.GetCurrentDirectory())
                                        .AddJsonFile("appsettings.json", false, true)
                                        .AddEnvironmentVariables()
                                        .Build();

            var services = new ServiceCollection();

            // Configuration
            services.Configure<AzureStorageOptions>(configuration.GetSection(nameof(AzureStorageOptions)))
                    .Configure<CosmosDbOptions>(configuration.GetSection(nameof(CosmosDbOptions)));

            // Domain
            services.AddTransient<IAggregate<Course>, CourseAggregate>()
                    .AddTransient<IAggregate<Scorecard>, ScorecardAggregate>();

            // Validators
            services.AddSingleton<IValidator<Course>, CourseValidator>()
                    .AddSingleton<IValidator<Delete<Course>>, DeleteValidator<Course>>()
                    .AddSingleton<IValidator<Scorecard>, ScorecardValidator>()
                    .AddSingleton<IValidator<Delete<Scorecard>>, DeleteValidator<Scorecard>>()
                    .AddSingleton<IValidator<Hole>, HoleValidator>()
                    .AddSingleton<IValidator<HoleScore>, HoleScoreValidator>();

            // Messaging
            services.AddTransient<IMessenger<ICommand<Course>>, DispatchingMessenger<ICommand<Course>>>()
                    .AddTransient<IMessenger<ICommand<Scorecard>>, DispatchingMessenger<ICommand<Scorecard>>>()
                    .AddTransient<IMessenger<IEvent>, DispatchingMessenger<IEvent>>()
                    .AddTransient<IDispatcher<ICommand<Course>>, CommandDispatcher<Course>>()
                    .AddTransient<IDispatcher<ICommand<Scorecard>>, CommandDispatcher<Scorecard>>()
                    .AddTransient<IDispatcher<IEvent>, EventDispatcher>();

            // Persistence
            switch (dataRepository)
            {
                case DataRepository.AzureTables:
                    services.AddTransient<IEventStore, Data.AzureTables.EventStore>()
                            .AddTransient<IQueryProcessor, Data.AzureTables.QueryProcessor>()
                            .AddTransient<IProjectionManager<Course>, Data.AzureTables.ProjectionManager<Course>>()
                            .AddTransient<IProjectionManager<CourseInfo>, Data.AzureTables.ProjectionManager<CourseInfo>>()
                            .AddTransient<IProjectionManager<Scorecard>, Data.AzureTables.ProjectionManager<Scorecard>>()
                            .AddTransient<IProjectionManager<ScorecardInfo>, Data.AzureTables.ProjectionManager<ScorecardInfo>>();
                    break;
                case DataRepository.EfCosmosDb:
                    services.AddTransient<DgpDbContext, DgpDbContext>()
                            .AddTransient<IEventStore, Data.EFCosmosDB.EventStore>()
                            .AddTransient<IQueryProcessor, Data.EFCosmosDB.QueryProcessor>()
                            .AddTransient<IProjectionManager<Course>, Data.EFCosmosDB.ProjectionManager<Course>>()
                            .AddTransient<IProjectionManager<CourseInfo>, Data.EFCosmosDB.ProjectionManager<CourseInfo>>()
                            .AddTransient<IProjectionManager<Scorecard>, Data.EFCosmosDB.ProjectionManager<Scorecard>>()
                            .AddTransient<IProjectionManager<ScorecardInfo>, Data.EFCosmosDB.ProjectionManager<ScorecardInfo>>();
                    break;
                default:
                    throw new Exception($"Persistence services not set for {dataRepository}.");
            }

            return services.BuildServiceProvider();
        }

        private static async Task SeedDataAsync(ServiceProvider serviceProvider)
        {
            var seedPack = BilbosSeedService.NewSeedPack();

            var createCourseCommands = seedPack.Courses.Select(c => new CreateCourse(c)).ToArray();

            // Use a new messenger for each command to ensure we have fresh context objects
            await Task.WhenAll(createCourseCommands.Select(c => serviceProvider.GetService<IMessenger<ICommand<Course>>>().SendAsync(c)));

            // Map original course Ids to those assigned by the CreateCourse command.
            var courseIds = new Dictionary<Guid, Guid>();
            for (int i = 0; i < seedPack.Courses.Length; i++)
            {
                courseIds.Add(seedPack.Courses[i].Id, createCourseCommands[i].Id);
            }

            // Update the CourseId values on all CreateScorecard using the values from CreateCourse
            await Task.WhenAll(seedPack.Scorecards
                                        .Select(s => serviceProvider.GetService<IMessenger<ICommand<Scorecard>>>()
                                                                    .SendAsync(new CreateScorecard(s)
                                                                    {
                                                                        CourseId = courseIds[s.CourseId]
                                                                    })));
        }

        private enum DataRepository
        {
            AzureTables,
            EfCosmosDb
        }
    }
}
