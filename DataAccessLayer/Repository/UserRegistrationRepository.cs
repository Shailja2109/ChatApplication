using ChatApplication2.DataAccessLayer.Data;
using ChatApplication2.DataAccessLayer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ChatApplication2.ParameterModels;
using Microsoft.AspNetCore.Http.HttpResults;
using Azure;
using System.Security.Claims;
using ChatApplication2.DataAccessLayer.Interfaces;

namespace ChatApplication2.DataAccessLayer.Repository
{
    public class UserRegistrationRepository : IUserRegistrationRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpcontextAccessor;

        public UserRegistrationRepository(AppDbContext context, UserManager<IdentityUser> userManager, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _userManager = userManager;
            _httpcontextAccessor = httpContextAccessor;
        }

        public async Task<IEnumerable<IdentityUser>> GetAllUsersAsync(string currentUserEmail)
        {
            return await _userManager.Users.Where(u => u.Email != currentUserEmail).ToListAsync();
        }

        public async Task<IdentityUser> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<IdentityUser> FindUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<bool> AddUserAsync(IdentityUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            return result.Succeeded;
        }

    }
}

