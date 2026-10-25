using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using _08_VuTriDung_Assignment02_FrontEnd.Models;
using _08_VuTriDung_Assignment02_FrontEnd.Services;

namespace _08_VuTriDung_Assignment02_FrontEnd.Controllers
{
    public class HomeController : Controller
    {
        private readonly INewsArticleApiService _newsArticleService;
        private readonly ICategoryApiService _categoryService;

        public HomeController(INewsArticleApiService newsArticleService, ICategoryApiService categoryService)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(string? search, short? categoryId)
        {
            var articles = await _newsArticleService.GetActiveAsync();
            var categories = await _categoryService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                articles = articles.Where(a => 
                    (!string.IsNullOrEmpty(a.NewsTitle) && a.NewsTitle.ToLower().Contains(s)) ||
                    (!string.IsNullOrEmpty(a.Headline) && a.Headline.ToLower().Contains(s)) ||
                    (!string.IsNullOrEmpty(a.NewsContent) && a.NewsContent.ToLower().Contains(s))
                ).ToList();
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                articles = articles.Where(a => a.CategoryID == categoryId.Value).ToList();
            }

            ViewBag.Categories = categories.Where(c => c.IsActive).ToList();
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentCategory = categoryId;

            return View(articles);
        }

        public async Task<IActionResult> Detail(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var article = await _newsArticleService.GetByIdAsync(id);
            if (article == null) return NotFound();

            return View(article);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
