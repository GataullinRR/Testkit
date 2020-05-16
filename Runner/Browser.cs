﻿using Microsoft.JSInterop;
using System.Linq;
using System.Threading.Tasks;

namespace Runner
{
    public class Browser
    {
        readonly IJSRuntime _runtime;

        public Browser(IJSRuntime runtime)
        {
            _runtime = runtime;
        }

        public async Task AlertAsync(string message)
        {
            await _runtime.InvokeAsync<object>("alert", new object[] { message });
        }
    }
}