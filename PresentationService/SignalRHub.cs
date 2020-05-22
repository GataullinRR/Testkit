using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.API;
using Utilities.Types;

namespace PresentationService
{
    public class SignalRHub : Hub<IMainHub>
    {
        [Inject] public UserService.API.UserService.UserServiceClient UserService { get; set; }

        public SignalRHub(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        public async Task Bind(string authToken)
        {
            var uInfReq = new GetUserInfoRequest();
            uInfReq.Token = authToken;
            var uInfResp = await UserService.GetUserInfoAsync(uInfReq);

            await Groups.AddToGroupAsync(Context.ConnectionId, uInfResp.UserName);
        }
    }
}
