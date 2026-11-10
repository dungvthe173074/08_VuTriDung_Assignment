using Microsoft.EntityFrameworkCore;
using _08_VuTriDung_Assignment02.DTOs;
using _08_VuTriDung_Assignment02.Models;

namespace _08_VuTriDung_Assignment02.DAOs
{
    public class NewsArticleDAO
    {
        private readonly FUNewsManagementDbContext _context;

        public NewsArticleDAO(FUNewsManagementDbContext context)
        {
            _context = context;
        }

        public IQueryable<NewsArticleDTO> GetAll()
        {
            return _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .AsNoTracking()
                .OrderByDescending(n => n.CreatedDate)
                .Select(n => new NewsArticleDTO
                {
                    NewsArticleID = n.NewsArticleID,
                    NewsTitle = n.NewsTitle,
                    Headline = n.Headline,
                    CreatedDate = n.CreatedDate,
                    NewsContent = n.NewsContent,
                    NewsSource = n.NewsSource,
                    CategoryID = n.CategoryID,
                    CategoryName = n.Category != null ? n.Category.CategoryName : null,
                    NewsStatus = n.NewsStatus,
                    CreatedByID = n.CreatedByID,
                    CreatedByName = n.CreatedBy != null ? n.CreatedBy.AccountName : null,
                    UpdatedByID = n.UpdatedByID,
                    ModifiedDate = n.ModifiedDate,
                    TagIDs = n.NewsTags.Select(nt => nt.TagID).ToList(),
                    Tags = n.NewsTags.Where(nt => nt.Tag != null).Select(nt => new TagDTO
                    {
                        TagID = nt.TagID,
                        TagName = nt.Tag != null ? nt.Tag.TagName : null,
                        Note = nt.Tag != null ? nt.Tag.Note : null
                    }).ToList()
                });
        }

        public IQueryable<NewsArticleDTO> GetActive()
        {
            return _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.NewsTags)
                    .ThenInclude(nt => nt.Tag)
                .Where(n => n.NewsStatus == true)
                .AsNoTracking()
                .OrderByDescending(n => n.CreatedDate)
                .Select(n => new NewsArticleDTO
                {
                    NewsArticleID = n.NewsArticleID,
                    NewsTitle = n.NewsTitle,
                    Headline = n.Headline,
                    CreatedDate = n.CreatedDate,
                    NewsContent = n.NewsContent,
                    NewsSource = n.NewsSource,
                    CategoryID = n.CategoryID,
                    CategoryName = n.Category != null ? n.Category.CategoryName : null,
                    NewsStatus = n.NewsStatus,
                    CreatedByID = n.CreatedByID,
                    CreatedByName = n.CreatedBy != null ? n.CreatedBy.AccountName : null,
                    UpdatedByID = n.UpdatedByID,
                    ModifiedDate = n.ModifiedDate,
                    TagIDs = n.NewsTags.Select(nt => nt.TagID).ToList(),
                    Tags = n.NewsTags.Where(nt => nt.Tag != null).Select(nt => new TagDTO
                    {
                        TagID = nt.TagID,
                        TagName = nt.Tag != null ? nt.Tag.TagName : null,
                        Note = nt.Tag != null ? nt.Tag.Note : null
                    }).ToList()
                });
        }

        public NewsArticleDTO? GetById(string id)
        {
            return GetAll().FirstOrDefault(n => n.NewsArticleID == id);
        }

        public List<NewsArticleDTO> GetByCreatedBy(short accountId)
        {
            return GetAll().Where(n => n.CreatedByID == accountId).ToList();
        }

        public List<NewsArticleDTO> GetReport(DateTime? startDate, DateTime? endDate)
        {
            var query = _context.NewsArticles
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

            return query.OrderByDescending(n => n.CreatedDate)
                .Select(n => new NewsArticleDTO
                {
                    NewsArticleID = n.NewsArticleID,
                    NewsTitle = n.NewsTitle,
                    Headline = n.Headline,
                    CreatedDate = n.CreatedDate,
                    NewsContent = n.NewsContent,
                    NewsSource = n.NewsSource,
                    CategoryID = n.CategoryID,
                    CategoryName = n.Category != null ? n.Category.CategoryName : null,
                    NewsStatus = n.NewsStatus,
                    CreatedByID = n.CreatedByID,
                    CreatedByName = n.CreatedBy != null ? n.CreatedBy.AccountName : null,
                    UpdatedByID = n.UpdatedByID,
                    ModifiedDate = n.ModifiedDate,
                    TagIDs = n.NewsTags.Select(nt => nt.TagID).ToList(),
                    Tags = n.NewsTags.Where(nt => nt.Tag != null).Select(nt => new TagDTO
                    {
                        TagID = nt.TagID,
                        TagName = nt.Tag != null ? nt.Tag.TagName : null,
                        Note = nt.Tag != null ? nt.Tag.Note : null
                    }).ToList()
                })
                .ToList();
        }

        public bool Exists(string id)
        {
            return _context.NewsArticles.Any(n => n.NewsArticleID == id);
        }

        public NewsArticleDTO Create(NewsArticle article, List<int>? tagIds)
        {
            if (string.IsNullOrWhiteSpace(article.NewsArticleID))
            {
                article.NewsArticleID = Guid.NewGuid().ToString("N")[..10];
            }

            if (!article.CreatedDate.HasValue)
            {
                article.CreatedDate = DateTime.Now;
            }

            _context.NewsArticles.Add(article);
            _context.SaveChanges();

            if (tagIds != null && tagIds.Any())
            {
                foreach (var tagId in tagIds.Distinct())
                {
                    _context.NewsTags.Add(new NewsTag
                    {
                        NewsArticleID = article.NewsArticleID,
                        TagID = tagId
                    });
                }
                _context.SaveChanges();
            }

            return GetById(article.NewsArticleID)!;
        }

        public NewsArticleDTO? Update(string id, NewsArticleCreateUpdateDTO dto, short? updatedById)
        {
            var existing = _context.NewsArticles
                .Include(n => n.NewsTags)
                .FirstOrDefault(n => n.NewsArticleID == id);

            if (existing == null) return null;

            existing.NewsTitle = dto.NewsTitle;
            existing.Headline = dto.Headline;
            existing.NewsContent = dto.NewsContent;
            existing.NewsSource = dto.NewsSource;
            existing.CategoryID = dto.CategoryID;
            existing.NewsStatus = dto.NewsStatus;
            existing.UpdatedByID = updatedById;
            existing.ModifiedDate = DateTime.Now;

            if (dto.TagIDs != null)
            {
                _context.NewsTags.RemoveRange(existing.NewsTags);
                foreach (var tagId in dto.TagIDs.Distinct())
                {
                    _context.NewsTags.Add(new NewsTag
                    {
                        NewsArticleID = existing.NewsArticleID,
                        TagID = tagId
                    });
                }
            }

            _context.SaveChanges();
            return GetById(id);
        }

        public bool Delete(string id)
        {
            var existing = _context.NewsArticles
                .Include(n => n.NewsTags)
                .FirstOrDefault(n => n.NewsArticleID == id);

            if (existing == null) return false;

            if (existing.NewsTags.Any())
            {
                _context.NewsTags.RemoveRange(existing.NewsTags);
            }

            _context.NewsArticles.Remove(existing);
            _context.SaveChanges();
            return true;
        }
    }
}
