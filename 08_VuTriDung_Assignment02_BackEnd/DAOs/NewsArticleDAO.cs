using Microsoft.EntityFrameworkCore;
using _08_VuTriDung_Assignment02.Models;

namespace _08_VuTriDung_Assignment02.DAOs
{
    public class NewsArticleDAO
    {
        private static NewsArticleDAO? _instance;
        private static readonly object _instanceLock = new object();

        private NewsArticleDAO() { }

        public static NewsArticleDAO Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new NewsArticleDAO();
                    }
                    return _instance;
                }
            }
        }

        public List<NewsArticle> GetAll()
        {
            using var db = new FUNewsManagementDbContext();
            return db.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .AsNoTracking()
                .OrderByDescending(n => n.CreatedDate)
                .ToList();
        }

        public List<NewsArticle> GetActive()
        {
            using var db = new FUNewsManagementDbContext();
            return db.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .Where(n => n.NewsStatus == true)
                .AsNoTracking()
                .OrderByDescending(n => n.CreatedDate)
                .ToList();
        }

        public NewsArticle? GetById(string id)
        {
            using var db = new FUNewsManagementDbContext();
            return db.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .AsNoTracking()
                .FirstOrDefault(n => n.NewsArticleID == id);
        }

        public List<NewsArticle> GetByCreatedBy(short accountId)
        {
            using var db = new FUNewsManagementDbContext();
            return db.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .Where(n => n.CreatedByID == accountId)
                .AsNoTracking()
                .OrderByDescending(n => n.CreatedDate)
                .ToList();
        }

        public List<NewsArticle> GetReport(DateTime? startDate, DateTime? endDate)
        {
            using var db = new FUNewsManagementDbContext();
            var query = db.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .AsNoTracking()
                .AsQueryable();

            if (startDate.HasValue)
            {
                var start = startDate.Value.Date;
                query = query.Where(n => n.CreatedDate >= start);
            }

            if (endDate.HasValue)
            {
                var end = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(n => n.CreatedDate <= end);
            }

            return query.OrderByDescending(n => n.CreatedDate).ToList();
        }

        public NewsArticle Create(NewsArticle article, List<int>? tagIds)
        {
            using var db = new FUNewsManagementDbContext();

            if (string.IsNullOrWhiteSpace(article.NewsArticleID))
            {
                // Auto-generate ID if empty
                article.NewsArticleID = Guid.NewGuid().ToString("N")[..10];
            }

            if (!article.CreatedDate.HasValue)
            {
                article.CreatedDate = DateTime.Now;
            }

            db.NewsArticles.Add(article);
            db.SaveChanges();

            if (tagIds != null && tagIds.Any())
            {
                foreach (var tagId in tagIds.Distinct())
                {
                    db.NewsTags.Add(new NewsTag
                    {
                        NewsArticleID = article.NewsArticleID,
                        TagID = tagId
                    });
                }
                db.SaveChanges();
            }

            return GetById(article.NewsArticleID) ?? article;
        }

        public NewsArticle? Update(NewsArticle article, List<int>? tagIds)
        {
            using var db = new FUNewsManagementDbContext();
            var existing = db.NewsArticles
                .Include(n => n.NewsTags)
                .FirstOrDefault(n => n.NewsArticleID == article.NewsArticleID);

            if (existing == null) return null;

            existing.NewsTitle = article.NewsTitle;
            existing.Headline = article.Headline;
            existing.NewsContent = article.NewsContent;
            existing.NewsSource = article.NewsSource;
            existing.CategoryID = article.CategoryID;
            existing.NewsStatus = article.NewsStatus;
            existing.UpdatedByID = article.UpdatedByID;
            existing.ModifiedDate = DateTime.Now;

            // Update Tags
            if (tagIds != null)
            {
                db.NewsTags.RemoveRange(existing.NewsTags);
                foreach (var tagId in tagIds.Distinct())
                {
                    db.NewsTags.Add(new NewsTag
                    {
                        NewsArticleID = existing.NewsArticleID,
                        TagID = tagId
                    });
                }
            }

            db.SaveChanges();
            return GetById(existing.NewsArticleID);
        }

        public bool Delete(string id)
        {
            using var db = new FUNewsManagementDbContext();
            var existing = db.NewsArticles
                .Include(n => n.NewsTags)
                .FirstOrDefault(n => n.NewsArticleID == id);

            if (existing == null) return false;

            if (existing.NewsTags.Any())
            {
                db.NewsTags.RemoveRange(existing.NewsTags);
            }

            db.NewsArticles.Remove(existing);
            db.SaveChanges();
            return true;
        }
    }
}
