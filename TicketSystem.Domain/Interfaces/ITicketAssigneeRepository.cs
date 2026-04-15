using TicketSystem.Domain.Models;

namespace TicketSystem.Domain.Interfaces
{
    public interface ITicketAssigneeRepository
    {
        Task<IEnumerable<TicketAssignee>> GetByTicketIdAsync(int ticketId);
        Task<TicketAssignee> CreateAsync(TicketAssignee assignee);
        Task DeleteAsync(int id);
        Task DeleteByTicketIdAsync(int ticketId);
    }
}