using System.Net.Http.Json;
using System.Text.Json;
using _08_VuTriDung_Assignment02_FrontEnd.Models;

namespace _08_VuTriDung_Assignment02_FrontEnd.Services
{
    public class ApiServiceBase
    {
        protected readonly IHttpClientFactory _clientFactory;
        protected readonly string _baseUrl;
        protected static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public ApiServiceBase(IHttpClientFactory clientFactory, IConfiguration configuration)
        {
            _clientFactory = clientFactory;
            _baseUrl = configuration["BackendApiUrl"];
            if (!_baseUrl.EndsWith("/")) _baseUrl += "/";
        }

        protected HttpClient CreateClient()
        {
            var client = _clientFactory.CreateClient("BackendApi");
            if (client.BaseAddress == null)
            {
                client.BaseAddress = new Uri(_baseUrl);
            }
            return client;
        }

        protected static async Task<List<T>> DeserializeListAsync<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(content)) return new List<T>();

            try
            {
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;
                if (root.ValueKind == JsonValueKind.Array)
                {
                    return JsonSerializer.Deserialize<List<T>>(content, _jsonOptions) ?? new List<T>();
                }
                if (root.ValueKind == JsonValueKind.Object && root.TryGetProperty("value", out var valueProp) && valueProp.ValueKind == JsonValueKind.Array)
                {
                    return JsonSerializer.Deserialize<List<T>>(valueProp.GetRawText(), _jsonOptions) ?? new List<T>();
                }
            }
            catch
            {
                // Fallback direct deserialize
            }
            return new List<T>();
        }

        protected static async Task<string> ExtractErrorMessageAsync(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            try
            {
                using var doc = JsonDocument.Parse(content);
                if (doc.RootElement.TryGetProperty("message", out var msg))
                {
                    return msg.GetString() ?? "Operation failed.";
                }
            }
            catch { }
            return string.IsNullOrWhiteSpace(content) ? $"Error ({response.StatusCode})" : content;
        }
    }

    public class AuthApiService : ApiServiceBase, IAuthApiService
    {
        public AuthApiService(IHttpClientFactory clientFactory, IConfiguration configuration) 
            : base(clientFactory, configuration) { }

        public async Task<UserSessionViewModel?> LoginAsync(LoginViewModel model)
        {
            try
            {
                var client = CreateClient();
                var response = await client.PostAsJsonAsync("api/Auth/login", new
                {
                    email = model.Email.Trim(),
                    password = model.Password.Trim()
                });

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<UserSessionViewModel>(content, _jsonOptions);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error calling login endpoint: {ex.Message}");
                return null;
            }
        }
    }

    public class AccountApiService : ApiServiceBase, IAccountApiService
    {
        public AccountApiService(IHttpClientFactory clientFactory, IConfiguration configuration) 
            : base(clientFactory, configuration) { }

        public async Task<List<AccountViewModel>> GetAllAsync()
        {
            var client = CreateClient();
            var response = await client.GetAsync("api/Accounts");
            if (!response.IsSuccessStatusCode) return new List<AccountViewModel>();
            return await DeserializeListAsync<AccountViewModel>(response);
        }

        public async Task<AccountViewModel?> GetByIdAsync(short id)
        {
            var client = CreateClient();
            var response = await client.GetAsync($"api/Accounts/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AccountViewModel>(content, _jsonOptions);
        }

        public async Task<AccountViewModel?> GetProfileAsync()
        {
            var client = CreateClient();
            var response = await client.GetAsync("api/Accounts/profile");
            if (!response.IsSuccessStatusCode) return null;
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<AccountViewModel>(content, _jsonOptions);
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateProfileAsync(AccountViewModel model)
        {
            var client = CreateClient();
            var response = await client.PutAsJsonAsync("api/Accounts/profile", model);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtractErrorMessageAsync(response));
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(AccountViewModel model)
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync("api/Accounts", model);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtractErrorMessageAsync(response));
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(AccountViewModel model)
        {
            var client = CreateClient();
            var response = await client.PutAsJsonAsync($"api/Accounts/{model.AccountID}", model);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtractErrorMessageAsync(response));
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(short id)
        {
            var client = CreateClient();
            var response = await client.DeleteAsync($"api/Accounts/{id}");
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtractErrorMessageAsync(response));
        }
    }

    public class CategoryApiService : ApiServiceBase, ICategoryApiService
    {
        public CategoryApiService(IHttpClientFactory clientFactory, IConfiguration configuration) 
            : base(clientFactory, configuration) { }

        public async Task<List<CategoryViewModel>> GetAllAsync()
        {
            var client = CreateClient();
            var response = await client.GetAsync("api/Categories");
            if (!response.IsSuccessStatusCode) return new List<CategoryViewModel>();
            return await DeserializeListAsync<CategoryViewModel>(response);
        }

        public async Task<CategoryViewModel?> GetByIdAsync(short id)
        {
            var client = CreateClient();
            var response = await client.GetAsync($"api/Categories/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<CategoryViewModel>(content, _jsonOptions);
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(CategoryViewModel model)
        {
            var client = CreateClient();
            var response = await client.PostAsJsonAsync("api/Categories", model);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtractErrorMessageAsync(response));
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(CategoryViewModel model)
        {
            var client = CreateClient();
            var response = await client.PutAsJsonAsync($"api/Categories/{model.CategoryID}", model);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtractErrorMessageAsync(response));
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(short id)
        {
            var client = CreateClient();
            var response = await client.DeleteAsync($"api/Categories/{id}");
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtractErrorMessageAsync(response));
        }
    }

    public class TagApiService : ApiServiceBase, ITagApiService
    {
        public TagApiService(IHttpClientFactory clientFactory, IConfiguration configuration) 
            : base(clientFactory, configuration) { }

        public async Task<List<TagViewModel>> GetAllAsync()
        {
            var client = CreateClient();
            var response = await client.GetAsync("api/Tags");
            if (!response.IsSuccessStatusCode) return new List<TagViewModel>();
            return await DeserializeListAsync<TagViewModel>(response);
        }

        public async Task<TagViewModel?> GetByIdAsync(int id)
        {
            var client = CreateClient();
            var response = await client.GetAsync($"api/Tags/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TagViewModel>(content, _jsonOptions);
        }
    }

    public class NewsArticleApiService : ApiServiceBase, INewsArticleApiService
    {
        public NewsArticleApiService(IHttpClientFactory clientFactory, IConfiguration configuration) 
            : base(clientFactory, configuration) { }

        public async Task<List<NewsArticleViewModel>> GetAllAsync()
        {
            var client = CreateClient();
            var response = await client.GetAsync("api/NewsArticles");
            if (!response.IsSuccessStatusCode) return new List<NewsArticleViewModel>();
            return await DeserializeListAsync<NewsArticleViewModel>(response);
        }

        public async Task<List<NewsArticleViewModel>> GetActiveAsync()
        {
            var client = CreateClient();
            var response = await client.GetAsync("api/NewsArticles/active");
            if (!response.IsSuccessStatusCode) return new List<NewsArticleViewModel>();
            return await DeserializeListAsync<NewsArticleViewModel>(response);
        }

        public async Task<List<NewsArticleViewModel>> GetMyHistoryAsync(short? accountId = null)
        {
            var client = CreateClient();
            var endpoint = (accountId.HasValue && accountId.Value > 0)
                ? $"api/NewsArticles/my-history/{accountId.Value}"
                : "api/NewsArticles/my-history";

            var response = await client.GetAsync(endpoint);
            if (!response.IsSuccessStatusCode) return new List<NewsArticleViewModel>();
            return await DeserializeListAsync<NewsArticleViewModel>(response);
        }

        public async Task<List<NewsArticleViewModel>> GetReportAsync(DateTime? startDate, DateTime? endDate)
        {
            var client = CreateClient();
            string query = "?";
            if (startDate.HasValue) query += $"startDate={startDate.Value:yyyy-MM-dd}&";
            if (endDate.HasValue) query += $"endDate={endDate.Value:yyyy-MM-dd}";

            var response = await client.GetAsync($"api/NewsArticles/report{query}");
            if (!response.IsSuccessStatusCode) return new List<NewsArticleViewModel>();
            return await DeserializeListAsync<NewsArticleViewModel>(response);
        }

        public async Task<NewsArticleViewModel?> GetByIdAsync(string id)
        {
            var client = CreateClient();
            var response = await client.GetAsync($"api/NewsArticles/{id}");
            if (!response.IsSuccessStatusCode) return null;
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<NewsArticleViewModel>(content, _jsonOptions);
        }

        public async Task<(bool Success, string? ErrorMessage)> CreateAsync(NewsArticleCreateEditViewModel model)
        {
            var client = CreateClient();
            var payload = new
            {
                newsArticleID = model.NewsArticleID,
                newsTitle = model.NewsTitle,
                headline = model.Headline,
                createdDate = model.CreatedDate ?? DateTime.Now,
                newsContent = model.NewsContent,
                newsSource = model.NewsSource,
                categoryID = model.CategoryID,
                newsStatus = model.NewsStatus,
                createdByID = model.CreatedByID,
                updatedByID = model.UpdatedByID,
                tagIDs = model.SelectedTagIDs
            };

            var response = await client.PostAsJsonAsync("api/NewsArticles", payload);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtractErrorMessageAsync(response));
        }

        public async Task<(bool Success, string? ErrorMessage)> UpdateAsync(NewsArticleCreateEditViewModel model)
        {
            var client = CreateClient();
            var payload = new
            {
                newsArticleID = model.NewsArticleID,
                newsTitle = model.NewsTitle,
                headline = model.Headline,
                newsContent = model.NewsContent,
                newsSource = model.NewsSource,
                categoryID = model.CategoryID,
                newsStatus = model.NewsStatus,
                updatedByID = model.UpdatedByID,
                tagIDs = model.SelectedTagIDs
            };

            var response = await client.PutAsJsonAsync($"api/NewsArticles/{model.NewsArticleID}", payload);
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtractErrorMessageAsync(response));
        }

        public async Task<(bool Success, string? ErrorMessage)> DeleteAsync(string id)
        {
            var client = CreateClient();
            var response = await client.DeleteAsync($"api/NewsArticles/{id}");
            if (response.IsSuccessStatusCode) return (true, null);
            return (false, await ExtractErrorMessageAsync(response));
        }
    }
}
