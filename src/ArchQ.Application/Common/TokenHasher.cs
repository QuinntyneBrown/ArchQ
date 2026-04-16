using System.Security.Cryptography;
using System.Text;

namespace ArchQ.Application.Common;

public static class TokenHasher
{
    public static string ComputeSha256(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexStringLower(bytes);
    }
}
