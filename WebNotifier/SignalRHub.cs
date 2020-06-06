using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserService.API;
using Utilities.Types;

namespace WebNotificationService
{
    public class SignalRHub : Hub<IMainHub>
    {
        [Inject] public IUserService UserService { get; set; }

        public SignalRHub(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        public async Task Bind(string authToken)
        {
            var userName = "";
            if (authToken != null)
            {
                var uInfReq = new GetUserInfoRequest(authToken);
                var uInfResp = await UserService.GetUserInfoAsync(uInfReq);
                userName = uInfResp.UserName;
            }

            await Groups.AddToGroupAsync(Context.ConnectionId, userName);
        }
    }
}
