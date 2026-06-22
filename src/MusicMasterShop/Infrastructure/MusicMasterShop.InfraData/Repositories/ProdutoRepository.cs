using Microsoft.EntityFrameworkCore;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Core.Pagination;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.InfraData.Context;

namespace MusicMasterShop.InfraData.Repositories
{
    public class ProdutoRepository : BaseRepository<Produto>, IProdutoRepository
    {
        public ProdutoRepository(EFContext context) : base(context)
        {
        }

        public Task<Produto?> GetWithDetailsAsync(Guid id, CancellationToken cancellationToken)
        {
            return Context.Produtos
                .Include(produto => produto.Categoria)
                .FirstOrDefaultAsync(produto => produto.Id == id, cancellationToken);
        }

        public async Task<PagedResult<Produto>> GetAllPagedWithDetailsAsync(
            int pageSize,
            int pageNumber,
            CancellationToken cancellationToken)
        {
            IQueryable<Produto> query = Context.Produtos
                .AsNoTracking()
                .Include(produto => produto.Categoria)
                .OrderBy(produto => produto.CreatedAt);

            return new PagedResult<Produto>
            {
                Items = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync(cancellationToken),
                CurrentPage = pageNumber,
                PageSize = pageSize,
                TotalCount = await query.CountAsync(cancellationToken)
            };
        }
    }
}
