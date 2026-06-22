using MediatR;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Entities;

namespace MusicMasterShop.Application.Queries.ListCategories
{
    public sealed class ListCategoriesQueryHandler : IRequestHandler<ListCategoriesRequest, BaseResponse<IEnumerable<ListCategoriesResponse>>>
    {
        private readonly ICategoriaRepository _categoryRepository;
        public ListCategoriesQueryHandler(ICategoriaRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }
        public async Task<BaseResponse<IEnumerable<ListCategoriesResponse>>> Handle(ListCategoriesRequest request, CancellationToken cancellationToken)
        {
            var entity = await _categoryRepository.GetAll(cancellationToken);

            var list = entity.Select(Mapping).AsEnumerable();

            return ResponseWrapper.Success(list);
        }

        private static ListCategoriesResponse Mapping(Categoria categoria)
        {
            return new ListCategoriesResponse(
                categoria.Nome,
                (int)categoria.Tipo);
        }
    }
}
