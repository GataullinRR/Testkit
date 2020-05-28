using Grpc.Core;
using Microsoft.AspNetCore.Components;
using PresentationService.API;
using Protobuf;
using Shared;
using StateService.Db;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Types;
using Utilities.Extensions;
using StateService.API;
using Microsoft.EntityFrameworkCore;

namespace StateService
{
    [GrpcService]
    public class GrpcService : API.StateService.StateServiceBase
    {
        [Inject] public StateContext Db { get; set; }

        public GrpcService(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        public override async Task<GGetTestsAddStateResponse> GetTestsAddState(GGetTestsAddStateRequest gRequest, ServerCallContext context)
        {
            var response = new GGetTestsAddStateResponse()
            {
                Status = new GResponseStatus(),
            };

            var state = await Db.States.FirstOrDefaultAsync(s => s.UserName == gRequest.UserName) 
                ?? new StateInfo() { UserName = gRequest.UserName };
            response.HasBegan = state.State == UserState.AddingTests;

            return response;
        }
    }
}
