using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using _08_VuTriDung_Assignment02.DTOs;
using _08_VuTriDung_Assignment02.Models;
using _08_VuTriDung_Assignment02.Repositories;

namespace _08_VuTriDung_Assignment02.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ODataController
    {
        private readonly ISystemAccountRepository _accountRepository;

        public AccountsController(ISystemAccountRepository accountRepository)
        {
            _accountRepository = accountRepository;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        [EnableQuery]
        public IActionResult Get()
        {
            var accounts = _accountRepository.GetAll();
            return Ok(accounts);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "Admin")]
        [EnableQuery]
        public IActionResult Get([FromRoute] short id)
        {
            var account = _accountRepository.GetById(id);
            if (account == null)
            {
                return NotFound(new { message = $"Account with ID {id} not found." });
            }
            return Ok(account);
        }

        [HttpGet("profile")]
        [Authorize(Roles = "Staff,Lecturer,Admin")]
        public IActionResult GetProfile()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!short.TryParse(idClaim, out var accountId) || accountId <= 0)
            {
                return Unauthorized(new { message = "Invalid token claims." });
            }

            var account = _accountRepository.GetById(accountId);
            if (account == null)
            {
                return NotFound(new { message = "Profile not found." });
            }

            return Ok(account);
        }

        [HttpPut("profile")]
        [Authorize(Roles = "Staff,Lecturer,Admin")]
        public IActionResult UpdateProfile([FromBody] SystemAccountDTO dto)
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!short.TryParse(idClaim, out var accountId) || accountId <= 0)
            {
                return Unauthorized(new { message = "Invalid token claims." });
            }

            var existing = _accountRepository.GetById(accountId);
            if (existing == null)
            {
                return NotFound(new { message = "Profile not found." });
            }

            if (!string.IsNullOrWhiteSpace(dto.AccountEmail))
            {
                var cleanEmail = dto.AccountEmail.Trim();
                if (_accountRepository.EmailExists(cleanEmail, accountId))
                {
                    return BadRequest(new { message = "Email already in use." });
                }
            }

            var updated = _accountRepository.UpdateProfile(accountId, dto.AccountName, dto.AccountEmail, dto.AccountPassword);
            return Ok(updated);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult Post([FromBody] SystemAccountDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cleanEmail = dto.AccountEmail.Trim();
            if (_accountRepository.EmailExists(cleanEmail))
            {
                return BadRequest(new { message = "Email already exists in the system." });
            }

            var account = new SystemAccount
            {
                AccountID = dto.AccountID,
                AccountName = dto.AccountName,
                AccountEmail = cleanEmail,
                AccountRole = dto.AccountRole,
                AccountPassword = dto.AccountPassword ?? "@1"
            };

            var created = _accountRepository.Create(account);
            return CreatedAtAction(nameof(Get), new { id = created.AccountID }, created);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Put([FromRoute] short id, [FromBody] SystemAccountDTO dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var existing = _accountRepository.GetById(id);
            if (existing == null)
            {
                return NotFound(new { message = $"Account with ID {id} not found." });
            }

            var cleanEmail = dto.AccountEmail.Trim();
            if (_accountRepository.EmailExists(cleanEmail, id))
            {
                return BadRequest(new { message = "Email already in use by another account." });
            }

            var updated = _accountRepository.UpdateAccount(id, dto.AccountName, cleanEmail, dto.AccountPassword, dto.AccountRole);
            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete([FromRoute] short id)
        {
            var result = _accountRepository.Delete(id);
            if (!result.Success)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(new { message = "Account deleted successfully." });
        }
    }
}
