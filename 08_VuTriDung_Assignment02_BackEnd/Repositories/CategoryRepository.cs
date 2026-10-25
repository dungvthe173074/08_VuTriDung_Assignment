using _08_VuTriDung_Assignment02.DAOs;
using _08_VuTriDung_Assignment02.Models;

namespace _08_VuTriDung_Assignment02.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        public IEnumerable<Category> GetAll()
        {
            return CategoryDAO.Instance.GetAll();
        }

        public Category? GetById(short id)
        {
            return CategoryDAO.Instance.GetById(id);
        }

        public Category Create(Category category)
        {
            return CategoryDAO.Instance.Create(category);
        }

        public Category? Update(Category category)
        {
            return CategoryDAO.Instance.Update(category);
        }

        public (bool Success, string? ErrorMessage) Delete(short id)
        {
            // Strict constraint: cannot delete if category has any news articles
            if (CategoryDAO.Instance.HasArticles(id))
            {
                return (false, "Cannot delete category because it contains news articles.");
            }

            bool deleted = CategoryDAO.Instance.Delete(id);
            if (!deleted)
            {
                return (false, "Category not found or could not be deleted.");
            }

            return (true, null);
        }
    }
}
