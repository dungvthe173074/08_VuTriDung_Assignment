using _08_VuTriDung_Assignment02.DAOs;
using _08_VuTriDung_Assignment02.DTOs;
using _08_VuTriDung_Assignment02.Models;
using _08_VuTriDung_Assignment02.Services;
using Microsoft.Extensions.Configuration;

namespace _08_VuTriDung_Assignment02.Repositories
{
    public class SystemAccountRepository : ISystemAccountRepository
    {
        private readonly SystemAccountDAO _accountDAO;
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;

        public SystemAccountRepository(SystemAccountDAO accountDAO, IConfiguration configuration, IJwtService jwtService)
        {
            _accountDAO = accountDAO;
            _configuration = configuration;
            _jwtService = jwtService;
        }

        public IQueryable<SystemAccount> GetAll() => _accountDAO.GetAll();

        public SystemAccount? GetById(short id) => _accountDAO.GetById(id);

        public SystemAccount? GetByEmail(string email) => _accountDAO.GetByEmail(email);

        public bool EmailExists(string email, short? excludeId = null) => _accountDAO.EmailExists(email, excludeId);

        public SystemAccount Create(SystemAccount account) => _accountDAO.Create(account);

        public SystemAccount? UpdateProfile(short id, string? name, string? email, string? password)
        {
            return _accountDAO.Update(id, name, email, password);
        }

        public SystemAccount? UpdateAccount(short id, string? name, string? email, string? password, int role)
        {
            return _accountDAO.Update(id, name, email, password, role);
        }

        public (bool Success, string? ErrorMessage) Delete(short id) => _accountDAO.Delete(id);

        public LoginResponseDTO? Authenticate(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var cleanEmail = email.Trim();
            var cleanPassword = password.Trim();

            // 1. Check Admin Credentials from appsettings.json
            var adminEmail = _configuration["AdminAccount:Email"] ?? "";
            var adminPassword = _configuration["AdminAccount:Password"] ?? "";

            if (!string.IsNullOrEmpty(adminEmail) && string.Equals(cleanEmail, adminEmail.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                if (string.Equals(cleanPassword, adminPassword.Trim(), StringComparison.Ordinal))
                {
                    var token = _jwtService.GenerateToken(0, adminEmail.Trim(), "Admin", "Administrator");
                    return new LoginResponseDTO
                    {
                        Token = token,
                        Role = "Admin",
                        Email = adminEmail.Trim(),
                        AccountId = 0,
                        AccountName = "Administrator"
                    };
                }

                return null;
            }

            // 2. Check Database for Staff / Lecturer via SystemAccountDAO
            var account = _accountDAO.GetByEmail(cleanEmail);
            if (account != null)
            {
                bool passwordMatch = string.Equals(account.AccountPassword?.Trim(), cleanPassword, StringComparison.Ordinal);
                if (passwordMatch)
                {
                    string? roleName = account.AccountRole switch
                    {
                        1 => "Staff",
                        2 => "Lecturer",
                        _ => null
                    };

                    if (string.IsNullOrEmpty(roleName))
                    {
                        return null;
                    }

                    var accountName = string.IsNullOrWhiteSpace(account.AccountName) ? (account.AccountEmail ?? "User") : account.AccountName;
                    var accountEmail = account.AccountEmail ?? cleanEmail;
                    var token = _jwtService.GenerateToken(account.AccountID, accountEmail, roleName, accountName);

                    return new LoginResponseDTO
                    {
                        Token = token,
                        Role = roleName,
                        Email = accountEmail,
                        AccountId = account.AccountID,
                        AccountName = accountName
                    };
                }
            }

            return null;
        }
    }
}
