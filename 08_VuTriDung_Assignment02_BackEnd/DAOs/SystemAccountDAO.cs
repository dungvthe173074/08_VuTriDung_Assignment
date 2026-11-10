using Microsoft.EntityFrameworkCore;
using _08_VuTriDung_Assignment02.Models;

namespace _08_VuTriDung_Assignment02.DAOs
{
    public class SystemAccountDAO
    {
        private readonly FUNewsManagementDbContext _context;

        public SystemAccountDAO(FUNewsManagementDbContext context)
        {
            _context = context;
        }

        public IQueryable<SystemAccount> GetAll()
        {
            return _context.SystemAccounts.AsNoTracking();
        }

        public SystemAccount? GetById(short id, bool tracking = false)
        {
            var query = tracking ? _context.SystemAccounts.AsQueryable() : _context.SystemAccounts.AsNoTracking();
            return query.FirstOrDefault(a => a.AccountID == id);
        }

        public SystemAccount? GetByEmail(string email)
        {
            var clean = email.Trim().ToLower();
            return _context.SystemAccounts.AsNoTracking()
                .FirstOrDefault(a => a.AccountEmail != null && a.AccountEmail.ToLower() == clean);
        }

        public bool EmailExists(string email, short? excludeId = null)
        {
            var clean = email.Trim().ToLower();
            return _context.SystemAccounts.Any(a => a.AccountEmail != null && 
                                                    a.AccountEmail.ToLower() == clean && 
                                                    (!excludeId.HasValue || a.AccountID != excludeId.Value));
        }

        public SystemAccount Create(SystemAccount account)
        {
            if (account.AccountID == 0)
            {
                short maxId = _context.SystemAccounts.Any() ? _context.SystemAccounts.Max(a => a.AccountID) : (short)0;
                account.AccountID = (short)(maxId + 1);
            }
            _context.SystemAccounts.Add(account);
            _context.SaveChanges();
            return account;
        }

        public SystemAccount? Update(short id, string? name, string? email, string? password, int? role = null)
        {
            var existing = _context.SystemAccounts.FirstOrDefault(a => a.AccountID == id);
            if (existing == null) return null;

            if (!string.IsNullOrWhiteSpace(name))
            {
                existing.AccountName = name;
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                existing.AccountEmail = email.Trim();
            }

            if (!string.IsNullOrEmpty(password))
            {
                existing.AccountPassword = password;
            }

            if (role.HasValue)
            {
                existing.AccountRole = role.Value;
            }

            _context.SaveChanges();
            return existing;
        }

        public bool HasCreatedArticles(short accountId)
        {
            return _context.NewsArticles.Any(n => n.CreatedByID == accountId);
        }

        public (bool Success, string? ErrorMessage) Delete(short id)
        {
            var existing = _context.SystemAccounts.FirstOrDefault(a => a.AccountID == id);
            if (existing == null)
            {
                return (false, $"Account with ID {id} not found.");
            }

            if (HasCreatedArticles(id))
            {
                return (false, "Cannot delete account because it has created news articles.");
            }

            _context.SystemAccounts.Remove(existing);
            _context.SaveChanges();
            return (true, null);
        }
    }
}
