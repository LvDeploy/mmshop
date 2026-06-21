using MusicMasterShop.Domain.Contracts.Repositories.Base;
using MusicMasterShop.Domain.Core.Pagination;
using MusicMasterShop.Domain.Entities;

namespace MusicMasterShop.Domain.Contracts.Repositories
{
    public interface IProdutoRepository : IBaseRepository<Produto>
    {
        Task<Produto?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken);
        Task<PagedResult<Produto>> GetAllPagedWithDetailsAsync(
            int pageSize,
            int pageNumber,
            CancellationToken cancellationToken);
    }
}
