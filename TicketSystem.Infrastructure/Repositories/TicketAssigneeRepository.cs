using Microsoft.EntityFrameworkCore;
using TicketSystem.Domain.Interfaces;
using TicketSystem.Domain.Models;
using TicketSystem.Infrastructure.Data;

namespace TicketSystem.Infrastructure.Repositories
{
    public class TicketAssigneeRepository : ITicketAssigneeRepository
    {
        private readonly AppDbContext _context;

        public TicketAssigneeRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TicketAssignee>> GetByTicketIdAsync(int ticketId)
        {
            return await _context.TicketAssignees
                .Include(a => a.User)
                .Where(a => a.TicketId == ticketId)
                .ToListAsync();
        }

        public async Task<TicketAssignee> CreateAsync(TicketAssignee assignee)
        {
            _context.TicketAssignees.Add(assignee);
            await _context.SaveChangesAsync();
            return assignee;
        }

        public async Task DeleteAsync(int id)
        {
            var assignee = await _context.TicketAssignees.FindAsync(id);
            if (assignee != null)
            {
                _context.TicketAssignees.Remove(assignee);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteByTicketIdAsync(int ticketId)
        {
            var assignees = await _context.TicketAssignees
                .Where(a => a.TicketId == ticketId)
                .ToListAsync();
            _context.TicketAssignees.RemoveRange(assignees);
            await _context.SaveChangesAsync();
        }
    }
}