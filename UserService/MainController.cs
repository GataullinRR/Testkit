using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
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

namespace UserService
{
    [ApiController, Microsoft.AspNetCore.Mvc.Route("api/v1")]
    public class MainController : ControllerBase
    {
        [Inject] public SignInManager<User> SignInManager { get; set; }
        [Inject] public UserManager<User> UserManager { get; set; }
        [Inject] public Microsoft.Extensions.Configuration.IConfiguration Configuration { get; set; } 

        public MainController(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        [HttpPost, Microsoft.AspNetCore.Mvc.Route(nameof(IUserService.SignInAsync))]
        public async Task<IActionResult> SignIn(SignInRequest request)
        {
            var result = await SignInManager.PasswordSignInAsync(request.UserName, request.Password, true, false);
            if (result.Succeeded)
            {
                var user = await UserManager.FindByNameAsync(request.UserName);

                return Ok(new SignInResponse(generateJwtToken(user)));
            }
            else
            {
                return NotFound("Could not authorize. Wrong credentials or the user does not exist");
            }

            string generateJwtToken(IdentityUser user)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
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

        [HttpPost, Microsoft.AspNetCore.Mvc.Route(nameof(IUserService.SignUpAsync))]
        public async Task<IActionResult> SignUp(SignUpRequest request)
        {
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

                return Ok(new SignUpResponse());
            }
            else
            {
                return UnprocessableEntity("Could not complete registration. Maybe the user with this name already exists.");
            }
        }

        [HttpPost, Microsoft.AspNetCore.Mvc.Route(nameof(IUserService.GetUserInfoAsync))]
        public async Task<IActionResult> GetUserInfo(GetUserInfoRequest request)
        {
            var currentUser = await tryGetTokenOwnerAsync(request.Token);
            if (request.UserName.IsNullOrEmpty()) // get current
            {
                if (currentUser == null)
                {
                    return Unauthorized();
                }
                else
                {
                    return Ok(new GetUserInfoResponse(currentUser.UserName, currentUser.Email, currentUser.PhoneNumber));
                }
            }
            else
            {
                var targetUser = await UserManager.FindByNameAsync(request.UserName);
                if (targetUser == null)
                {
                    return NotFound("User not found");
                }
                else
                {
                    if (currentUser == targetUser)
                    {
                        return Ok(new GetUserInfoResponse(targetUser.UserName, targetUser.Email, targetUser.PhoneNumber));
                    }
                    else if (currentUser != null) // authorized
                    {
                        return Ok(new GetUserInfoResponse(targetUser.UserName, targetUser.Email, null));
                    }
                    else // anon
                    {
                        return Ok(new GetUserInfoResponse(targetUser.UserName, null, null));
                    }
                }
            }
        }

        [HttpPost, Microsoft.AspNetCore.Mvc.Route(nameof(IUserService.ValidateTokenAsync))]
        public async Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest request)
        {
            return new ValidateTokenResponse(validateToken(request.Token));
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
            if (authToken.IsNullOrEmpty())
            {
                return false;
            }
            else
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
