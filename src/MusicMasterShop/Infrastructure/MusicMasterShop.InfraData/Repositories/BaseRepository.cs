using Microsoft.EntityFrameworkCore;
using MusicMasterShop.Domain.Contracts.Repositories.Base;
using MusicMasterShop.Domain.Core.Pagination;
using MusicMasterShop.Domain.Entities.Base;
using MusicMasterShop.InfraData.Context;

namespace MusicMasterShop.InfraData.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly EFContext Context;

        public BaseRepository(EFContext context)
        {
            Context = context;
        }
        public void Create(T entity)
        {
            Context.Add(entity);
        }

        public void Delete(T entity)
        {
            Context.Remove(entity);
        }

        public async Task<T> Get(Guid id, CancellationToken cancellationToken)
        {
            return await Context.Set<T>().FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        }

        public async Task<List<T>> GetAll(CancellationToken cancellationToken)
        {
            return await Context.Set<T>().ToListAsync(cancellationToken);
        }

        public void Update(T entity)
        {
            Context.Update(entity);
        }
    }
}
