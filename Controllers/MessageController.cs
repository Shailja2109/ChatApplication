using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChatApplication2.DataAccessLayer.Interfaces;
using ChatApplication2.DataAccessLayer.Models;
using ChatApplication2.Response_Models;
using System.Security.Claims;
using ChatApplication2.ParameterModels;
using Microsoft.AspNetCore.SignalR;
using ChatApplication2.Hubs;
using ChatApplication2.BusinessLogicLayer.Interfaces;

namespace ChatApplication2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [HttpGet, Authorize]
        public async Task<IActionResult> GetConversationHistory(string userId, DateTime? before = null, int count = 20, string sort = "asc")
        {
            string currentEmail = GetCurrentEmail();
            var messages = await _messageService.GetConversationHistory(userId, before, count, sort, currentEmail);
            return Ok(messages);
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> SendMessage([FromBody] ReceiveMessage msg)
        {
            string currentEmail = GetCurrentEmail();
            var response = await _messageService.SendMessage(msg, currentEmail);
            return Ok(response);
        }

        [HttpPut("{msgId}"), Authorize]
        public async Task<IActionResult> MessageEdit(int msgId, [FromBody] EditMessage edit)
        {
            string currentEmail = GetCurrentEmail();
            await _messageService.EditMessage(msgId, edit, currentEmail);
            return Ok();
        }

        [HttpDelete("{msgId}"), Authorize]
        public async Task<IActionResult> DeleteMessage(int msgId)
        {
            string currentEmail = GetCurrentEmail();
            await _messageService.DeleteMessage(msgId, currentEmail);
            return Ok();
        }

        [HttpGet("search")]
        [Authorize]
        public async Task<IActionResult> SearchConversations([FromQuery] string query)
        {
            string currentEmail = GetCurrentEmail();
            var conversations = await _messageService.SearchConversations(query, currentEmail);
            return Ok(new { messages = conversations });
        }

        private string GetCurrentEmail()
        {
            // Get the current user's claims principal
            var claimsPrincipal = HttpContext.User;

            // Get the email claim
            var emailClaim = claimsPrincipal.FindFirst(ClaimTypes.Email);

            return emailClaim?.Value;
        }
    }
}


