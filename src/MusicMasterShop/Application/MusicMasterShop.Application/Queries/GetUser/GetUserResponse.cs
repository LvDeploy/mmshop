using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.Queries.GetUser
{
    public record GetUserResponse(Guid Id, string Email, string Nome, TipoUsuario TipoUsuario)
    {
    }
}
