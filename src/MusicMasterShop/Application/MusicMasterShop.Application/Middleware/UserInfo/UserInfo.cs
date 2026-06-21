using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.Middleware.UserInfo;

public sealed class UserInfo(IHttpContextAccessor httpContextAccessor) : IUserInfo
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public bool IsAuthenticated => User?.Identity?.IsAuthenticated == true;

    public Guid? Id
    {
        get
        {
            string? value = User?.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User?.FindFirstValue("sub");

            return Guid.TryParse(value, out Guid id) ? id : null;
        }
    }

    public string? Email => User?.FindFirstValue(ClaimTypes.Email)
        ?? User?.FindFirstValue("email");

    public string? Nome => User?.FindFirstValue(ClaimTypes.Name);

    public TipoUsuario? TipoUsuario
    {
        get
        {
            string? value = User?.FindFirstValue(ClaimTypes.Role);

            return Enum.TryParse(value, ignoreCase: true, out TipoUsuario tipoUsuario)
                ? tipoUsuario
                : null;
        }
    }

    public bool IsAdministrator =>
        TipoUsuario == MusicMasterShop.Domain.Enums.TipoUsuario.Administrador;
}
