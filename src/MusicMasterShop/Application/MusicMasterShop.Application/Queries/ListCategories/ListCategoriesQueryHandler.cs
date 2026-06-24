using MediatR;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.Domain.Enums;

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
            try
            {
                var entity = await _categoryRepository.GetAll(cancellationToken);

                var list = entity.Select(Mapping).AsEnumerable();

                return ResponseWrapper.Success(list);
            }
            catch (Exception ex)
            {
                return ResponseWrapper.Failure<IEnumerable<ListCategoriesResponse>>(
                   Error.Set($"Ocorreu um erro inesperado ao executar a ação. Message: {ex.Message}. Stacktrace: {ex.Message}"),
                   ErrorType.InternalError);
            }
        }

        private static ListCategoriesResponse Mapping(Categoria categoria)
        {
            return new ListCategoriesResponse(
                categoria.Nome,
                (int)categoria.Tipo);
        }
    }
}
