using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Runner
{
    public static class Extensions
    {
        public static void AddMessage(this IMessageService service, string message)
        {
            service.Messages.Add(new Message() { Text = message });
        }

        public static async Task<string?> GetValueAsync(this ICookieStorage cookies, string name)
        {
            var d = await cookies.GetAsync(name);
            
            return d?.Value;

            //return await cookies
            //    .GetAsync(name)
            //    .ThenDo(v => v?.Value);
        }
    }
}
