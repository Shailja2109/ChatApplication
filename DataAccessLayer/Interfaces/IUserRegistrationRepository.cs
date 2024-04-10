using Microsoft.AspNetCore.Identity;
using ChatApplication2.DataAccessLayer.Models;
using ChatApplication2.ParameterModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatApplication2.DataAccessLayer.Interfaces
{
     public interface IUserRegistrationRepository
    {
        Task<IEnumerable<IdentityUser>> GetAllUsersAsync(string currentUserEmail);
        Task<IdentityUser> GetUserByIdAsync(string id);
        Task<IdentityUser> FindUserByEmailAsync(string email);
        Task<bool> AddUserAsync(IdentityUser user,string password);

    }
}
