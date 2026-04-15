using TicketSystem.Domain.Models;

namespace TicketSystem.Domain.Interfaces
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetByTicketIdAsync(int ticketId);
        Task<Comment> CreateAsync(Comment comment);
        Task DeleteAsync(int id);
    }
}