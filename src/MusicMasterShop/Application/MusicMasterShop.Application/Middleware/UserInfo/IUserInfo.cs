using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.Middleware.UserInfo;

public interface IUserInfo
{
    bool IsAuthenticated { get; }
    Guid? Id { get; }
    string? Email { get; }
    string? Nome { get; }
    TipoUsuario? TipoUsuario { get; }
    bool IsAdministrator { get; }
}
