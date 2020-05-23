﻿using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Types;
using Microsoft.AspNetCore.Components;
using PresentationService.API;

namespace Runner
{
    [Service(ServiceLifetime.Singleton)]
    class NotificationDaemon
    {
        [Inject] public IWebMessageHub WebMessageHub { get; set; }
        [Inject] public IMessageService MessageService { get; set; }

        public NotificationDaemon(IDependencyResolver di)
        {
            di.ResolveProperties(this);

            WebMessageHub.TestRecordedAsync += WebMessageHub_TestRecordedAsync;
            WebMessageHub.TestCompletedAsync += WebMessageHub_TestCompletedAsync;
        }

        async Task WebMessageHub_TestCompletedAsync(TestCompletedWebMessage arg)
        {
            MessageService.AddMessage($"{arg.TestId} completed with state {arg.RunResult}");
        }

        async Task WebMessageHub_TestRecordedAsync(TestRecordedWebMessage arg)
        {
            MessageService.AddMessage($"\"{arg.TestId}\" has been added");
        }
    }
}
