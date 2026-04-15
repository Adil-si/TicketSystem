using TicketSystem.Domain.Models;

namespace TicketSystem.Application.Services
{
    public interface IBlockedByService
    {
        Task<IEnumerable<BlockedBy>> GetBlockingTicketsAsync(int ticketId);
        Task<IEnumerable<BlockedBy>> GetBlockedByTicketsAsync(int ticketId);
        Task<BlockedBy> AddBlockAsync(int ticketId, int blockedByTicketId);
        Task RemoveBlockAsync(int id);
        Task<bool> CanCloseTicketAsync(int ticketId);
        Task<List<Ticket>> GetAvailableTicketsForBlockingAsync(int ticketId);
    }
}