using System;
using System.Collections.Generic;
using System.Text;
using TicketSystem.Domain.Interfaces;
using TicketSystem.Domain.Models;
using TicketSystem.Infrastructure.Repositories;


namespace TicketSystem.Application.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _repo;
        private readonly IAttachmentRepository _attachmentRepo;
        private readonly IUserRepository _userRepo;
        public TicketService(ITicketRepository repo,
            IAttachmentRepository attachmentRepo,
            IUserRepository userRepo)
        {
            _repo = repo;
            _attachmentRepo = attachmentRepo;
            _userRepo = userRepo;
        }
        public async Task<IEnumerable<Ticket>> GetAllTicketsAsync()
        {
            return await _repo.GetAllAsync();
        }
        public async Task<Ticket?> GetTicketByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }
        public async Task<Ticket?> CreateTicketAsync(Ticket ticket)
        {
            ticket.CreatedAt = DateTime.UtcNow;
            return await _repo.CreateAsync(ticket);
        }
        public async Task<Ticket> UpdateTicketAsync(Ticket ticket)
        {
            return await _repo.UpdateAsync(ticket);
        }
        public async Task<IEnumerable<Ticket>> GetTicketsByUserDepartmentAsync(string userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return new List<Ticket>();

            var allTickets = await _repo.GetAllAsync();

            // User sieht Tickets seiner Abteilung// neu code
            return allTickets.Where(t =>
                t.DepartmentId == user.DepartmentId ||
                t.ApplicationUserId == userId);
        }
        public async Task DeleteTicketAsync(int id)
        {
            await _attachmentRepo.DeleteByTicketIdAsync(id);
            await _repo.DeleteAsync(id);
        }
        public async Task<Ticket?> CreateTicketAsync(Ticket ticket, string userId) //double
        {
           
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User nicht gefunden");

            // Wenn keine Abteilung gewählt wurde, User-Abteilung verwenden
            if (ticket.DepartmentId == null || ticket.DepartmentId == 0)
            {
                ticket.DepartmentId = user.DepartmentId;
            }

            ticket.ApplicationUserId = userId;
            ticket.CreatedAt = DateTime.UtcNow;
            ticket.Status = TicketStatus.Open;

            return await _repo.CreateAsync(ticket);
        }
        public async Task<IEnumerable<Ticket>> GetTicketsByUserDepartmentIdAsync(string userId)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null) return new List<Ticket>();

            var allTickets = await _repo.GetAllAsync();

            return allTickets.Where(t => 
            t.DepartmentId == user.DepartmentId || t.ApplicationUserId == userId);
        }
        public async Task<IEnumerable<Ticket>> GetTicketsByUserIdAsync(string userId)
        {
            var allTickets = await _repo.GetAllAsync();
            return allTickets.Where(t => t.ApplicationUserId == userId);
        }
        public async Task<IEnumerable<Ticket>> GetTicketsAssignedToUserAsync(string userId)
        {
            var allTickets = await _repo.GetAllAsync();
            return allTickets.Where(t => t.Assignees != null && t.Assignees.Any(a => a.UserId == userId));
        }
    }
}
