using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.Configuration;
using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using UserService.API;
using UserServiceDb;
using Utilities.Extensions;
using Utilities.Types;
using Protobuf;

namespace UserService.Endpoints
{
    public class UserServiceEndpoints : API.UserService.UserServiceBase
    {
        [Inject] public SignInManager<User> SignInManager { get; set; }
        [Inject] public UserManager<User> UserManager { get; set; }
        [Inject] public IMapper Mapper { get; set; }
        [Inject] public Microsoft.Extensions.Configuration.IConfiguration Configuration { get; set; } 

        public UserServiceEndpoints(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        public override async Task<SignInResponse> SignIn(SignInRequest request, ServerCallContext context)
        {
            var response = new SignInResponse()
            {
                Status = new ResponseStatus()
            };

            var result = await SignInManager.PasswordSignInAsync(request.UserName, request.Password, true, false);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByNameAsync(request.UserName);
                response.Token = generateJwtToken(user);
                response.Status.Code = Protobuf.StatusCode.Ok;
            }
            else
            {
                response.Status.Code = Protobuf.StatusCode.Error;
                response.Status.Description = "Could not authorize. Wrong credentials or the user does not exist";
            }

            return response;

            string generateJwtToken(IdentityUser user)
            {
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.UserName)
                };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"]));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var expires = DateTime.Now.AddDays(Convert.ToDouble(Configuration["JWT:ExpireDays"]));

                var token = new JwtSecurityToken(
                    Configuration["JWT:Issuer"],
                    Configuration["JWT:Issuer"],
                    claims,
                    expires: expires,
                    signingCredentials: creds
                );

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        }

        public override async Task<SignUpResponse> SignUp(SignUpRequest request, ServerCallContext context)
        {
            var response = new SignUpResponse();

            var user = new User
            {
                UserName = request.UserName,
                Email = request.EMail,
                PhoneNumber = request.Phone
            };
            var result = await UserManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                await UserManager.AddToRoleAsync(user, Roles.User);
                response.Status = new ResponseStatus()
                {
                    Code = Protobuf.StatusCode.Ok,
                    Description = "Done!"
                };
            }
            else
            {
                response.Status = new ResponseStatus()
                {
                    Code = Protobuf.StatusCode.Error,
                    Description = "Could not complete registration. Maybe the user with this name already exists."
                };
            }

            return response;
        }

        public override async Task<GetUserInfoResponse> GetUserInfo(GetUserInfoRequest request, ServerCallContext context)
        {
            var response = new GetUserInfoResponse()
            {
                Status = new ResponseStatus()
            };

            var currentUser = await tryGetTokenOwnerAsync(request.Token);
            if (request.UserName.IsNullOrEmpty()) // get current
            {
                if (currentUser == null)
                {
                    response.Status.Code = Protobuf.StatusCode.Error;
                    response.Status.Description = "Authentification required!";
                }
                else
                {
                    response.UserName = currentUser.UserName;
                    response.EMail = currentUser.Email;
                    response.Phone = currentUser.PhoneNumber;
                }
            }
            else
            {
                var targetUser = await UserManager.FindByNameAsync(request.UserName);
                if (targetUser == null)
                {
                    response.Status.Code = Protobuf.StatusCode.Error;
                    response.Status.Description = "User not found";
                }
                else
                {
                    if (currentUser == targetUser)
                    {
                        response.UserName = targetUser.UserName;
                        response.EMail = targetUser.Email;
                        response.Phone = targetUser.PhoneNumber;
                    }
                    else if (currentUser != null) // authorized
                    {
                        response.UserName = targetUser.UserName;
                        response.EMail = targetUser.Email;
                    }
                    else // anon
                    {
                        response.UserName = targetUser.UserName;
                    }
                }
            }

            return response;
        }

        public override async Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest request, ServerCallContext context)
        {
            var response = new ValidateTokenResponse()
            {
                Valid = validateToken(request.Token)
            };

            return response;
        }

        async Task<User?> tryGetTokenOwnerAsync(string token)
        {
            if (token.IsNotNullOrEmpty() && validateToken(token))
            {
                var tokenOwnerName = extractUserName(token);
                return await UserManager.FindByNameAsync(tokenOwnerName);
            }
            else
            {
                return null;
            }

            string extractUserName(string jwtToken)
            {
#warning smth wrong
                var handler = new JwtSecurityTokenHandler();
                var tokenObject = handler.ReadToken(jwtToken) as JwtSecurityToken;
                var userName = tokenObject.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value;

                return userName;
            }
        }

        bool validateToken(string authToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = GetValidationParameters();
            try
            {
                tokenHandler.ValidateToken(authToken, validationParameters, out var validatedToken);

                return true;
            }
            catch (SecurityTokenException)
            {
                return false;
            }
        }

        TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateLifetime = true, 
                ValidateAudience = true,
                ValidateIssuer = true, 
                ValidIssuer = Configuration["JWT:Issuer"],
                ValidAudience = Configuration["JWT:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"])) // The same key as the one that generate the token
            };
        }
    }
}
