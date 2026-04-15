using System;
using System.Collections.Generic;
using System.Text;
using TicketSystem.Domain.Models;

namespace TicketSystem.Domain.Interfaces
{
    public interface ITicketRepository
    {
        Task<IEnumerable<Ticket>> GetAllAsync();
        Task<Ticket?> GetByIdAsync(int id);

        Task<Ticket?> CreateAsync(Ticket ticket);

        Task<Ticket> UpdateAsync (Ticket ticket);

        Task DeleteAsync (int id);

    }
}
