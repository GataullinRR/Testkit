using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using SharedT.Types;
using Utilities.Types;
using Microsoft.AspNetCore.Components;

namespace Runner
{
    [Service(ServiceLifetime.Singleton)]
    class TokenProvider : ITokenProvider
    {
        [Inject] public ICookieStorage Cookies { get; set; }

        public TokenProvider(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        public async Task<string?> GetTokenAsync()
        {
            return await Cookies.GetValueAsync(Constants.AUTH_TOKEN_COOKIE);
        }
    }
}
