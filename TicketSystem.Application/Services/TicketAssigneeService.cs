using TicketSystem.Domain.Interfaces;
using TicketSystem.Domain.Models;
using TicketSystem.Infrastructure.Repositories;

namespace TicketSystem.Application.Services
{
    public class TicketAssigneeService : ITicketAssigneeService
    {
        private readonly ITicketAssigneeRepository _repository;

        public TicketAssigneeService(ITicketAssigneeRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<TicketAssignee>> GetAssigneesByTicketIdAsync(int ticketId)
        {
            return await _repository.GetByTicketIdAsync(ticketId);
        }

        public async Task<TicketAssignee> AddAssigneeAsync(int ticketId, string userId)
        {
            return await _repository.CreateAsync(new TicketAssignee
            {
                TicketId = ticketId,
                UserId = userId,
                AssignedAt = DateTime.UtcNow
            });
        }

        public async Task RemoveAssigneeAsync(int assigneeId)
        {
            await _repository.DeleteAsync(assigneeId);
        }

        public async Task<bool> IsUserAssignedAsync(int ticketId, string userId)
        {
            var assignees = await _repository.GetByTicketIdAsync(ticketId);
            return assignees.Any(a => a.UserId == userId);
        }
    }
}