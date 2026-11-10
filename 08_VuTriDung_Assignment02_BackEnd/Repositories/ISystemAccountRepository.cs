using _08_VuTriDung_Assignment02.DTOs;
using _08_VuTriDung_Assignment02.Models;

namespace _08_VuTriDung_Assignment02.Repositories
{
    public interface ISystemAccountRepository
    {
        IQueryable<SystemAccount> GetAll();
        SystemAccount? GetById(short id);
        SystemAccount? GetByEmail(string email);
        bool EmailExists(string email, short? excludeId = null);
        SystemAccount Create(SystemAccount account);
        SystemAccount? UpdateProfile(short id, string? name, string? email, string? password);
        SystemAccount? UpdateAccount(short id, string? name, string? email, string? password, int role);
        (bool Success, string? ErrorMessage) Delete(short id);
        LoginResponseDTO? Authenticate(string email, string password);
    }
}
