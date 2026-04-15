using System;
using System.Collections.Generic;
using System.Text;
using TicketSystem.Domain.Interfaces;
using TicketSystem.Domain.Models;

namespace TicketSystem.Application.Services
{

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string id)
        {
            return await _userRepository.GetByIdAsync(id);
        }
        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task DeleteUserAsync(string id)  
        {
            await _userRepository.DeleteAsync(id);
        }
    }
}


