using _08_VuTriDung_Assignment02.Models;

namespace _08_VuTriDung_Assignment02.Repositories
{
    public interface INewsArticleRepository
    {
        IEnumerable<NewsArticle> GetAll();
        IEnumerable<NewsArticle> GetActive();
        NewsArticle? GetById(string id);
        IEnumerable<NewsArticle> GetByCreatedBy(short accountId);
        IEnumerable<NewsArticle> GetReport(DateTime? startDate, DateTime? endDate);
        NewsArticle Create(NewsArticle article, List<int>? tagIds);
        NewsArticle? Update(NewsArticle article, List<int>? tagIds);
        bool Delete(string id);
    }
}
