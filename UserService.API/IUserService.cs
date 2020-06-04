using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace UserService.API
{
    public interface IUserService
    {
        Task<SignInResponse> SignInAsync(SignInRequest request);
        Task<SignUpResponse> SignUpAsync(SignUpRequest request);
        Task<GetUserInfoResponse> GetUserInfoAsync(GetUserInfoRequest request);
        Task<ValidateTokenResponse> ValidateTokenAsync(ValidateTokenRequest request);
    }
}
