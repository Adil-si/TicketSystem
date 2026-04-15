using TicketSystem.Domain.Models;

namespace TicketSystem.Application.Services
{
    public interface ITicketAssigneeService
    {
        Task<IEnumerable<TicketAssignee>> GetAssigneesByTicketIdAsync(int ticketId);
        Task<TicketAssignee> AddAssigneeAsync(int ticketId, string userId);
        Task RemoveAssigneeAsync(int assigneeId);
        Task<bool> IsUserAssignedAsync(int ticketId, string userId);
    }
}