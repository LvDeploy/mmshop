using MediatR;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Application.Middleware.UserInfo;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.Domain.Enums;

namespace MusicMasterShop.Application.UseCases.UpdateCart;

public sealed class UpdateCartCommandHandler
    : IRequestHandler<UpdateCartRequest, BaseResponse<UpdateCartResponse>>
{
    private readonly IUserInfo _userInfo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICarrinhoRepository _carrinhoRepository;
    private readonly IProdutoRepository _produtoRepository;

    public UpdateCartCommandHandler(
        IUserInfo userInfo,
        IUnitOfWork unitOfWork,
        ICarrinhoRepository carrinhoRepository,
        IProdutoRepository produtoRepository)
    {
        _userInfo = userInfo;
        _unitOfWork = unitOfWork;
        _carrinhoRepository = carrinhoRepository;
        _produtoRepository = produtoRepository;
    }

    public async Task<BaseResponse<UpdateCartResponse>> Handle(
        UpdateCartRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.IsValid())
        {
            return ResponseWrapper.Failure<UpdateCartResponse>(
                request.ValidationResult.Errors,
                ErrorType.BadRequest);
        }

        if (!_userInfo.IsAuthenticated
            || _userInfo.TipoUsuario != TipoUsuario.Vendedor
            || !_userInfo.Id.HasValue)
        {
            return ResponseWrapper.Failure<UpdateCartResponse>(
                Error.Set("Apenas usuários vendedores podem atualizar carrinhos"),
                ErrorType.Forbidden);
        }

        Carrinho? carrinho = await _carrinhoRepository.GetWithProductsAsync(
            request.CarrinhoId,
            cancellationToken);

        if (carrinho is null)
        {
            return ResponseWrapper.Failure<UpdateCartResponse>(
                Error.Set("Carrinho não encontrado"),
                ErrorType.NotFound);
        }

        if (carrinho.UsuarioId != _userInfo.Id.Value)
        {
            return ResponseWrapper.Failure<UpdateCartResponse>(
                Error.Set("O carrinho não pertence ao usuário autenticado"),
                ErrorType.Forbidden);
        }

        if (!carrinho.Ativo)
        {
            return ResponseWrapper.Failure<UpdateCartResponse>(
                Error.Set("Carrinho está inativo"),
                ErrorType.BadRequest);
        }

        foreach (UpdateCartItemRequest item in request.Produtos)
        {
            if (item.Quantidade == 0)
            {
                carrinho.RemoveProduto(item.ProdutoId);
                continue;
            }

            Produto? produto = await _produtoRepository.Get(
                item.ProdutoId,
                cancellationToken);

            if (produto is null)
            {
                return ResponseWrapper.Failure<UpdateCartResponse>(
                    Error.Set($"Produto {item.ProdutoId} não encontrado"),
                    ErrorType.NotFound);
            }

            carrinho.AddOrUpdateProduto(produto, item.Quantidade);
        }

        await _unitOfWork.CommitAsync(cancellationToken);

        return ResponseWrapper.Success(
            new UpdateCartResponse(carrinho.Id, carrinho.Produtos.Count));
    }
}
