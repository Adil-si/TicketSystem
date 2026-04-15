using Microsoft.EntityFrameworkCore;
using TicketSystem.Domain.Interfaces;
using TicketSystem.Domain.Models;
using TicketSystem.Infrastructure.Data;

namespace TicketSystem.Infrastructure.Repositories
{
    public class BlockedByRepository : IBlockedByRepository
    {
        private readonly AppDbContext _context;

        public BlockedByRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BlockedBy>> GetBlockingTicketsAsync(int ticketId)
        {
            return await _context.Set<BlockedBy>()
                .Include(b => b.BlockedByTicket)
                .Where(b => b.TicketId == ticketId)
                .ToListAsync();
        }

        public async Task<IEnumerable<BlockedBy>> GetBlockedByTicketsAsync(int ticketId)
        {
            return await _context.Set<BlockedBy>()
                .Include(b => b.Ticket)
                .Where(b => b.BlockedByTicketId == ticketId)
                .ToListAsync();
        }

        public async Task<BlockedBy?> GetByIdAsync(int id)
        {
            return await _context.Set<BlockedBy>()
                .Include(b => b.Ticket)
                .Include(b => b.BlockedByTicket)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<BlockedBy> CreateAsync(BlockedBy blockedBy)
        {
            _context.Set<BlockedBy>().Add(blockedBy);
            await _context.SaveChangesAsync();
            return blockedBy;
        }

        public async Task DeleteAsync(int id)
        {
            var blockedBy = await _context.Set<BlockedBy>().FindAsync(id);
            if (blockedBy != null)
            {
                _context.Set<BlockedBy>().Remove(blockedBy);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsBlockedAsync(int ticketId)
        {
            return await _context.Set<BlockedBy>()
                .AnyAsync(b => b.TicketId == ticketId);
        }

        public async Task<bool> CanBeClosedAsync(int ticketId)
        {
            var blockingTickets = await _context.Set<BlockedBy>()
                .Include(b => b.BlockedByTicket)
                .Where(b => b.TicketId == ticketId)
                .ToListAsync();

            return !blockingTickets.Any(block =>
                block.BlockedByTicket != null &&
                block.BlockedByTicket.Status != TicketStatus.Closed);
        }
    }
}