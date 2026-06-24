using MediatR;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.UseCases.DeleteProduct;

public sealed class DeleteProductCommandHandler
    : IRequestHandler<DeleteProductRequest, BaseResponse<bool>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProdutoRepository _produtoRepository;

    public DeleteProductCommandHandler(
        IUnitOfWork unitOfWork,
        IProdutoRepository produtoRepository)
    {
        _unitOfWork = unitOfWork;
        _produtoRepository = produtoRepository;
    }

    public async Task<BaseResponse<bool>> Handle(
        DeleteProductRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            Produto? produto = await _produtoRepository.GetWithDetailsAsync(
                request.Id,
                cancellationToken);

            if (produto is null)
            {
                return ResponseWrapper.Failure<bool>(
                    Error.Set("Produto não encontrado"),
                    ErrorType.NotFound);
            }

            _produtoRepository.Delete(produto);
            await _unitOfWork.CommitAsync(cancellationToken);

            return ResponseWrapper.Success(true);
        }
        catch (Exception ex)
        {
            return ResponseWrapper.Failure<bool>(
               Error.Set($"Ocorreu um erro inesperado ao executar a ação. Message: {ex.Message}. Stacktrace: {ex.StackTrace}"),
               ErrorType.InternalError);
        }
    }
}
