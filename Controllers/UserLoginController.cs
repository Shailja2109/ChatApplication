// PresentationLayer/Controllers/UserLoginController.cs

using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ChatApplication2.BusinessLogicLayer.Interfaces;
using ChatApplication2.DataAccessLayer.Interfaces;
using ChatApplication2.DataAccessLayer.Models;
//using ChatApplication2.Helper;
using ChatApplication2.ParameterModels;
using ChatApplication2.Response_Models;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace ChatApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserLoginController : ControllerBase
    {
        
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserService _userService;

        public UserLoginController(IUserRepository userRepository, IConfiguration configuration, UserManager<IdentityUser> userManager,IUserService userService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _userManager = userManager;
            _userService = userService;
            
        }

        [HttpPost("Login")]
        public async Task<IActionResult> UserLogin([FromBody] UserLogin userLogin)
        {
            //Checking the user
            var user = await _userRepository.FindUserByEmailAsync(userLogin.Email);
            if (user == null)
            {
                return NotFound("User Not Found");
            }

            //check Password
            if (!await _userRepository.CheckPasswordAsync(user, userLogin.Password))
            {
                return BadRequest("Password is Incorrect");
            }

            //Generate the token
            string token = CreateToken(user);

            //return the token
            return Ok(new { token = token });
        }
        //[HttpPost("ExternalLogin")]
        //public async Task<IActionResult> GetOrCreateExternalLoginUser([FromBody] ExternalAuthDto externalAuth)
        //{
        //    Payload payload = await ValidateAsync(externalAuth.IdToken, new ValidationSettings
        //    {
        //        Audience = new[] { _configuration["Authentication:Google:ClientId"] }
        //    });

        //    if (payload == null)
        //        return BadRequest("Invalid External Authentication.");

        //    var info = new UserLoginInfo(payload.Subject, payload.Name, null); // Using the correct constructor with default ProviderDisplayName

        //    var user = await _userManager.FindByLoginAsync(payload.Subject, payload.Name);

        //    if (user == null)
        //    {
        //        user = await _userManager.FindByEmailAsync(payload.Email);
        //        if (user == null)
        //        {
        //            user = new IdentityUser
        //            {
        //                Email = payload.Email,
        //                UserName = payload.Name
        //            };
        //            await _userManager.CreateAsync(user);
        //        }

        //        await _userManager.AddLoginAsync(user, info);
        //    }

        //    if (user == null)
        //        return BadRequest("Invalid External Authentication.");

        //    string token = CreateToken(user);

        //    var response = new AuthResponseDto
        //    {
        //        Token = token,
        //        IsAuthSuccessful = true
        //    };

        //    return Ok(response);
        //}

        [HttpPost("GoogleAuthenticate")]
        public async Task<IActionResult> GoogleAuthenticate([FromBody] ExternalAuthDto request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState.Values.SelectMany(it => it.Errors).Select(it => it.ErrorMessage));
            var token = CreateToken(await _userService.AuthenticateGoogleUserAsync(request));
            return Ok(new { token = token });
        }

        private string CreateToken(IdentityUser user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                 //new Claim("UserId", user.UserID.ToString())
                //new Claim(ClaimTypes.Role,"User")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var token = new JwtSecurityToken
                (
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;

        }
    }
}




