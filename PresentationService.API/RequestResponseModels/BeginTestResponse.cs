using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Shared.Types;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Types;

namespace PresentationService.API
{
    public class BeginTestResponse : ResponseBase
    {
        public static implicit operator global::PresentationService.API2.GRunTestResponse(BeginTestResponse response)
        {
            return new global::PresentationService.API2.GRunTestResponse()
            {
                Status = response.Status
            };
        }
        public static implicit operator BeginTestResponse(global::PresentationService.API2.GRunTestResponse response)
        {
            return new BeginTestResponse(response.Status);
        }

        public BeginTestResponse(ResponseStatus status) : base (status)
        {

        }
    }
}
