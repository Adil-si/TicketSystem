using Microsoft.EntityFrameworkCore;
using TicketSystem.Domain.Interfaces;
using TicketSystem.Domain.Models;
using TicketSystem.Infrastructure.Data;

namespace TicketSystem.Infrastructure.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AppDbContext _context;

        public MessageRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Message>> GetSentMessagesAsync(string userId)
        {
            return await _context.Messages
                .Include(m => m.Receiver)
                .Include(m => m.Sender)
                .Where(m => m.SenderId == userId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetReceivedMessagesAsync(string userId)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => m.ReceiverId == userId)
                .OrderByDescending(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Message>> GetConversationAsync(string userId1, string userId2)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Where(m => (m.SenderId == userId1 && m.ReceiverId == userId2) ||
                           (m.SenderId == userId2 && m.ReceiverId == userId1))
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task<Message> CreateAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<Message?> GetByIdAsync(int id)
        {
            return await _context.Messages
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public async Task MarkAsReadAsync(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null && !message.IsRead)
            {
                message.IsRead = true;
                message.ReadAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> GetUnreadCountAsync(string userId)
        {
            return await _context.Messages
                .CountAsync(m => m.ReceiverId == userId && !m.IsRead);
        }

        public async Task DeleteAsync(int id)
        {
            var message = await _context.Messages.FindAsync(id);
            if (message != null)
            {
                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();
            }
        }
    }
}