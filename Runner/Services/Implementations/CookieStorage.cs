using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Extensions;
using Utilities.Types;

namespace Runner
{
    [Service(ServiceLifetime.Singleton)]
    class CookieStorage : ICookieStorage
    {
        class CV
        {
            [JsonProperty("Exp")]
            public DateTime ExpireAt { get; set; }

            [JsonProperty("Val")]
            public string Value { get; set; }

            public CV(DateTime expireAt, string value)
            {
                ExpireAt = expireAt;
                Value = value ?? throw new ArgumentNullException(nameof(value));
            }
        }

        [Inject] public IJSRuntime Runtime { get; set; }

        public CookieStorage(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        public async Task<CookieValue?> GetAsync(string name)
        {
            var cv = await Runtime.InvokeAsync<string>("Cookies.get", name);
            var cvObj = cv == null
                ? null
                : JsonConvert.DeserializeObject<CV>(cv);
            return cvObj == null
                ? null
                : new CookieValue(cvObj.Value, cvObj.ExpireAt);
        }

        public async Task SetAsync(string name, CookieValue? value)
        {
            await Runtime.InvokeVoidAsync("Cookies.remove", name);
            if (value == null)
            {

            }
            else
            {
                await Runtime.InvokeVoidAsync("Cookies.set", name, JsonConvert.SerializeObject(new CV(value.ExpireAt, value.Value)), new { expires = value.ExpiresAfter.TotalDays.Round() });
            }
        }
    }
}
