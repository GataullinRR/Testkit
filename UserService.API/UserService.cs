using SharedT.Types;
using System.Net.Http;
using System.Threading.Tasks;

namespace UserService.API
{
    class UserService : ServiceBase, IUserService
    {
        public UserService(HttpClient client) : base(client)
        {

        }

        public Task<GetUserInfoResponse> GetUserInfoAsync(GetUserInfoRequest request)
        {
            return MakeRequestAsync<GetUserInfoRequest, GetUserInfoResponse>(HttpMethod.Post, nameof(GetUserInfoAsync), request);
        }

        public Task<SignInResponse> SignInAsync(SignInRequest request)
        {
            return MakeRequestAsync<SignInRequest, SignInResponse>(HttpMethod.Post, nameof(SignInAsync), request);
        }

        public Task<SignUpResponse> SignUpAsync(SignUpRequest request)
        {
            return MakeRequestAsync<SignUpRequest, SignUpResponse>(HttpMethod.Post, nameof(SignUpAsync), request);
        }

        public Task<ValidateTokenResponse> ValidateTokenAsync(ValidateTokenRequest request)
        {
            return MakeRequestAsync<ValidateTokenRequest, ValidateTokenResponse>(HttpMethod.Post, nameof(ValidateTokenAsync), request);
        }
    }
}
