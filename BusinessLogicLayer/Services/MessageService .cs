using Microsoft.AspNetCore.SignalR;
using ChatApplication2.BusinessLogicLayer.Interfaces;
using ChatApplication2.DataAccessLayer.Interfaces;
using ChatApplication2.DataAccessLayer.Models;
using ChatApplication2.Hubs;
using ChatApplication2.ParameterModels;
using ChatApplication2.Response_Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Message = ChatApplication2.DataAccessLayer.Models.Message;

namespace ChatApplication2.BusinessLogicLayer.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly RedisConnection _connection;

        public MessageService(IMessageRepository messageRepository, IUserRepository userRepository, IHubContext<ChatHub> hubContext,RedisConnection connection)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _hubContext = hubContext;
            _connection = connection;
        }

        public async Task<IEnumerable<MessageResponse>> GetConversationHistory(string userId, DateTime? before, int count, string sort, string currentUserEmail)
        {
            var sender = await _userRepository.FindUserByEmailAsync(currentUserEmail);
            if (sender == null)
            {
                throw new InvalidOperationException("Unauthorized");
            }

            bool isAscending = sort.ToLower() == "asc";
            var messages = await _messageRepository.GetConversationHistoryAsync(userId, sender.Id, before, count, sort.ToLower() == "asc");

            // If sort is "asc", reverse the list to show the latest messages first
            if (isAscending)
            {
                messages.Reverse();
            }

            var response = messages.Select(m => new MessageResponse
            {
                id = m.MessageID,
                userId = m.Id,
                receiverID = m.ReceiverID,
                content = m.Content,
                timestamp = m.Timestamp
            }).ToList();

            return response;
        }


        public async Task<MessageResponse> SendMessage(ReceiveMessage msg, string currentUserEmail)
        {
            var sender = await _userRepository.FindUserByEmailAsync(currentUserEmail);
            if (sender == null)
            {
                throw new InvalidOperationException("Unauthorized");
            }

            var message = new Message
            {
                Id = sender.Id,
                ReceiverID = msg.ReceiverID,
                Content = msg.Content,
                Timestamp = DateTime.Now
            };

            message = await _messageRepository.SendMessageAsync(message);
            //For Receive Message in hub
            var connectionId = await _connection.GetConnIdAsync(message.ReceiverID);
            if(connectionId !=null)
            {
                await _hubContext.Clients.Client(Convert.ToString(connectionId)).SendAsync("BroadCast", message);
            }

            //For All Client in hub
            //await _hubContext.Clients.All.Broadcast(message);

            var response = new MessageResponse
            {
                id = message.MessageID,
                content = message.Content,
                userId = sender.Id,
                receiverID = msg.ReceiverID,
                timestamp = message.Timestamp
            };

            return response;
        }

        public async Task EditMessage(int msgId, EditMessage edit, string currentUserEmail)
        {
            var sender = await _userRepository.FindUserByEmailAsync(currentUserEmail);
            if (sender == null)
            {
                throw new InvalidOperationException("Unauthorized");
            }

            var messageExists = await _messageRepository.EditMessageAsync(msgId, edit.Content);
            if (!messageExists)
            {
                throw new InvalidOperationException("Message Not Found");
            }
        }

        public async Task DeleteMessage(int msgId, string currentUserEmail)
        {
            var sender = await _userRepository.FindUserByEmailAsync(currentUserEmail);
            if (sender == null)
            {
                throw new InvalidOperationException("Unauthorized");
            }

            var messageDeleted = await _messageRepository.DeleteMessageAsync(msgId);
            if (!messageDeleted)
            {
                throw new InvalidOperationException("Message Not Found");
            }
        }

        public async Task<IEnumerable<MessageResponse>> SearchConversations(string query, string currentUserEmail)
        {
            var currentUser = await _userRepository.FindUserByEmailAsync(currentUserEmail);
            if (currentUser == null)
            {
                throw new InvalidOperationException("Unauthorized");
            }

            var conversations = await _messageRepository.SearchConversationsAsync(currentUserEmail, query);

            if (conversations == null)
            {
                throw new InvalidOperationException("No conversations found");
            }

            var response = conversations.Select(m => new MessageResponse
            {
                id = m.MessageID,
                userId = m.Id,
                receiverID = m.ReceiverID,
                content = m.Content,
                timestamp = m.Timestamp
            });

            return response;
        }

    }
}
