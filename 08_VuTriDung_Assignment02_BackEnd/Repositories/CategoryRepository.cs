using _08_VuTriDung_Assignment02.DAOs;
using _08_VuTriDung_Assignment02.Models;

namespace _08_VuTriDung_Assignment02.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly CategoryDAO _categoryDAO;

        public CategoryRepository(CategoryDAO categoryDAO)
        {
            _categoryDAO = categoryDAO;
        }

        public IQueryable<Category> GetAll() => _categoryDAO.GetAll();

        public Category? GetById(short id) => _categoryDAO.GetById(id);

        public Category Create(Category category) => _categoryDAO.Create(category);

        public Category? Update(short id, Category category) => _categoryDAO.Update(id, category);

        public (bool Success, string? ErrorMessage) Delete(short id) => _categoryDAO.Delete(id);
    }
}
