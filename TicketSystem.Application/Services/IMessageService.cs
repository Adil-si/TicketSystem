using System;
using System.Collections.Generic;
using System.Text;
using TicketSystem.Domain.Models;

namespace TicketSystem.Application.Services
{
   public interface IMessageService
    {
        Task<IEnumerable<Message>> GetSentMessagesAsync(string userId);
        Task<IEnumerable<Message>> GetReceivedMessagesAsync(string userId);
        Task<IEnumerable<Message>> GetConversationAsync(string userId1, string userId2);
        Task<Message> SendMessageAsync (string senderId, string receiverId, string content);

        Task MarkAsReadAsync (int massageId);
        Task<int> GetUnreadCountAsync (string userId);
        Task<List<ApplicationUser>> GetUsersForMessagingAsync (string currentUserId);
        Task<List<MessageGroupDto>> GetGroupedMessagesAsync(string userId);
        Task<Message> SendMessageAsync(Message message);
        Task DeleteMessageAsync(int id);
    }
    public class MessageGroupDto
    {
        public string OtherUserId { get; set; } = string.Empty;
        public string OtherUserName { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
        public DateTime LastMessageTime { get; set; }
        public int UnreadCount { get; set; }
        public List<Message> Messages { get; set; } = new();
    }
}
