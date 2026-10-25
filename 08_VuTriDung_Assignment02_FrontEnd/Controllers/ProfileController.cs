using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _08_VuTriDung_Assignment02_FrontEnd.Models;
using _08_VuTriDung_Assignment02_FrontEnd.Services;

namespace _08_VuTriDung_Assignment02_FrontEnd.Controllers
{
    [Authorize(Roles = "Staff,Lecturer")]
    public class ProfileController : Controller
    {
        private readonly IAccountApiService _accountService;

        public ProfileController(IAccountApiService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var account = await _accountService.GetProfileAsync();
            if (account == null)
            {
                return NotFound("Profile not found or access denied.");
            }

            var model = new ProfileViewModel
            {
                AccountID = account.AccountID,
                AccountName = account.AccountName,
                AccountEmail = account.AccountEmail,
                AccountRole = account.AccountRole,
                RoleName = account.AccountRole == 1 ? "Staff" : "Lecturer"
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var account = await _accountService.GetProfileAsync();
            if (account == null)
            {
                return NotFound();
            }

            account.AccountName = model.AccountName;
            account.AccountEmail = model.AccountEmail;
            if (!string.IsNullOrWhiteSpace(model.NewPassword))
            {
                account.AccountPassword = model.NewPassword;
            }

            var result = await _accountService.UpdateProfileAsync(account);
            if (!result.Success)
            {
                ModelState.AddModelError("", result.ErrorMessage ?? "Error updating profile.");
                return View(model);
            }

            TempData["SuccessMessage"] = "Profile updated successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
