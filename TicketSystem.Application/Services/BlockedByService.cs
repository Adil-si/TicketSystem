using TicketSystem.Domain.Interfaces;
using TicketSystem.Domain.Models;

namespace TicketSystem.Application.Services
{
    public class BlockedByService : IBlockedByService
    {
        private readonly IBlockedByRepository _blockedByRepository;
        private readonly ITicketRepository _ticketRepository;

        public BlockedByService(IBlockedByRepository blockedByRepository, ITicketRepository ticketRepository)
        {
            _blockedByRepository = blockedByRepository;
            _ticketRepository = ticketRepository;
        }

        public async Task<IEnumerable<BlockedBy>> GetBlockingTicketsAsync(int ticketId)
        {
            return await _blockedByRepository.GetBlockingTicketsAsync(ticketId);
        }

        public async Task<IEnumerable<BlockedBy>> GetBlockedByTicketsAsync(int ticketId)
        {
            return await _blockedByRepository.GetBlockedByTicketsAsync(ticketId);
        }

        public async Task<BlockedBy> AddBlockAsync(int ticketId, int blockedByTicketId)
        {
            if (ticketId == blockedByTicketId)
                throw new InvalidOperationException("Ein Ticket kann sich nicht selbst blockieren.");

            var existingBlocks = await _blockedByRepository.GetBlockingTicketsAsync(ticketId);
            if (existingBlocks.Any(b => b.BlockedByTicketId == blockedByTicketId))
                throw new InvalidOperationException("Diese Blockierung existiert bereits.");

            var blockedBy = new BlockedBy
            {
                TicketId = ticketId,
                BlockedByTicketId = blockedByTicketId,
                CreatedAt = DateTime.UtcNow
            };

            return await _blockedByRepository.CreateAsync(blockedBy);
        }

        public async Task RemoveBlockAsync(int id)
        {
            await _blockedByRepository.DeleteAsync(id);
        }

        public async Task<bool> CanCloseTicketAsync(int ticketId)
        {
            return await _blockedByRepository.CanBeClosedAsync(ticketId);
        }

        public async Task<List<Ticket>> GetAvailableTicketsForBlockingAsync(int ticketId)
        {
            var allTickets = await _ticketRepository.GetAllAsync();
            var existingBlocks = await _blockedByRepository.GetBlockingTicketsAsync(ticketId);

            var blockedByIds = existingBlocks.Select(b => b.BlockedByTicketId).ToList();

            return allTickets
                .Where(t => t.Id != ticketId &&
                           !blockedByIds.Contains(t.Id) &&
                           t.Status != TicketStatus.Closed)
                .OrderBy(t => t.Title)
                .ToList();
        }
    }
}