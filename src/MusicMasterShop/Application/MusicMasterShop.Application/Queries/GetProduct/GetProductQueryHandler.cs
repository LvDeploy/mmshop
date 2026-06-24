using MediatR;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.Queries.GetProduct
{
    public class GetProductQueryHandler : IRequestHandler<GetProductRequest, BaseResponse<GetProductResponse>>
    {
        private readonly IProdutoRepository _produtoRepository;
        public GetProductQueryHandler(IProdutoRepository produtoRepository)
        {
            _produtoRepository = produtoRepository;
        }

        public async Task<BaseResponse<GetProductResponse>> Handle(GetProductRequest request, CancellationToken cancellationToken)
        {
            try
            {
                var entity = await _produtoRepository.GetWithDetailsAsync(request.Id, cancellationToken);

                if (entity is null)
                {
                    return ResponseWrapper.Failure<GetProductResponse>(Error.Set("Registro não encontrado"), ErrorType.NotFound);
                }

                return ResponseWrapper.Success(new GetProductResponse(
                    entity.Id,
                    entity.Nome,
                    entity.Descricao,
                    entity.NumeroNotaFiscal,
                    entity.Preco,
                    entity.QtdDisponivel,
                    entity.Categoria.Tipo));
            }
            catch (Exception ex)
            {
                return ResponseWrapper.Failure<GetProductResponse>(
                   Error.Set($"Ocorreu um erro inesperado ao executar a ação. Message: {ex.Message}. Stacktrace: {ex.Message}"),
                   ErrorType.InternalError);
            }
        }
    }
}
