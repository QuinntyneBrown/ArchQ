using ArchQ.Core.Interfaces;

namespace ArchQ.Infrastructure.Identity;

public class PasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12;

    public string Hash(string plaintext)
    {
        return BCrypt.Net.BCrypt.HashPassword(plaintext, WorkFactor);
    }

    public bool Verify(string plaintext, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(plaintext, hash);
    }
}
