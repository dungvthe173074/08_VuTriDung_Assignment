namespace _08_VuTriDung_Assignment02.Services
{
    public interface IJwtService
    {
        string GenerateToken(short accountId, string email, string role, string accountName);
    }
}
