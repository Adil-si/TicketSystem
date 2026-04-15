using System;
using System.Collections.Generic;
using System.Text;
using TicketSystem.Domain.Models;

namespace TicketSystem.Domain.Interfaces
{
  public interface IMessageRepository
    {
        Task<IEnumerable<Message>> GetSentMessagesAsync(string userId);
        Task<IEnumerable<Message>> GetReceivedMessagesAsync(string userId);
        Task<IEnumerable<Message>> GetConversationAsync(string userId, string userId2);
        Task<Message> CreateAsync (Message message);
        Task<Message?> GetByIdAsync(int id);
        Task MarkAsReadAsync(int id);
        Task <int> GetUnreadCountAsync(string userId);
        Task DeleteAsync(int id);
    }
}
