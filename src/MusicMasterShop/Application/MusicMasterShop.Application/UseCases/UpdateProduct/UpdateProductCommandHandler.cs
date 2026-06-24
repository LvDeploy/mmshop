using MediatR;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.UseCases.UpdateProduct;

public sealed class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductRequest, BaseResponse<UpdateProductResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProdutoRepository _produtoRepository;
    private readonly ICategoriaRepository _categoriaRepository;

    public UpdateProductCommandHandler(
        IUnitOfWork unitOfWork,
        IProdutoRepository produtoRepository,
        ICategoriaRepository categoriaRepository)
    {
        _unitOfWork = unitOfWork;
        _produtoRepository = produtoRepository;
        _categoriaRepository = categoriaRepository;
    }

    public async Task<BaseResponse<UpdateProductResponse>> Handle(
        UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            if (!request.IsValid())
            {
                return ResponseWrapper.Failure<UpdateProductResponse>(
                    request.ValidationResult.Errors,
                    ErrorType.BadRequest);
            }

            Produto? produto = await _produtoRepository.GetWithDetailsAsync(
                request.Id,
                cancellationToken);

            if (produto is null)
            {
                return ResponseWrapper.Failure<UpdateProductResponse>(
                    Error.Set("Produto não encontrado"),
                    ErrorType.NotFound);
            }
            Categoria? categoria = null;
            if (request.TipoCategoriaId != null)
            {
                categoria = await _categoriaRepository.GetByTipoAsync(
                    request.TipoCategoriaId!.Value,
                    cancellationToken);


                if (categoria is null)
                {
                    return ResponseWrapper.Failure<UpdateProductResponse>(
                        Error.Set("Categoria não encontrada"),
                        ErrorType.NotFound);
                }
            }

            produto.Update(
                string.IsNullOrEmpty(request.Nome) ? produto.Nome : request.Nome,
                string.IsNullOrEmpty(request.Descricao) ? produto.Descricao : request.Descricao,
                request.Preco == 0 ? produto.Preco : request.Preco,
                request.TipoCategoriaId == null ? produto.Categoria : categoria);

            await _unitOfWork.CommitAsync(cancellationToken);

            return ResponseWrapper.Success(
                new UpdateProductResponse(produto.Id, produto.UpdatedAt));
        }
        catch (Exception ex)
        {
            return ResponseWrapper.Failure<UpdateProductResponse>(
               Error.Set($"Ocorreu um erro inesperado ao executar a ação. Message: {ex.Message}. Stacktrace: {ex.Message}"),
               ErrorType.InternalError);
        }
    }
}
