using System;
using System.Collections.Generic;
using System.Text;
using TicketSystem.Domain.Models;

namespace TicketSystem.Domain.Interfaces
{
  public interface IBlockedByRepository
    {
        Task<IEnumerable<BlockedBy>> GetBlockingTicketsAsync(int ticketId);
        Task<IEnumerable<BlockedBy>> GetBlockedByTicketsAsync(int ticketId);
        Task<BlockedBy?> GetByIdAsync(int id);
        Task<BlockedBy> CreateAsync(BlockedBy blockedBy);
        Task DeleteAsync(int id);
        Task<bool> IsBlockedAsync(int ticketId);
        Task<bool> CanBeClosedAsync(int ticketId);
    }
}
