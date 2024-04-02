using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ConversationContext _context;
        private readonly string _secretKey;
        private readonly string _issuer;
        IConfiguration _configuration;

        public UsersController(ConversationContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _secretKey = configuration["Jwt:SecretKey"];
            _issuer = configuration["Jwt:Issuer"];
        }

        // GET: api/Users
        [HttpGet, Authorize]
        [Route("/API/Users")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            var authorizationHeader = HttpContext.Request.Headers["Authorization"];
            if (authorizationHeader.Count == 0)
            {
                return Unauthorized();
            }
            _ = authorizationHeader.FirstOrDefault().Split(' ').LastOrDefault();
            _ = User.FindFirst(ClaimTypes.Email)?.Value;
            return await _context.Users.ToListAsync();
        }

        // POST: api/Registration
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [AllowAnonymous]
        [HttpPost]
        [Route("/API/Users/Registration")]
        public async Task<ActionResult<User>> PostUser(UserDTO user)
        {
            var userDuplicate = await _context.Users.FirstOrDefaultAsync(e => e.Email == user.Email);

            if (userDuplicate != null)
            {
                return Conflict();
            }
            _context.Users.Add(new Models.User()
            {
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                UserId = Guid.NewGuid()
            });
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserExists(user.Name))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(user);
        }

        private bool UserExists(string name)
        {
            return _context.Users.Any(e => e.Name == name);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("/API/Users/Login")]
        public async Task<ActionResult<Object>> Auth([FromBody] LoginDTO loginUP)
        {
            IActionResult response = Unauthorized();
            var user = await _context.Users.FirstOrDefaultAsync(e => e.Email == loginUP.Email);

            if (user != null)
            {
                if (user.Password == loginUP.Password)
                {
                    var issuer = _configuration["Jwt:Issuer"];
                    var audience = _configuration["Jwt:Audience"];
                    var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
                    var signingCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha512Signature
                    );

                    var subject = new ClaimsIdentity(new[]
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                        new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    });

                    var expires = DateTime.UtcNow.AddMinutes(10);

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = subject,
                        Expires = expires,
                        Issuer = issuer,
                        Audience = audience,
                        SigningCredentials = signingCredentials
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var jwtToken = tokenHandler.WriteToken(token);

                    return Ok(jwtToken);
                }
            }
            return Unauthorized();
        }
    }
}
