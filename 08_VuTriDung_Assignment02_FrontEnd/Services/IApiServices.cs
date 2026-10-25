using _08_VuTriDung_Assignment02_FrontEnd.Models;

namespace _08_VuTriDung_Assignment02_FrontEnd.Services
{
    public interface IAuthApiService
    {
        Task<UserSessionViewModel?> LoginAsync(LoginViewModel model);
    }

    public interface IAccountApiService
    {
        Task<List<AccountViewModel>> GetAllAsync();
        Task<AccountViewModel?> GetByIdAsync(short id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(AccountViewModel model);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(AccountViewModel model);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(short id);
        Task<AccountViewModel?> GetProfileAsync();
        Task<(bool Success, string? ErrorMessage)> UpdateProfileAsync(AccountViewModel model);
    }

    public interface ICategoryApiService
    {
        Task<List<CategoryViewModel>> GetAllAsync();
        Task<CategoryViewModel?> GetByIdAsync(short id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(CategoryViewModel model);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(CategoryViewModel model);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(short id);
    }

    public interface ITagApiService
    {
        Task<List<TagViewModel>> GetAllAsync();
        Task<TagViewModel?> GetByIdAsync(int id);
    }

    public interface INewsArticleApiService
    {
        Task<List<NewsArticleViewModel>> GetAllAsync();
        Task<List<NewsArticleViewModel>> GetActiveAsync();
        Task<List<NewsArticleViewModel>> GetMyHistoryAsync(short? accountId = null);
        Task<List<NewsArticleViewModel>> GetReportAsync(DateTime? startDate, DateTime? endDate);
        Task<NewsArticleViewModel?> GetByIdAsync(string id);
        Task<(bool Success, string? ErrorMessage)> CreateAsync(NewsArticleCreateEditViewModel model);
        Task<(bool Success, string? ErrorMessage)> UpdateAsync(NewsArticleCreateEditViewModel model);
        Task<(bool Success, string? ErrorMessage)> DeleteAsync(string id);
    }
}
