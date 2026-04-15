using System;
using System.Collections.Generic;
using System.Text;
using TicketSystem.Domain.Models;

namespace TicketSystem.Application.Services
{
    public interface IUserService
    {
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<ApplicationUser?> GetUserByIdAsync(string id);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task DeleteUserAsync(string id);
    }
}
