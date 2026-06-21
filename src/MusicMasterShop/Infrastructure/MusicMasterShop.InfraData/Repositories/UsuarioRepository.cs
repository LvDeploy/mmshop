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

        public Task<Usuario?> GetByEmailAsync(string email, CancellationToken cancellationToken)
        {
            string normalizedEmail = email.Trim().ToUpperInvariant();

            return Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
                Context.Usuarios,
                usuario => usuario.Email.ToUpper() == normalizedEmail,
                cancellationToken);
        }
    }
}
