using _08_VuTriDung_Assignment02.DAOs;
using _08_VuTriDung_Assignment02.Models;

namespace _08_VuTriDung_Assignment02.Repositories
{
    public class NewsArticleRepository : INewsArticleRepository
    {
        public IEnumerable<NewsArticle> GetAll()
        {
            return NewsArticleDAO.Instance.GetAll();
        }

        public IEnumerable<NewsArticle> GetActive()
        {
            return NewsArticleDAO.Instance.GetActive();
        }

        public NewsArticle? GetById(string id)
        {
            return NewsArticleDAO.Instance.GetById(id);
        }

        public IEnumerable<NewsArticle> GetByCreatedBy(short accountId)
        {
            return NewsArticleDAO.Instance.GetByCreatedBy(accountId);
        }

        public IEnumerable<NewsArticle> GetReport(DateTime? startDate, DateTime? endDate)
        {
            return NewsArticleDAO.Instance.GetReport(startDate, endDate);
        }

        public NewsArticle Create(NewsArticle article, List<int>? tagIds)
        {
            return NewsArticleDAO.Instance.Create(article, tagIds);
        }

        public NewsArticle? Update(NewsArticle article, List<int>? tagIds)
        {
            return NewsArticleDAO.Instance.Update(article, tagIds);
        }

        public bool Delete(string id)
        {
            return NewsArticleDAO.Instance.Delete(id);
        }
    }
}
