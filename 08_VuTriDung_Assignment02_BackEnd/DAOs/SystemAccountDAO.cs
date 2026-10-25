using Microsoft.EntityFrameworkCore;
using _08_VuTriDung_Assignment02.Models;

namespace _08_VuTriDung_Assignment02.DAOs
{
    public class SystemAccountDAO
    {
        private static SystemAccountDAO? _instance;
        private static readonly object _instanceLock = new object();

        private SystemAccountDAO() { }

        public static SystemAccountDAO Instance
        {
            get
            {
                lock (_instanceLock)
                {
                    if (_instance == null)
                    {
                        _instance = new SystemAccountDAO();
                    }
                    return _instance;
                }
            }
        }

        public List<SystemAccount> GetAll()
        {
            using var db = new FUNewsManagementDbContext();
            return db.SystemAccounts.AsNoTracking().ToList();
        }

        public SystemAccount? GetById(short id)
        {
            using var db = new FUNewsManagementDbContext();
            return db.SystemAccounts.AsNoTracking().FirstOrDefault(a => a.AccountID == id);
        }

        public SystemAccount? GetByEmail(string email)
        {
            using var db = new FUNewsManagementDbContext();
            return db.SystemAccounts.AsNoTracking()
                .FirstOrDefault(a => a.AccountEmail != null && a.AccountEmail.ToLower() == email.Trim().ToLower());
        }

        public SystemAccount Create(SystemAccount account)
        {
            using var db = new FUNewsManagementDbContext();
            if (account.AccountID == 0)
            {
                short maxId = db.SystemAccounts.Any() ? db.SystemAccounts.Max(a => a.AccountID) : (short)0;
                account.AccountID = (short)(maxId + 1);
            }
            db.SystemAccounts.Add(account);
            db.SaveChanges();
            return account;
        }

        public SystemAccount? Update(SystemAccount account)
        {
            using var db = new FUNewsManagementDbContext();
            var existing = db.SystemAccounts.FirstOrDefault(a => a.AccountID == account.AccountID);
            if (existing == null) return null;

            existing.AccountName = account.AccountName;
            existing.AccountEmail = account.AccountEmail;
            existing.AccountRole = account.AccountRole;
            if (!string.IsNullOrEmpty(account.AccountPassword))
            {
                existing.AccountPassword = account.AccountPassword;
            }

            db.SaveChanges();
            return existing;
        }

        public bool HasCreatedArticles(short accountId)
        {
            using var db = new FUNewsManagementDbContext();
            return db.NewsArticles.Any(n => n.CreatedByID == accountId);
        }

        public bool Delete(short id)
        {
            using var db = new FUNewsManagementDbContext();
            var existing = db.SystemAccounts.FirstOrDefault(a => a.AccountID == id);
            if (existing == null) return false;

            db.SystemAccounts.Remove(existing);
            db.SaveChanges();
            return true;
        }
    }
}
