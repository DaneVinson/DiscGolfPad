using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dgp.Data.AzureServiceBusQueue;
using Dgp.Data.AzureTables;
using Dgp.Domain.Core;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

namespace Dgp.Service.Rest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) { app.UseDeveloperExceptionPage(); }

            app.UseRouting()
                .UseCors("AllowAllCorsPolicy")
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                })
                .UseSwagger()
                .UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/1.0/swagger.json", "Disc Gold Pad API"); })
                .UseHttpsRedirection();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .Configure<AzureStorageOptions>(Configuration.GetSection(nameof(AzureStorageOptions)))
                .AddSingleton<IValidator<Course>, CourseValidator>()
                .AddSingleton<IValidator<Delete<Course>>, DeleteValidator<Course>>()
                .AddSingleton<IValidator<Scorecard>, ScorecardValidator>()
                .AddSingleton<IValidator<Delete<Scorecard>>, DeleteValidator<Scorecard>>()
                .AddSingleton<IValidator<Hole>, HoleValidator>()
                .AddSingleton<IValidator<HoleScore>, HoleScoreValidator>()
                .AddTransient<IQueryProcessor, QueryProcessor>();

            // Azure Service Bus Queue messenger
            bool.TryParse(Configuration["UseDispatchingMessenger"], out bool useDispatchingMessenger);
            if (!useDispatchingMessenger)
            {
                services
                    .AddSingleton(Configuration.GetSection(nameof(AzureServiceBusOptions)).Get<AzureServiceBusOptions>())
                    .AddTransient<IMessenger<ICommand<Course>>, CommandMessenger<Course>>()
                    .AddTransient<IMessenger<ICommand<Scorecard>>, CommandMessenger<Scorecard>>();
            }
            // Otherwise use DispatchingMessenger
            else
            {
                services
                    .AddTransient<IMessenger<ICommand<Course>>, DispatchingMessenger<ICommand<Course>>>()
                    .AddTransient<IMessenger<ICommand<Scorecard>>, DispatchingMessenger<ICommand<Scorecard>>>()
                    .AddTransient<IMessenger<IEvent>, DispatchingMessenger<IEvent>>()
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

            services.AddSwaggerGen(options =>
                    {
                        options.SwaggerDoc("1.0", new OpenApiInfo { Title = "Disc Golf Pad API", Version = "1.0" });
                    })
                    .AddCors(options =>
                    {
                        options.AddPolicy("AllowAllCorsPolicy", builder =>
                        {
                            builder.AllowAnyOrigin()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader();
                        });
                    })
                    .AddControllers()
                    .ConfigureApiBehaviorOptions(options =>
                    {
                        // Enable/disable automatic validation
                        options.SuppressModelStateInvalidFilter = true;
                    });

        }


        private readonly IConfiguration Configuration;
    }
}
