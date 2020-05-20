using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
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
        readonly ConcurrentDictionary<string, CookieValue?> _cookies = new ConcurrentDictionary<string, CookieValue?>();

        public async Task<CookieValue?> GetAsync(string name)
        {
            return _cookies.TryGetValueOrDefault(name);
        }

        public async Task SetAsync(string name, CookieValue? value)
        {
            _cookies[name] = value;
        }
    }
}
