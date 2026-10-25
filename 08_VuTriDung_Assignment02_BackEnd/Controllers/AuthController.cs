using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using _08_VuTriDung_Assignment02.DTOs;
using _08_VuTriDung_Assignment02.Repositories;

namespace _08_VuTriDung_Assignment02.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ISystemAccountRepository _accountRepository;

        public AuthController(ISystemAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public IActionResult Login([FromBody] LoginRequestDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = _accountRepository.Authenticate(request.Email, request.Password);
            if (result == null)
            {
                return Unauthorized(new { message = "You do not have permission or invalid credentials!" });
            }

            return Ok(result);
        }
    }
}
