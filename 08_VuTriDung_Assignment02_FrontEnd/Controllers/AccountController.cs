using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using _08_VuTriDung_Assignment02_FrontEnd.Models;
using _08_VuTriDung_Assignment02_FrontEnd.Services;

namespace _08_VuTriDung_Assignment02_FrontEnd.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthApiService _authService;

        public AccountController(IAuthApiService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.ReturnUrl = returnUrl;
            return View(new LoginViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.Email = model.Email?.Trim() ?? string.Empty;
            model.Password = model.Password?.Trim() ?? string.Empty;

            var user = await _authService.LoginAsync(model);
            if (user == null || string.IsNullOrEmpty(user.Token))
            {
                model.ErrorMessage = "You do not have permission or invalid credentials!";
                return View(model);
            }

            // Store in Session
            HttpContext.Session.SetString("JWToken", user.Token);
            HttpContext.Session.SetString("UserEmail", user.AccountEmail);
            HttpContext.Session.SetString("UserName", user.AccountName);
            HttpContext.Session.SetString("UserRole", user.RoleName);
            HttpContext.Session.SetString("UserId", user.AccountID.ToString());

            // Create Claims and Cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.AccountID.ToString()),
                new Claim(ClaimTypes.Name, string.IsNullOrWhiteSpace(user.AccountName) ? user.AccountEmail : user.AccountName),
                new Claim(ClaimTypes.Email, user.AccountEmail),
                new Claim(ClaimTypes.Role, user.RoleName),
                new Claim("Token", user.Token)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Redirect based on role
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            if (user.RoleName == "Admin")
            {
                return RedirectToAction("Index", "Accounts");
            }
            else if (user.RoleName == "Staff")
            {
                return RedirectToAction("Index", "NewsArticles");
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
