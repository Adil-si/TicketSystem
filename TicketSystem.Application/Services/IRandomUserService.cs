using TicketSystem.Domain.Models;

namespace TicketSystem.Application.Services
{
    public interface IRandomUserService
    {
        Task<ApplicationUser> GenerateRandomUserAsync();
        Task<List<ApplicationUser>> GenerateRandomUsersAsync(int count);
        string GetAvatarUrl(string email);
    }
}