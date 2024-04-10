using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ChatApplication2.ParameterModels;
using ChatApplication2.Response_Models;

namespace ChatApplication2.BusinessLogicLayer.Interfaces
{
    public interface IMessageService
    {
        Task<IEnumerable<MessageResponse>> GetConversationHistory(string userId, DateTime? before, int count, string sort, string currentUserEmail);
        Task<MessageResponse> SendMessage(ReceiveMessage msg, string currentUserEmail);
        Task EditMessage(int msgId, EditMessage edit, string currentUserEmail);
        Task DeleteMessage(int msgId, string currentUserEmail);
        Task<IEnumerable<MessageResponse>> SearchConversations(string query, string currentUserEmail);
    }
}
