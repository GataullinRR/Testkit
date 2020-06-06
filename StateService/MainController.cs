using Grpc.Core;
using Microsoft.AspNetCore.Components;
using PresentationService.API;
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
using Microsoft.AspNetCore.Mvc;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;
using GetTestsAddStateResponse = StateService.API.GetTestsAddStateResponse;
using GetTestsAddStateRequest = StateService.API.GetTestsAddStateRequest;

namespace StateService
{
    [ApiController, Route("api/v1")]
    public class MainController : ControllerBase
    {
        [Inject] public StateContext Db { get; set; }

        public MainController(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        [HttpPost, Route(nameof(IStateService.GetTestsAddStateAsync))]
        public async Task<GetTestsAddStateResponse> GetTestsAddState(GetTestsAddStateRequest request)
        { 
            var state = await Db.States.FirstOrDefaultAsync(s => s.UserName == request.UserName) 
                ?? new StateInfo() { UserName = request.UserName };

            return new GetTestsAddStateResponse(state.State == UserState.AddingTests);
        }
    }
}
