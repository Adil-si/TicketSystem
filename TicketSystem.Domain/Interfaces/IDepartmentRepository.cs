using System;
using System.Collections.Generic;
using System.Text;
using TicketSystem.Domain.Models;

namespace TicketSystem.Domain.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(int id);
        Task<Department> CreateAsync(Department department);
        Task<Department> UpdateAsync(Department department);
        Task DeleteAsync(int id);
    }
}
