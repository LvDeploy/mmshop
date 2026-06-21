using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MusicMasterShop.Domain.Entities;

namespace MusicMasterShop.Application.Services.Jwt;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly string _issuer;
    private readonly string _audience;
    private readonly SymmetricSecurityKey _signingKey;
    private readonly int _expirationMinutes;

    public JwtTokenService(IConfiguration configuration)
    {
        _issuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT configuration 'Jwt:Issuer' was not provided.");
        _audience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT configuration 'Jwt:Audience' was not provided.");

        string key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT configuration 'Jwt:Key' was not provided.");

        if (!int.TryParse(configuration["Jwt:ExpirationMinutes"], out _expirationMinutes)
            || _expirationMinutes <= 0)
        {
            throw new InvalidOperationException(
                "JWT configuration 'Jwt:ExpirationMinutes' must be a positive integer.");
        }

        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    }

    public JwtTokenResult GenerateToken(Usuario usuario)
    {
        DateTime issuedAt = DateTime.UtcNow;
        DateTime expiresAt = issuedAt.AddMinutes(_expirationMinutes);

        Claim[] claims =
        [
            new(JwtRegisteredClaimNames.Sub, usuario.Id.ToString()),
            new(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, usuario.Email),
            new(ClaimTypes.Name, usuario.Nome),
            new(ClaimTypes.Role, usuario.Tipo.ToString()),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            notBefore: issuedAt,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtTokenResult(new JwtSecurityTokenHandler().WriteToken(token), expiresAt);
    }
}
