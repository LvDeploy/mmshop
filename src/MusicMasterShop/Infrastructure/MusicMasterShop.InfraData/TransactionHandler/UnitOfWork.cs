using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.InfraData.Context;
using System.Runtime.InteropServices;

namespace MusicMasterShop.InfraData.TransactionHandler
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly EFContext _context;

        public UnitOfWork(EFContext context)
        {
            _context = context;
        }

        public async Task CommitAsync(CancellationToken cancellationToken) 
        {
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
