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

            WebMessageHub.TestAddedAsync += WebMessageHub_TestRecordedAsync;
            WebMessageHub.TestCompletedAsync += WebMessageHub_TestCompletedAsync;
        }

        async Task WebMessageHub_TestCompletedAsync(TestCompletedWebMessage arg)
        {
            MessageService.AddMessage($"\"{arg.TestName}\" completed with state {arg.RunResult}");
        }

        async Task WebMessageHub_TestRecordedAsync(TestAddedWebMessage arg)
        {
            MessageService.AddMessage($"\"{arg.TestName}\" has been added");
        }
    }
}
