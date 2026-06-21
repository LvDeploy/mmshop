using MediatR;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.UseCases.UpdateProductStorageCount;

public sealed class UpdateProductStorageCountCommandHandler
    : IRequestHandler<
        UpdateProductStorageCountRequest,
        BaseResponse<UpdateProductStorageCountResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProdutoRepository _produtoRepository;

    public UpdateProductStorageCountCommandHandler(
        IUnitOfWork unitOfWork,
        IProdutoRepository produtoRepository)
    {
        _unitOfWork = unitOfWork;
        _produtoRepository = produtoRepository;
    }

    public async Task<BaseResponse<UpdateProductStorageCountResponse>> Handle(
        UpdateProductStorageCountRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.IsValid())
        {
            return ResponseWrapper.Failure<UpdateProductStorageCountResponse>(
                request.ValidationResult.Errors,
                ErrorType.BadRequest);
        }

        Produto? produto = await _produtoRepository.Get(request.Id, cancellationToken);

        if (produto is null)
        {
            return ResponseWrapper.Failure<UpdateProductStorageCountResponse>(
                Error.Set("Produto não encontrado"),
                ErrorType.NotFound);
        }

        produto.AddQtdDisponivel((uint)request.Quantidade);
        await _unitOfWork.CommitAsync(cancellationToken);

        return ResponseWrapper.Success(
            new UpdateProductStorageCountResponse(produto.Id, produto.QtdDisponivel));
    }
}
