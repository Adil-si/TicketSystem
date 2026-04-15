using TicketSystem.Domain.Interfaces;
using TicketSystem.Domain.Models;

namespace TicketSystem.Application.Services
{
    public class CategoryService  : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task<Category> CreateCategoryAsync(Category category)
        {
            return await _categoryRepository.CreateAsync(category);
        }

        public async Task DeleteCategoryAsync(int id)
        {
            await _categoryRepository.DeleteAsync(id);
        }
        public async Task<Category> UpdateCategoryAsync(Category category)
        {
            return await _categoryRepository.UpdateAsync(category);
        }

    }
}
