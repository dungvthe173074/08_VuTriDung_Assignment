using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _08_VuTriDung_Assignment02_FrontEnd.Models;
using _08_VuTriDung_Assignment02_FrontEnd.Services;

namespace _08_VuTriDung_Assignment02_FrontEnd.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly INewsArticleApiService _newsArticleService;

        public ReportController(INewsArticleApiService newsArticleService)
        {
            _newsArticleService = newsArticleService;
        }

        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            var articles = await _newsArticleService.GetReportAsync(startDate, endDate);

            var model = new ReportFilterViewModel
            {
                StartDate = startDate,
                EndDate = endDate,
                Articles = articles
            };

            return View(model);
        }
    }
}
