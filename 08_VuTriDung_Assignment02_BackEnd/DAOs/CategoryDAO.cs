using Microsoft.EntityFrameworkCore;
using _08_VuTriDung_Assignment02.Models;

namespace _08_VuTriDung_Assignment02.DAOs
{
    public class CategoryDAO
    {
        private static CategoryDAO? _instance;
        private static readonly object _instanceLock = new object();

        private CategoryDAO() { }

        public static CategoryDAO Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new CategoryDAO();
                    }
                    return _instance;
                }
            }
        }

        public List<Category> GetAll()
        {
            using var db = new FUNewsManagementDbContext();
            return db.Categories
                .Include(c => c.ParentCategory)
                .AsNoTracking()
                .ToList();
        }

        public Category? GetById(short id)
        {
            using var db = new FUNewsManagementDbContext();
            return db.Categories
                .Include(c => c.ParentCategory)
                .AsNoTracking()
                .FirstOrDefault(c => c.CategoryID == id);
        }

        public Category Create(Category category)
        {
            using var db = new FUNewsManagementDbContext();
            db.Categories.Add(category);
            db.SaveChanges();
            return category;
        }

        public Category? Update(Category category)
        {
            using var db = new FUNewsManagementDbContext();
            var existing = db.Categories.FirstOrDefault(c => c.CategoryID == category.CategoryID);
            if (existing == null) return null;

            existing.CategoryName = category.CategoryName;
            existing.CategoryDesciption = category.CategoryDesciption;
            existing.ParentCategoryID = category.ParentCategoryID;
            existing.IsActive = category.IsActive;

            db.SaveChanges();
            return existing;
        }

        public bool HasArticles(short categoryId)
        {
            using var db = new FUNewsManagementDbContext();
            return db.NewsArticles.Any(n => n.CategoryID == categoryId);
        }

        public bool Delete(short id)
        {
            using var db = new FUNewsManagementDbContext();
            var existing = db.Categories.FirstOrDefault(c => c.CategoryID == id);
            if (existing == null) return false;

            db.Categories.Remove(existing);
            db.SaveChanges();
            return true;
        }
    }
}
