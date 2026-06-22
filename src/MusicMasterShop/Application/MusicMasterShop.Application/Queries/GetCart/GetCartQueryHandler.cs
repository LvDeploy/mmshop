using MediatR;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Application.Middleware.UserInfo;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.Domain.Enums;
using MusicMasterShop.Domain.ValueObjects;

namespace MusicMasterShop.Application.Queries.GetCart;

public sealed class GetCartQueryHandler
    : IRequestHandler<GetCartRequest, BaseResponse<GetCartResponse>>
{
    private readonly IUserInfo _userInfo;
    private readonly ICarrinhoRepository _carrinhoRepository;

    public GetCartQueryHandler(
        IUserInfo userInfo,
        ICarrinhoRepository carrinhoRepository)
    {
        _userInfo = userInfo;
        _carrinhoRepository = carrinhoRepository;
    }

    public async Task<BaseResponse<GetCartResponse>> Handle(
        GetCartRequest request,
        CancellationToken cancellationToken)
    {
        if (!_userInfo.IsAuthenticated
            || _userInfo.TipoUsuario != TipoUsuario.Vendedor
            || !_userInfo.Id.HasValue)
        {
            return ResponseWrapper.Failure<GetCartResponse>(
                Error.Set("Apenas usuários vendedores podem consultar o carrinho"),
                ErrorType.Forbidden);
        }

        Carrinho? carrinho = await _carrinhoRepository.GetActiveByUsuarioIdAsync(
            _userInfo.Id.Value,
            cancellationToken);

        if (carrinho is null)
        {
            return ResponseWrapper.Failure<GetCartResponse>(
                Error.Set("Carrinho ativo não encontrado"),
                ErrorType.NotFound);
        }

        GetCartItemResponse[] produtos = carrinho.Produtos
            .Select(MapItem)
            .ToArray();

        return ResponseWrapper.Success(
            new GetCartResponse(
                carrinho.Id,
                carrinho.CreatedAt,
                carrinho.UpdatedAt,
                produtos));
    }

    private static GetCartItemResponse MapItem(CarrinhoProduto item)
    {
        return new GetCartItemResponse(
            item.Produto.Id,
            item.Produto.Nome,
            item.Produto.Preco,
            item.Produto.QtdDisponivel,
            item.Quantidade);
    }
}
