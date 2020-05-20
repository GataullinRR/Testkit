using Microsoft.AspNetCore.SignalR;
using PresentationService.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PresentationService
{
    public interface IMainHub
    {
        Task TestRecorded(string userName, TestRecordedWebMessage message);
    }

    public class SignalRHub : Hub<IMainHub>
    {
        //public async Task SendTestRecordedAsync(string userName, TestRecordedWebMessage message)
        //{
        //    await Clients
        //        .User(userName)
        //        .SendAsync("TestRecorded", message);
        //}
    }
}
