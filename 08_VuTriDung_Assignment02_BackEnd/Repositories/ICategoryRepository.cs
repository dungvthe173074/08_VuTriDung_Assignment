using _08_VuTriDung_Assignment02.Models;

namespace _08_VuTriDung_Assignment02.Repositories
{
    public interface ICategoryRepository
    {
        IQueryable<Category> GetAll();
        Category? GetById(short id);
        Category Create(Category category);
        Category? Update(short id, Category category);
        (bool Success, string? ErrorMessage) Delete(short id);
    }
}
