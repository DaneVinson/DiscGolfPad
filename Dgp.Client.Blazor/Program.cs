using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Dgp.Client.Blazor.ViewModels;
using Dgp.Client.Blazor.Services;
using Dgp.Domain.Core;
using FluentValidation;
using System.Net.Http.Headers;

namespace Dgp.Client.Blazor
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services
                    .AddSingleton(new ClientOptions("https://dgp-api-alpha.azurewebsites.net/api", "E593FEE6-BB9D-4118-8936-4F5A03820F85"))
                    .AddSingleton(typeof(IApiClient<,>), typeof(ApiClient<,>))
                    .AddSingleton<IAppStateService, AppStateService>()
                    .AddSingleton<IValidator<Course>, CourseValidator>()
                    .AddSingleton<IValidator<Delete<Course>>, DeleteValidator<Course>>()
                    .AddSingleton<IValidator<Scorecard>, ScorecardValidator>()
                    .AddSingleton<IValidator<Delete<Scorecard>>, DeleteValidator<Scorecard>>()
                    .AddSingleton<IValidator<Hole>, HoleValidator>()
                    .AddSingleton<IValidator<HoleScore>, HoleScoreValidator>().AddTransient<CoursesViewModel>()
                    .AddTransient<CoursesViewModel>()
                    .AddTransient<CourseViewModel>()
                    .AddTransient<ScorecardsViewModel>()
                    .AddTransient<ScorecardViewModel>()
                    .AddTransient(_ =>
                    {
                        var httpClient = new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) };
                        httpClient.DefaultRequestHeaders.Clear();
                        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                        return httpClient;
                    });

            await builder.Build().RunAsync();
        }
    }
}
