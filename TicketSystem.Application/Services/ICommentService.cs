using TicketSystem.Domain.Models;

namespace TicketSystem.Application.Services
{
    public interface ICommentService
    {
        Task<IEnumerable<Comment>> GetCommentsByTicketIdAsync(int ticketId);
        Task<Comment> AddCommentAsync(int ticketId, string content, string userId);
        Task DeleteCommentAsync(int commentId);
    }
}