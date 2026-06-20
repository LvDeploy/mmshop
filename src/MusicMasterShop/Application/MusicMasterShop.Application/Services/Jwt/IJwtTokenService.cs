using MusicMasterShop.Domain.Entities;

namespace MusicMasterShop.Application.Services.Jwt;

public interface IJwtTokenService
{
    JwtTokenResult GenerateToken(Usuario usuario);
}
