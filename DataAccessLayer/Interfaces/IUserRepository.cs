
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ChatApplication2.DataAccessLayer.Models;


namespace ChatApplication2.DataAccessLayer.Interfaces
{
    public interface IUserRepository
    {
        Task<IdentityUser> FindUserByEmailAsync(string email);
        Task<bool> CheckPasswordAsync(IdentityUser user, string password);
    }
}
