using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using AutoMapper;
using UserService.API;
using Grpc.Net.Client;
using Grpc.Core;
using Grpc.Net.Client.Web;
using Microsoft.JSInterop;
using Utilities.Types;
using System.Collections.ObjectModel;
using System.Reflection;
using Utilities.Extensions;
using Microsoft.AspNetCore.Components;

namespace Runner
{
    public class Message
    {
        public string Text { get; set; }
    }

    public interface IMessageService
    {
        ObservableCollection<Message> Messages { get; }
    }

    [Service(ServiceLifetime.Scoped)]
    public class MessageService : IMessageService
    {
        public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>();
    }

    public interface IIdentityContext
    {
        Identity Identity { get; }
    }

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

    public interface IUserChangeNotifier
    {
        event Func<Task> ProfileChangedAsync;
        event Func<Task> AuthStateChangedAsync;

        Task FireProfileChangedAsync();
        Task FireAuthStateChangedAsync();
    }

    [Service(ServiceLifetime.Singleton)]
    class UserChangeNotifier : IUserChangeNotifier
    {
        public event Func<Task> ProfileChangedAsync = () => Task.CompletedTask;
        public event Func<Task> AuthStateChangedAsync = () => Task.CompletedTask;

        public async Task FireAuthStateChangedAsync()
        {
            await AuthStateChangedAsync.InvokeAndWaitAsync();
        }

        public async Task FireProfileChangedAsync()
        {
            await ProfileChangedAsync.InvokeAndWaitAsync();
        }
    }

    public class Identity
    {
        public bool IsAuthentificated { get; }
        public UserInfo? User { get; }

        public Identity() : this(false, null)
        {
        
        }

        public Identity(bool isAuthentificated, UserInfo? user)
        {
            IsAuthentificated = isAuthentificated;
            User = user;
        }
    }

    [AutoMapFrom(typeof(GetUserInfoResponse))]
    public class UserInfo
    {
        public string UserName { get; set; }
        public string? EMail { get; set; }
        public string? Phone { get; set; }
    }

    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            var services = builder.Services;
            services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            services.AddAutoMapper(typeof(MappingProfile));
            services.AddSingleton(sp => createUserServiceClient(sp.GetRequiredService<ICookieStorage>())); 

            Assembly.GetExecutingAssembly().FindAndRegisterServicesTo(services);

            await builder.Build().RunAsync();
        }

        //[Service(ServiceLifetime.Scoped)]
        // why cant use async here?
        static UserService.API.UserService.UserServiceClient createUserServiceClient(ICookieStorage cookies)
        {
            var token = cookies.GetValueAsync(Constants.AUTH_TOKEN_COOKIE).Result;
            var credentials = CallCredentials.FromInterceptor((context, metadata) =>
            {
                if (token.IsNotNullOrEmpty())
                {
                    metadata.Add("Authorization", $"Bearer {token}");
                }
                return Task.CompletedTask;
            });

            var handler = new GrpcWebHandler(GrpcWebMode.GrpcWebText, new HttpClientHandler());
            var channel = GrpcChannel.ForAddress("https://localhost:5001/", new GrpcChannelOptions
            {
                HttpClient = new HttpClient(handler),
                Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
            });
            return new UserService.API.UserService.UserServiceClient(channel);
        }

        //GrpcChannel createAuthenticatedChannel(string address)
        //{
        //    //var test = await JSRuntime.InvokeAsync<string>("blazorExtensions.WriteCookie", name, value, days);

        //    var token = Browser.Cookies[Constants.AUTH_TOKEN_COOKIE]?.Value;
        //    var credentials = CallCredentials.FromInterceptor((context, metadata) =>
        //    {
        //        if (!string.IsNullOrEmpty(token))
        //        {
        //            metadata.Add("Authorization", $"Bearer {token}");
        //        }
        //        return Task.CompletedTask;
        //    });

        //    // SslCredentials is used here because this channel is using TLS.
        //    // CallCredentials can't be used with ChannelCredentials.Insecure on non-TLS channels.
        //    return GrpcChannel.ForAddress(address, new GrpcChannelOptions
        //    {
        //        Credentials = ChannelCredentials.Create(new SslCredentials(), credentials)
        //    });
        //}
    }
}
