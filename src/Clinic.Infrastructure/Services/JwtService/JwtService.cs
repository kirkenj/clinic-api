using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Clinic.Infrastructure.Services.JwtService;

public sealed class JwtService(IOptions<JwtOptions> jwtOptions) : IJwtService
{
    public string GenerateToken(uint memberId, RoleType roleTypes)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Role, roleTypes.ToString()),
            new(ClaimTypes.NameIdentifier, memberId.ToString())
        };


        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(jwtOptions.Value.Key));
        SigningCredentials signingCredentials = new(key, jwtOptions.Value.SecurityAlgorithm);

        var jwtToken = new JwtSecurityToken
        (
            jwtOptions.Value.Issuer,
            jwtOptions.Value.Audience,
            claims,
            expires: DateTime.UtcNow.AddMinutes(jwtOptions.Value.DurationInMinutes),
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(jwtToken);
    }
}