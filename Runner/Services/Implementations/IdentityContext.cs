using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using UserService.API;
using Utilities.Types;

namespace Runner
{
    [Service(ServiceLifetime.Singleton)]
    class IdentityContext : IIdentityContext
    {
        readonly UserService.API.UserService.UserServiceClient _userService;
        readonly ICookieStorage _cookies;
        readonly IMapper _mapper;

        public Identity Identity { get; private set; } = new Identity();

        public IdentityContext(UserService.API.UserService.UserServiceClient userService, 
            ICookieStorage cookies, 
            IMapper mapper, 
            IUserChangeNotifier userChangeNotifier)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _cookies = cookies ?? throw new ArgumentNullException(nameof(cookies));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));

            userChangeNotifier.ProfileChangedAsync += reloadIdentityAsync;
            userChangeNotifier.AuthStateChangedAsync += reloadIdentityAsync;
        }

        async Task reloadIdentityAsync()
        {
            var token = await _cookies.GetValueAsync(Constants.AUTH_TOKEN_COOKIE);
            if (token == null)
            {
                Identity = new Identity();
            }
            else
            {
                var request = new GetUserInfoRequest()
                {
                    Token = token
                };

                var response = await _userService.GetUserInfoAsync(request);
                if (response.Status.Code == UserService.API.StatusCode.Ok)
                {
                    var user = _mapper.Map<UserInfo>(response);
                    Identity = new Identity(true, user);
                }
            }
        }
    }
}
