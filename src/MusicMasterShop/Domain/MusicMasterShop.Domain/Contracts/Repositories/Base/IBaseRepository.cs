using MusicMasterShop.Domain.Core.Pagination;
using MusicMasterShop.Domain.Entities.Base;

namespace MusicMasterShop.Domain.Contracts.Repositories.Base
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<T> Get(Guid id, CancellationToken cancellationToken);
        Task<List<T>> GetAll(CancellationToken cancellationToken);
    }
}
