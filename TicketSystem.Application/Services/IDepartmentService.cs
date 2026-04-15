using TicketSystem.Domain.Models;

namespace TicketSystem.Application.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<Department?> GetDepartmentByIdAsync(int id);
        Task<Department> CreateDepartmentAsync(Department department);
        Task<Department> UpdateDepartmentAsync(Department department);
        Task<Department?> GetDepartmentByNameAsync(string name);
        Task DeleteDepartmentAsync(int id);
    }
}