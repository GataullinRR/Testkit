using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Types;
using Microsoft.AspNetCore.Components;
using PresentationService.API;

namespace Runner
{
    [Service(ServiceLifetime.Singleton, RegisterAsPolicy.Self)]
    class WebMessagingBinder
    {
        [Inject] public ICookieStorage Cookies { get; set; }
        [Inject] public IUserChangeNotifier UserChangeNotifier { get; set; }
        [Inject] public IWebMessageHubConnectionProvider WebMessageHubConnectionProvider { get; set; }

        public WebMessagingBinder(IDependencyResolver di)
        {
            di.ResolveProperties(this);

            UserChangeNotifier.AuthStateChangedAsync += UserChangeNotifier_AuthStateChangedAsync;
        }

        async Task UserChangeNotifier_AuthStateChangedAsync()
        {
            var token = await Cookies.GetValueAsync(Constants.AUTH_TOKEN_COOKIE);

            await WebMessageHubConnectionProvider.BindUserAsync(token);
        }
    }
}
