using Microsoft.EntityFrameworkCore;
using TicketSystem.Application.DTOs;
using TicketSystem.Domain.Interfaces;
using TicketSystem.Domain.Models;
using TicketSystem.Infrastructure.Data;

namespace TicketSystem.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly AppDbContext _context;
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;

        public DashboardService(AppDbContext context, ITicketRepository ticketRepository, IUserRepository userRepository)
        {
            _context = context;
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
        }

        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            var tickets = await _context.Tickets.ToListAsync();
            var users = await _context.Users.ToListAsync();

            // User nach Rollen zählen
            var adminCount = await _context.UserRoles
                .Where(ur => ur.RoleId == _context.Roles.FirstOrDefault(r => r.Name == "Admin").Id)
                .CountAsync();

            var userCount = users.Count - adminCount;

            return new DashboardDto
            {
                // Ti Statistik
                TotalTickets = tickets.Count,
                OpenTickets = tickets.Count(t => t.Status == TicketStatus.Open),
                ClosedTickets = tickets.Count(t => t.Status == TicketStatus.Closed),
                InProgressTickets = tickets.Count(t => t.Status == TicketStatus.InProgress),

                
                TotalUsers = users.Count,
                AdminCount = adminCount,
                UserCount = userCount,

                // letzte 5 
                RecentTickets = tickets
                    .OrderByDescending(t => t.CreatedAt)
                    .Take(5)
                    .Select(t => new TicketDto
                    {
                        Id = t.Id,
                        Title = t.Title,
                        Status = t.Status.ToString(),
                        CreatorName = _context.Users.FirstOrDefault(u => u.Id == t.ApplicationUserId)?.Name ?? "Unbekannt",
                        CreatedAt = t.CreatedAt
                    })
                    .ToList()
            };
        }
    }
}