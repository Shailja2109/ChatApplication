using Microsoft.AspNetCore.Identity;
using ChatApplication2.BusinessLogicLayer.Interfaces;
using ChatApplication2.DataAccessLayer.Interfaces;
using ChatApplication2.ParameterModels;
using static Google.Apis.Auth.GoogleJsonWebSignature;

namespace ChatApplication2.BusinessLogicLayer.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        public UserService(UserManager<IdentityUser> userManager, IConfiguration configuration,IUserRepository userRepository)
        {
            _userManager = userManager;
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public async Task<IdentityUser> AuthenticateGoogleUserAsync(ExternalAuthDto request)
        {
            Payload payload = await ValidateAsync(request.IdToken, new ValidationSettings
            {
                Audience = new[] { _configuration["Authentication:Google:ClientId"] }
            });

            return await GetOrCreateExternalLoginUser(ExternalAuthDto.PROVIDER, payload.Subject, payload.Email, payload.GivenName, payload.FamilyName);
        }


        private async Task<IdentityUser> GetOrCreateExternalLoginUser(string provider, string key, string email, string firstName, string lastName)
        {
            var user = await _userManager.FindByLoginAsync(provider, key);


            if (user != null)
                return user;

            user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                // If the email is not found, try to create the user with the provided firstName as the username
                user = new IdentityUser
                {
                    Email = email,
                    UserName = firstName,
                    Id = key,
                };

            }
            var userName = await _userManager.FindByNameAsync(firstName);
            if (userName != null)
            {
                // If the email exists and the username (firstName) is also taken, generate a unique username
                string newUserName = firstName;
                int count = 1;
                while (userName != null)
                {
                    newUserName = $"{firstName}{count:D2}"; // Appending a unique number to the username
                    userName = await _userManager.FindByNameAsync(newUserName);
                    count++;
                }
                user.UserName = newUserName;
                await _userManager.UpdateAsync(user);
            }
            await _userManager.CreateAsync(user);
            var info = new UserLoginInfo(provider, key, provider.ToUpperInvariant());
            var result = await _userManager.AddLoginAsync(user, info);

            if (result.Succeeded)
                return user;

            return null;
        }

    }

}