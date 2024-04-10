using Microsoft.AspNetCore.Identity;
using ChatApplication2.ParameterModels;

namespace ChatApplication2.BusinessLogicLayer.Interfaces
{
    public interface IUserService
    {
          Task<IdentityUser> AuthenticateGoogleUserAsync(ExternalAuthDto request);
    }
}
