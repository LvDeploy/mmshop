using MusicMasterShop.Domain.Contracts.Repositories.Base;
using MusicMasterShop.Domain.Entities;

namespace MusicMasterShop.Domain.Contracts.Repositories
{
    public interface IUsuarioRepository : IBaseRepository<Usuario>
    {
        Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken);
    }
}
