using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Types
{
    public abstract class ServiceBase
    {
        static readonly JsonSerializerSettings JSON_SETTINGS = new JsonSerializerSettings()
        {
            Formatting = Formatting.Indented,
            TypeNameHandling = TypeNameHandling.All
        };

        readonly HttpClient _client;

        public ServiceBase(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        protected async Task<TResponse> MakeRequestAsync<TRequest, TResponse>(HttpMethod method, string url, TRequest request)
        {
            var json = JsonConvert.SerializeObject(request, JSON_SETTINGS);
            var stringContent = new StringContent(json, Encoding.UTF8, MediaTypeNames.Application.Json);
            var requestMessage = new HttpRequestMessage(method, url)
            {
                Content = stringContent
            };
            var response = await _client.SendAsync(requestMessage);

            response.EnsureSuccessStatusCode();

            var jsonResponse = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<TResponse>(jsonResponse, JSON_SETTINGS);
        }
    }
}
