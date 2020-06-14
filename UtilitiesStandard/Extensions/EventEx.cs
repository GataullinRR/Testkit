using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Extensions
{
    public static class EventEx
    {
        /// <summary>
        /// Awaits each handler, returns when all delegates are executed
        /// </summary>
        /// <param name="event"></param>
        /// <param name="sender"></param>
        /// <returns></returns>
        public static async Task InvokeAndWaitAsync(this Func<Task> @event)
        {
            foreach (var func in @event?.GetInvocationList()?.Cast<Func<Task>>() ?? Enumerable.Empty<Func<Task>>())
            {
                await func();
            } 
        }

        public static async Task InvokeAndWaitAsync<T>(this Func<T, Task> @event, T arg)
        {
            foreach (var func in @event?.GetInvocationList()?.Cast<Func<T, Task>>() ?? Enumerable.Empty<Func<T, Task>>())
            {
                await func(arg);
            }
        }

        public static void Invoke(this EventHandler @event, object sender)
        {
            @event.Invoke(sender, EventArgs.Empty);
        }
    }
}
