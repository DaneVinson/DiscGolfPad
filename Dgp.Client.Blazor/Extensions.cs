using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Dgp.Client.Blazor
{
    public static class Extensions
    {
        // String extensions
        public static T DeserializeJson<T>(this string json)
        {
            return JsonSerializer.Deserialize<T>(json, _defaultSerializerOptions);
        }


        // HttpClient extensions
        public static Task<TResult?> GetAsync<TResult>(this HttpClient httpClient, string uri) where TResult : class
        {
            return GetHttpResult<TResult>(() => httpClient.GetAsync(uri));
        }

        public static Task<TResult?> PostAsync<TResult>(this HttpClient httpClient, string uri, object obj) where TResult : class
        {
            return GetHttpResult<TResult>(() => httpClient.PostAsync(
                                                                uri,
                                                                new StringContent(
                                                                        JsonSerializer.Serialize(obj, _defaultSerializerOptions),
                                                                        Encoding.UTF8,
                                                                        "application/json")));
        }

        public static Task<TResult?> PutAsync<TResult>(this HttpClient httpClient, string uri, object obj) where TResult : class
        {
            return GetHttpResult<TResult>(() => httpClient.PutAsync(
                                                                uri,
                                                                new StringContent(
                                                                        JsonSerializer.Serialize(obj, _defaultSerializerOptions),
                                                                        Encoding.UTF8,
                                                                        "application/json")));
        }


        // HttpStatusCode extensions
        public static bool IsSuccess(this HttpStatusCode httpStatusCode)
        {
            return (int)httpStatusCode > 199 && (int)httpStatusCode < 300;
        }


        // Private
        private static async Task<TResult?> GetHttpResult<TResult>(Func<Task<HttpResponseMessage>> httpCall) where TResult : class
        {
            var response = await httpCall.Invoke();
            var json = await response.Content.ReadAsStringAsync();

            return string.IsNullOrWhiteSpace(json) ? default : json.DeserializeJson<TResult>();
        }

        private static readonly JsonSerializerOptions _defaultSerializerOptions = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
    }
}
