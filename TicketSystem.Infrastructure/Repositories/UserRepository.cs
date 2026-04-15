using Microsoft.EntityFrameworkCore;
using TicketSystem.Domain.Interfaces;
using TicketSystem.Domain.Models;
using TicketSystem.Infrastructure.Data;

namespace TicketSystem.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;

    public UserRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
        => await _context.Users.ToListAsync();

    public async Task<ApplicationUser?> GetByIdAsync(string id)
        => await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

    public async Task<ApplicationUser?> GetByEmailAsync(string email)
        => await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<ApplicationUser> CreateAsync(ApplicationUser user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public async Task DeleteAsync(string id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
