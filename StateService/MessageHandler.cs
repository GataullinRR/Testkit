using MessageHub;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StateService.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Types;

namespace StateService
{
    public class MessageHandler : IHostedService
    {
        [Inject] public IMessageConsumer MessageConsumer { get; set; }
        [Inject] public IServiceScopeFactory ScopeFactory { get; set; }

        public MessageHandler(IDependencyResolver di, IHostApplicationLifetime fdfd)
        {
            di.ResolveProperties(this); ;

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            MessageConsumer.BeginAddTestAsync += MessageConsumer_BeginAddTestAsync;
            MessageConsumer.StopAddTestAsync += MessageConsumer_StopAddTestAsync;

            await Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

        }

        async Task MessageConsumer_BeginAddTestAsync(BeginAddTestMessage arg)
        {
            using var scope = ScopeFactory.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<StateContext>();

            var state = await db.States.FirstOrDefaultAsync(s => s.UserName == arg.UserName);
            if (state == null)
            {
                state = new StateInfo() 
                { 
                    UserName = arg.UserName 
                };

                await db.States.AddAsync(state);
            }

            state.State = UserState.AddingTests;
            await db.SaveChangesAsync();
        }

        async Task MessageConsumer_StopAddTestAsync(StopAddTestMessage arg)
        {
            using var scope = ScopeFactory.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<StateContext>();

            var state = await db.States.FirstOrDefaultAsync(s => s.UserName == arg.UserName);
            if (state == null)
            {
                state = new StateInfo()
                {
                    UserName = arg.UserName
                };

                await db.States.AddAsync(state);
            }

            state.State = UserState.Default;
            await db.SaveChangesAsync();
        }
    }
}
