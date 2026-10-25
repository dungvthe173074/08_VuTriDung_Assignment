using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _08_VuTriDung_Assignment02_FrontEnd.Models;
using _08_VuTriDung_Assignment02_FrontEnd.Services;

namespace _08_VuTriDung_Assignment02_FrontEnd.Controllers
{
    [Authorize(Roles = "Staff")]
    public class NewsArticlesController : Controller
    {
        private readonly INewsArticleApiService _newsArticleService;
        private readonly ICategoryApiService _categoryService;
        private readonly ITagApiService _tagService;

        public NewsArticlesController(
            INewsArticleApiService newsArticleService,
            ICategoryApiService categoryService,
            ITagApiService tagService)
        {
            _newsArticleService = newsArticleService;
            _categoryService = categoryService;
            _tagService = tagService;
        }

        private short GetCurrentUserId()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return short.TryParse(idClaim, out var id) ? id : (short)1;
        }

        public async Task<IActionResult> Index(string? search, short? categoryId, bool? status)
        {
            var articles = await _newsArticleService.GetAllAsync();
            var categories = await _categoryService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                articles = articles.Where(a =>
                    (!string.IsNullOrEmpty(a.NewsTitle) && a.NewsTitle.ToLower().Contains(s)) ||
                    (!string.IsNullOrEmpty(a.Headline) && a.Headline.ToLower().Contains(s)) ||
                    (!string.IsNullOrEmpty(a.NewsContent) && a.NewsContent.ToLower().Contains(s)) ||
                    (!string.IsNullOrEmpty(a.NewsSource) && a.NewsSource.ToLower().Contains(s))
                ).ToList();
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                articles = articles.Where(a => a.CategoryID == categoryId.Value).ToList();
            }

            if (status.HasValue)
            {
                articles = articles.Where(a => a.NewsStatus == status.Value).ToList();
            }

            ViewBag.Categories = categories;
            ViewBag.CurrentSearch = search;
            ViewBag.CurrentCategory = categoryId;
            ViewBag.CurrentStatus = status;

            return View(articles);
        }

        public async Task<IActionResult> History()
        {
            var articles = await _newsArticleService.GetMyHistoryAsync();
            return View(articles);
        }

        [HttpGet]
        public async Task<IActionResult> GetFormData(string? id)
        {
            var categories = await _categoryService.GetAllAsync();
            var tags = await _tagService.GetAllAsync();

            NewsArticleViewModel? article = null;
            if (!string.IsNullOrEmpty(id))
            {
                article = await _newsArticleService.GetByIdAsync(id);
            }

            return Json(new
            {
                categories,
                tags,
                article
            });
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] NewsArticleCreateEditViewModel model, [FromQuery] bool isEdit = false)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { success = false, message = errors });
            }

            short userId = GetCurrentUserId();
            model.UpdatedByID = userId;

            if (isEdit)
            {
                var result = await _newsArticleService.UpdateAsync(model);
                return Json(new { success = result.Success, message = result.ErrorMessage ?? "News article updated successfully!" });
            }
            else
            {
                model.CreatedByID = userId;
                var result = await _newsArticleService.CreateAsync(model);
                return Json(new { success = result.Success, message = result.ErrorMessage ?? "News article created successfully!" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _newsArticleService.DeleteAsync(id);
            return Json(new { success = result.Success, message = result.ErrorMessage ?? "News article deleted successfully!" });
        }
    }
}
