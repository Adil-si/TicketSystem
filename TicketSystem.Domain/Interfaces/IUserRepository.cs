using System;
using System.Collections.Generic;
using System.Text;
using TicketSystem.Domain.Models;

namespace TicketSystem.Domain.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<ApplicationUser?> GetByIdAsync(string id);
        Task<ApplicationUser?> GetByEmailAsync (string email);
        Task<ApplicationUser> CreateAsync(ApplicationUser applicationUser);
        Task DeleteAsync (string id);
    }
}
