using Microsoft.EntityFrameworkCore;
using _08_VuTriDung_Assignment02.Models;

namespace _08_VuTriDung_Assignment02.DAOs
{
    public class CategoryDAO
    {
        private readonly FUNewsManagementDbContext _context;

        public CategoryDAO(FUNewsManagementDbContext context)
        {
            _context = context;
        }

        public IQueryable<Category> GetAll()
        {
            return _context.Categories
                .Include(c => c.ParentCategory)
                .AsNoTracking();
        }

        public Category? GetById(short id)
        {
            return _context.Categories
                .Include(c => c.ParentCategory)
                .AsNoTracking()
                .FirstOrDefault(c => c.CategoryID == id);
        }

        public Category Create(Category category)
        {
            _context.Categories.Add(category);
            _context.SaveChanges();
            return category;
        }

        public Category? Update(short id, Category updated)
        {
            var existing = _context.Categories.FirstOrDefault(c => c.CategoryID == id);
            if (existing == null) return null;

            existing.CategoryName = updated.CategoryName;
            existing.CategoryDesciption = updated.CategoryDesciption;
            existing.ParentCategoryID = updated.ParentCategoryID;
            existing.IsActive = updated.IsActive;

            _context.SaveChanges();
            return existing;
        }

        public bool HasArticles(short categoryId)
        {
            return _context.NewsArticles.Any(n => n.CategoryID == categoryId);
        }

        public (bool Success, string? ErrorMessage) Delete(short id)
        {
            var existing = _context.Categories.FirstOrDefault(c => c.CategoryID == id);
            if (existing == null)
            {
                return (false, $"Category with ID {id} not found.");
            }

            if (HasArticles(id))
            {
                return (false, "Cannot delete category because it contains news articles.");
            }

            _context.Categories.Remove(existing);
            _context.SaveChanges();
            return (true, null);
        }
    }
}
