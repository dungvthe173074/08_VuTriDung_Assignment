using _08_VuTriDung_Assignment02.DAOs;
using _08_VuTriDung_Assignment02.DTOs;
using _08_VuTriDung_Assignment02.Models;
using _08_VuTriDung_Assignment02.Services;
using Microsoft.Extensions.Configuration;

namespace _08_VuTriDung_Assignment02.Repositories
{
    public class SystemAccountRepository : ISystemAccountRepository
    {
        private readonly IConfiguration _configuration;
        private readonly IJwtService _jwtService;

        public SystemAccountRepository(IConfiguration configuration, IJwtService jwtService)
        {
            _configuration = configuration;
            _jwtService = jwtService;
        }

        public IEnumerable<SystemAccount> GetAll()
        {
            return SystemAccountDAO.Instance.GetAll();
        }

        public SystemAccount? GetById(short id)
        {
            return SystemAccountDAO.Instance.GetById(id);
        }

        public SystemAccount? GetByEmail(string email)
        {
            return SystemAccountDAO.Instance.GetByEmail(email);
        }

        public SystemAccount Create(SystemAccount account)
        {
            return SystemAccountDAO.Instance.Create(account);
        }

        public SystemAccount? Update(SystemAccount account)
        {
            return SystemAccountDAO.Instance.Update(account);
        }

        public (bool Success, string? ErrorMessage) Delete(short id)
        {
            // Strict constraint: cannot delete if account has created any news articles
            if (SystemAccountDAO.Instance.HasCreatedArticles(id))
            {
                return (false, "Cannot delete account because it has created news articles.");
            }

            bool deleted = SystemAccountDAO.Instance.Delete(id);
            if (!deleted)
            {
                return (false, "Account not found or could not be deleted.");
            }

            return (true, null);
        }

        public LoginResponseDTO? Authenticate(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return null;
            }

            var cleanEmail = email.Trim();
            var cleanPassword = password.Trim();

            // 1. Check Admin Credentials from appsettings.json
            var adminEmail = _configuration["AdminAccount:Email"];
            var adminPassword = _configuration["AdminAccount:Password"];

            if (string.Equals(cleanEmail, adminEmail.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                bool adminPassMatch = string.Equals(cleanPassword, adminPassword.Trim(), StringComparison.Ordinal);

                if (adminPassMatch)
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

                // Admin email with incorrect password
                return null;
            }

            // 2. Check Database for Staff / Lecturer
            try
            {
                var account = SystemAccountDAO.Instance.GetByEmail(cleanEmail);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database access error during authentication: {ex.Message}");
            }

            // Fallback for default seed accounts in case database is starting up
            //var defaultAccounts = new[]
            //{
            //    new { ID = (short)3, Name = "Isabella David", Email = "isabelladavid@funewsmanagement.org", Role = 1, Password = "@1" },
            //    new { ID = (short)4, Name = "Michael Charlotte", Email = "michaelcharlotte@funewsmanagement.org", Role = 1, Password = "@1" },
            //    new { ID = (short)5, Name = "Steve Paris", Email = "steveparis@funewsmanagement.org", Role = 1, Password = "@1" },
            //    new { ID = (short)1, Name = "Emma William", Email = "emmawilliam@funewsmanagement.org", Role = 2, Password = "@1" },
            //    new { ID = (short)2, Name = "Olivia James", Email = "oliviajames@funewsmanagement.org", Role = 2, Password = "@1" }
            //};

            //var fallbackMatch = defaultAccounts.FirstOrDefault(a => string.Equals(a.Email, cleanEmail, StringComparison.OrdinalIgnoreCase));
            //if (fallbackMatch != null)
            //{
            //    bool passwordMatch = string.Equals(fallbackMatch.Password, cleanPassword, StringComparison.Ordinal)
            //        || (fallbackMatch.Password == "@1" && cleanPassword == "@@1");

            //    if (passwordMatch)
            //    {
            //        string roleName = fallbackMatch.Role == 1 ? "Staff" : "Lecturer";
            //        var token = _jwtService.GenerateToken(fallbackMatch.ID, fallbackMatch.Email, roleName, fallbackMatch.Name);
            //        return new LoginResponseDTO
            //        {
            //            Token = token,
            //            Role = roleName,
            //            Email = fallbackMatch.Email,
            //            AccountId = fallbackMatch.ID,
            //            AccountName = fallbackMatch.Name
            //        };
            //    }
            //}

            return null;
        }
    }
}
