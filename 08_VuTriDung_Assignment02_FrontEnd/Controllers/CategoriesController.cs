using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _08_VuTriDung_Assignment02_FrontEnd.Models;
using _08_VuTriDung_Assignment02_FrontEnd.Services;

namespace _08_VuTriDung_Assignment02_FrontEnd.Controllers
{
    [Authorize(Roles = "Staff")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryApiService _categoryService;

        public CategoriesController(ICategoryApiService categoryService)
        {
            _categoryService = categoryService;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var categories = await _categoryService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                categories = categories.Where(c =>
                    (!string.IsNullOrEmpty(c.CategoryName) && c.CategoryName.ToLower().Contains(s)) ||
                    (!string.IsNullOrEmpty(c.CategoryDesciption) && c.CategoryDesciption.ToLower().Contains(s))
                ).ToList();
            }

            ViewBag.CurrentSearch = search;
            ViewBag.AllCategories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        [HttpGet]
        public async Task<IActionResult> GetCategory(short id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                return NotFound(new { message = "Category not found." });
            }
            return Json(category);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] CategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { success = false, message = errors });
            }

            if (model.CategoryID == 0)
            {
                var result = await _categoryService.CreateAsync(model);
                return Json(new { success = result.Success, message = result.ErrorMessage ?? "Category created successfully!" });
            }
            else
            {
                var result = await _categoryService.UpdateAsync(model);
                return Json(new { success = result.Success, message = result.ErrorMessage ?? "Category updated successfully!" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(short id)
        {
            var result = await _categoryService.DeleteAsync(id);
            return Json(new { success = result.Success, message = result.ErrorMessage ?? "Category deleted successfully!" });
        }
    }
}
