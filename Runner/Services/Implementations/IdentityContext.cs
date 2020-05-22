﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using UserService.API;
using Utilities.Types;
using Microsoft.AspNetCore.Components;

namespace Runner
{
    [Service(ServiceLifetime.Singleton)]
    class IdentityContext : IIdentityContext
    {
        [Inject] public UserService.API.UserService.UserServiceClient UserService { get; set; }
        [Inject] public ICookieStorage Cookies { get; set; }
        [Inject] public Browser Browser { get; set; }
        [Inject] public IMapper Mapper { get; set; }

        public Identity Identity { get; private set; } = new Identity();

        public IdentityContext(IDependencyResolver di, 
            IUserChangeNotifier userChangeNotifier)
        {
            di.ResolveProperties(this);

            userChangeNotifier.ProfileChangedAsync += reloadIdentityAsync;
            userChangeNotifier.AuthStateChangedAsync += reloadIdentityAsync;
        }

        async Task reloadIdentityAsync()
        {
            await Browser.LogToConsoleAsync("Reloading identity...");

            var token = await Cookies.GetValueAsync(Constants.AUTH_TOKEN_COOKIE);
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

                var response = await UserService.GetUserInfoAsync(request);
                if (response.Status.Code == Protobuf.StatusCode.Ok)
                {
                    var user = Mapper.Map<CSUserInfo>(response);
                    Identity = new Identity(true, user);
                }
            }
        }
    }
}