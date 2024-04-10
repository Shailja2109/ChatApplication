using ChatApplication2.DataAccessLayer.Data;
using Microsoft.EntityFrameworkCore;
using ChatApplication2.DataAccessLayer.Interfaces;
using Message = ChatApplication2.DataAccessLayer.Models.Message;
using Microsoft.AspNetCore.Identity;

namespace ChatApplication2.DataAccessLayer.Repository
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MessageRepository(AppDbContext context, UserManager<IdentityUser> userManage)
        {
            _context = context;
            _userManager = userManage;
        }

        public async Task<List<Message>> GetConversationHistoryAsync(string userId, string receiverId, DateTime? before, int count, bool isAscending)
        {

            var messages = await _context.Messages
                .Where(m => m.Id == userId && m.ReceiverID == receiverId || m.Id == receiverId && m.ReceiverID == userId)
                .Where(m => before == null || (isAscending ? m.Timestamp < before : m.Timestamp > before))
                .OrderByDescending(m => m.Timestamp)
                .Take(count)
                .ToListAsync();

            return messages;
        }

        public async Task<Message> SendMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<bool> EditMessageAsync(int messageId, string content)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null)
                return false;

            message.Content = content;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteMessageAsync(int messageId)
        {
            var message = await _context.Messages.FindAsync(messageId);
            if (message == null)
                return false;

            _context.Messages.Remove(message);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<Message>> SearchConversationsAsync(string currentUserEmail, string query)
        {
            var currentUser = await FindUserByEmailAsync(currentUserEmail);
            if (currentUser == null)
            {
                return null;
            }

            var keywords = query.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            //Fetch all messages of the current user
            var users = await _context.Messages
                .Where(m => (m.Id == currentUser.Id || m.ReceiverID == currentUser.Id))
                .ToListAsync();

            // Filter messages containing the provided keywords
            var conversation = users
                .Where(m => keywords.Any(keyword => m.Content.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                .OrderBy(m => m.Timestamp)
                .ToList();
            return conversation;
        }
        public async Task<IdentityUser> FindUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

    }
}
