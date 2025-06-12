using System.Security.Cryptography;
using System.Text;
using Clinic.Application.Abstractions;

namespace Clinic.Infrastructure.Services.HashService;

public sealed class HashService : IHashService
{
    public string GetHash(string hash)
    {
        var bytes = Encoding.UTF8.GetBytes(hash);
        var hashedBytes = MD5.HashData(bytes);
        return Encoding.UTF8.GetString(hashedBytes);
    }
}