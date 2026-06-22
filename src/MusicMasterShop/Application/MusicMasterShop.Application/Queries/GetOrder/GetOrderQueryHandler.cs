using MediatR;
using MusicMasterShop.Application.Abstractions.Response;
using MusicMasterShop.Application.Middleware.UserInfo;
using MusicMasterShop.Domain.Contracts.Repositories;
using MusicMasterShop.Domain.Core.Result;
using MusicMasterShop.Domain.Entities;
using MusicMasterShop.Domain.Enums;
using MusicMasterShop.Domain.ValueObjects;

namespace MusicMasterShop.Application.Queries.GetOrder;

public sealed class GetOrderQueryHandler
    : IRequestHandler<GetOrderRequest, BaseResponse<GetOrderResponse>>
{
    private readonly IUserInfo _userInfo;
    private readonly IPedidoRepository _pedidoRepository;

    public GetOrderQueryHandler(
        IUserInfo userInfo,
        IPedidoRepository pedidoRepository)
    {
        _userInfo = userInfo;
        _pedidoRepository = pedidoRepository;
    }

    public async Task<BaseResponse<GetOrderResponse>> Handle(
        GetOrderRequest request,
        CancellationToken cancellationToken)
    {
        if (!_userInfo.IsAuthenticated
            || _userInfo.TipoUsuario != TipoUsuario.Vendedor
            || !_userInfo.Id.HasValue)
        {
            return ResponseWrapper.Failure<GetOrderResponse>(
                Error.Set("Apenas usuarios vendedores podem consultar pedidos"),
                ErrorType.Forbidden);
        }

        Pedido? pedido = await _pedidoRepository.GetWithDetailsAsync(
            request.PedidoId,
            cancellationToken);

        if (pedido is null)
        {
            return ResponseWrapper.Failure<GetOrderResponse>(
                Error.Set("Pedido nao encontrado"),
                ErrorType.NotFound);
        }

        if (pedido.Carrinho.UsuarioId != _userInfo.Id.Value)
        {
            return ResponseWrapper.Failure<GetOrderResponse>(
                Error.Set("O pedido nao pertence ao usuario autenticado"),
                ErrorType.Forbidden);
        }

        GetOrderItemResponse[] produtos = pedido.Carrinho.Produtos
            .Select(MapItem)
            .ToArray();

        return ResponseWrapper.Success(
            new GetOrderResponse(
                pedido.Id,
                pedido.CarrinhoId,
                pedido.DocumentoCliente,
                pedido.Status,
                pedido.CreatedAt,
                pedido.UpdatedAt,
                produtos));
    }

    private static GetOrderItemResponse MapItem(CarrinhoProduto item)
    {
        return new GetOrderItemResponse(
            item.Produto.Id,
            item.Produto.Nome,
            item.Produto.Preco,
            item.Quantidade);
    }
}
