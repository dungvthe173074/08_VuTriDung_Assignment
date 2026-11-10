using _08_VuTriDung_Assignment02.DAOs;
using _08_VuTriDung_Assignment02.DTOs;
using _08_VuTriDung_Assignment02.Models;

namespace _08_VuTriDung_Assignment02.Repositories
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        private readonly NewsArticleDAO _newsArticleDAO;

        public NewsArticleRepository(NewsArticleDAO newsArticleDAO)
        {
            _newsArticleDAO = newsArticleDAO;
        }

        public IQueryable<NewsArticleDTO> GetAll() => _newsArticleDAO.GetAll();

        public IQueryable<NewsArticleDTO> GetActive() => _newsArticleDAO.GetActive();

        public NewsArticleDTO? GetById(string id) => _newsArticleDAO.GetById(id);

        public List<NewsArticleDTO> GetByCreatedBy(short accountId) => _newsArticleDAO.GetByCreatedBy(accountId);

        public List<NewsArticleDTO> GetReport(DateTime? startDate, DateTime? endDate) => _newsArticleDAO.GetReport(startDate, endDate);

        public bool Exists(string id) => _newsArticleDAO.Exists(id);

        public NewsArticleDTO Create(NewsArticle article, List<int>? tagIds) => _newsArticleDAO.Create(article, tagIds);

        public NewsArticleDTO? Update(string id, NewsArticleCreateUpdateDTO dto, short? updatedById)
        {
            return _newsArticleDAO.Update(id, dto, updatedById);
        }

        public bool Delete(string id) => _newsArticleDAO.Delete(id);
    }
}
