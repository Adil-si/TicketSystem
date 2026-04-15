using System;
using System.Collections.Generic;
using System.Text;
using TicketSystem.Domain.Models;

namespace TicketSystem.Application.Services
{
   public interface ITicketService
    {
        Task<IEnumerable<Ticket>> GetAllTicketsAsync();
        Task<Ticket?> GetTicketByIdAsync(int id);
        Task<Ticket?> CreateTicketAsync(Ticket ticket,string userId);
        Task<Ticket> UpdateTicketAsync(Ticket ticket);

        Task DeleteTicketAsync(int id);
        
        Task<IEnumerable<Ticket>> GetTicketsByUserIdAsync(string userId);
        Task<IEnumerable<Ticket>> GetTicketsByUserDepartmentAsync(string userId);
        Task<IEnumerable<Ticket>> GetTicketsAssignedToUserAsync(string userId);
    }
}
