using SimpleCrud.Models;

namespace SimpleCrud.Services.JwtService
{
    public interface IJwtService
    {
        (byte[] passwordSalt, byte[] passwordHash) CreatePasswordHash(string password);
        bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt);
        string CreatedToken(User user);
        RefreshToken GenerateRefreshToken();
        string CreateRandomToken();
    }
}
