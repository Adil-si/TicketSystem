using System;
using System.Collections.Generic;
using System.Linq;  
using System.Threading.Tasks;
using TicketSystem.Domain.Interfaces;
using TicketSystem.Domain.Models;

namespace TicketSystem.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
            => await _departmentRepository.GetAllAsync();

        public async Task<Department?> GetDepartmentByIdAsync(int id)
            => await _departmentRepository.GetByIdAsync(id);

        public async Task<Department> CreateDepartmentAsync(Department department)
            => await _departmentRepository.CreateAsync(department);

        public async Task<Department> UpdateDepartmentAsync(Department department)
            => await _departmentRepository.UpdateAsync(department);

        public async Task DeleteDepartmentAsync(int id)
            => await _departmentRepository.DeleteAsync(id);
        public async Task<Department?> GetDepartmentByNameAsync(string name)
        {
            var departments = await _departmentRepository.GetAllAsync();
            return departments.FirstOrDefault(d => d.Name == name);
        }
    }
}