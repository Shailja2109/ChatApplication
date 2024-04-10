using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ChatApplication2.DataAccessLayer.Data;
using ChatApplication2.DataAccessLayer.Models;
using System.Security.Claims;

namespace ChatApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiLogsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ApiLogsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<ApiLog>>> GetApiLogs()
        {
            var logs = await _context.ApiLogs.ToListAsync();
            return Ok(logs);
        }

    }
}
