using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using TicketSystem.Domain.Models;
using TicketSystem.Infrastructure.Data;
using TicketSystem.Domain.Interfaces;

namespace TicketSystem.Infrastructure.Repositories
{
    public class TicketRepository : ITicketRepository
    {
        private readonly AppDbContext _context;

        public TicketRepository(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Ticket>> GetAllAsync()
            => await _context.Tickets.Include(t => t.Category).Include(t => t.ApplicationUser)
            .ToListAsync();

        public async Task<Ticket?> GetByIdAsync(int id)
            => await _context.Tickets.Include(t => t.Category).Include(t =>t.ApplicationUser)
            .FirstOrDefaultAsync(t => t.Id == id);

        public async Task<Ticket?> CreateAsync(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();
            return ticket;

        }
        public async Task<Ticket> UpdateAsync(Ticket ticket) 
        {
            _context.Entry(ticket).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ticket;
        }
        public async Task DeleteAsync(int id) 
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket != null) 
            {
                _context.Tickets.Remove(ticket);
                await _context.SaveChangesAsync();
            
            }
        }

    }
}
