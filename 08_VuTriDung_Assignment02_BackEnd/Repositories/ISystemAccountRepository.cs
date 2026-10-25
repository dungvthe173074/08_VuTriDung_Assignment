using _08_VuTriDung_Assignment02.DTOs;
using _08_VuTriDung_Assignment02.Models;

namespace _08_VuTriDung_Assignment02.Repositories
{
    public interface ISystemAccountRepository
    {
        IEnumerable<SystemAccount> GetAll();
        SystemAccount? GetById(short id);
        SystemAccount? GetByEmail(string email);
        SystemAccount Create(SystemAccount account);
        SystemAccount? Update(SystemAccount account);
        (bool Success, string? ErrorMessage) Delete(short id);
        LoginResponseDTO? Authenticate(string email, string password);
    }
}
