using _08_VuTriDung_Assignment02.DTOs;
using _08_VuTriDung_Assignment02.Models;

namespace _08_VuTriDung_Assignment02.Repositories
{
    public interface INewsArticleRepository
    {
        IQueryable<NewsArticleDTO> GetAll();
        IQueryable<NewsArticleDTO> GetActive();
        NewsArticleDTO? GetById(string id);
        List<NewsArticleDTO> GetByCreatedBy(short accountId);
        List<NewsArticleDTO> GetReport(DateTime? startDate, DateTime? endDate);
        bool Exists(string id);
        NewsArticleDTO Create(NewsArticle article, List<int>? tagIds);
        NewsArticleDTO? Update(string id, NewsArticleCreateUpdateDTO dto, short? updatedById);
        bool Delete(string id);
    }
}
