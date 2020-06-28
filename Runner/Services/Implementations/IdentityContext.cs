using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using UserService.API;
using Utilities.Types;
using Microsoft.AspNetCore.Components;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;

namespace Runner
{
    [Service(ServiceLifetime.Singleton)]
    class IdentityContext : IIdentityContext
    {
        [Inject] public IUserService UserService { get; set; }
        [Inject] public ICookieStorage Cookies { get; set; }
        [Inject] public Browser Browser { get; set; }
        [Inject] public IUserChangeNotifier UserChangeNotifier { get; set; }

        public Identity Identity { get; private set; } = new Identity();

        public IdentityContext(IDependencyResolver di)
        {
            di.ResolveProperties(this);

            UserChangeNotifier.ProfileChangedAsync += LoadAsync;
            UserChangeNotifier.AuthStateChangedAsync += LoadAsync;

            //var jwt = Cookies.Get(Constants.AUTH_TOKEN_COOKIE);
            //if (jwt != null)
            //{
            //    var handler = new JwtSecurityTokenHandler();
            //    var token = handler.ReadJwtToken(jwt);
            //    var u = new GetUserInfoResponse(
            //        token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value,
            //        token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email).Value,
            //        token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone).Value);
            //    Identity = new Identity(true, u);
            //}
        }

        public async Task LoadAsync()
        {
            await Browser.LogToConsoleAsync("Reloading identity...");

            var token = await Cookies.GetValueAsync(Constants.AUTH_TOKEN_COOKIE);
            if (token == null)
            {
                Identity = new Identity();
            }
            else
            {
                var request = new GetUserInfoRequest(token);
                var response = await UserService.GetUserInfoAsync(request);
                Identity = new Identity(true, response);
            }

            await UserChangeNotifier.FireIdentityChangedAsync();
        }
    }
}
