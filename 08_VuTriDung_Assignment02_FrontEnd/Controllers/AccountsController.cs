using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _08_VuTriDung_Assignment02_FrontEnd.Models;
using _08_VuTriDung_Assignment02_FrontEnd.Services;

namespace _08_VuTriDung_Assignment02_FrontEnd.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AccountsController : Controller
    {
        private readonly IAccountApiService _accountService;

        public AccountsController(IAccountApiService accountService)
        {
            _accountService = accountService;
        }

        public async Task<IActionResult> Index(string? search)
        {
            var accounts = await _accountService.GetAllAsync();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var s = search.Trim().ToLower();
                accounts = accounts.Where(a =>
                    (!string.IsNullOrEmpty(a.AccountName) && a.AccountName.ToLower().Contains(s)) ||
                    (!string.IsNullOrEmpty(a.AccountEmail) && a.AccountEmail.ToLower().Contains(s))
                ).ToList();
            }

            ViewBag.CurrentSearch = search;
            return View(accounts);
        }

        [HttpGet]
        public async Task<IActionResult> GetAccount(short id)
        {
            var account = await _accountService.GetByIdAsync(id);
            if (account == null)
            {
                return NotFound(new { message = "Account not found." });
            }
            return Json(account);
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] AccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join(" ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
                return Json(new { success = false, message = errors });
            }

            if (model.AccountID == 0)
            {
                var result = await _accountService.CreateAsync(model);
                return Json(new { success = result.Success, message = result.ErrorMessage ?? "Account created successfully!" });
            }
            else
            {
                var result = await _accountService.UpdateAsync(model);
                return Json(new { success = result.Success, message = result.ErrorMessage ?? "Account updated successfully!" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(short id)
        {
            var result = await _accountService.DeleteAsync(id);
            return Json(new { success = result.Success, message = result.ErrorMessage ?? "Account deleted successfully!" });
        }
    }
}
