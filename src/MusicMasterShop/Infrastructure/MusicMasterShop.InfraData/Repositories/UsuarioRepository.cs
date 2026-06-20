using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.InfraData.Context;

namespace MusicMasterShop.InfraData.Repositories
{
    public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
    {
        public UsuarioRepository(EFContext context) : base(context)
        {
        }
    }
}
