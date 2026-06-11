using CashFlow.Domain.Security.Cryptography;
using BC = BCrypt.Net.BCrypt;

namespace CashFlow.Infrastructure.Security.Cryptography;

internal class BCrypt : IPasswordEncrypter
{
    public string Encrypt(string password)
    {
        return BC.HashPassword(password);
    }

    public bool Verify(string password, string passwordHash)
    {
        return BC.Verify(password, passwordHash);
    }
}
